using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理火车路线，持有车厢及路线的引用
/// </summary>
public class TrainManager : MonoBehaviour
{
    public static TrainManager Instance
    {
        get
        {
            return instance;
        }
    }
    private static TrainManager instance;

    [Header("外部引用")]
    //[Tooltip("用于通知车厢信息显示更新"), SerializeField]
    //private UIManager uiManager;

    [SerializeField, Tooltip("用于获取节点具体位置和路线规划")]
    private RouteManager routeManager;

    [SerializeField, Tooltip("车厢prefab")]
    private UnitMove unitPrefab;

    [Header("列车参数")]
    [Tooltip("火车移动的基本速度")]
    public float speed = 5f;

    [SerializeField, Tooltip("火车车厢半径，用于初始生成")]
    private float unitRadius = 0.5f;
    [SerializeField, Tooltip("火车车厢间距，用于初始生成")]
    private float unitInterval = 0.05f;
    [Tooltip("火车车厢数量"), Range(1, 6)]
    public int unitCount = 3;

    [HideInInspector, Tooltip("所有车厢的引用")]
    public List<Transform> trainUnits;
    [Tooltip("目前存活的车厢，用于给敌人提供定位")]
    private List<int> aliveIndices;
    //public List<Transform> deadUnits;
    [Tooltip("路线所有节点的索引")]
    private List<int> pointIndices;

    [Tooltip("车厢在进度到达多少之后可以确定下一个节点")]
    public const float searchAheadProgress = 0.9f;

    [Tooltip("转向的输入")]
    private float horizontalInput = 0f;
    [Tooltip("加/减速的输入"), HideInInspector]
    public float verticalInput = 0f;

    [Header("车厢参数")]
    [Tooltip("各车厢的最大生命值"), Range(1, 6)]
    public int unitMaxHealth = 5;
    public List<Color> unitColors;

    //private void Start()
    //{
    //    RoundStart();
    //}

    private void OnEnable()
    {
        RoundStart();
    }

    public void RoundStart()
    {
        trainUnits = new List<Transform>();
        //deadUnits = new List<Transform>();
        aliveIndices = new List<int>();
        unitColors = new List<Color>();
        for (int i = 0; i < unitCount; i++)
        {
            aliveIndices.Add(i);
            unitColors.Add(Random.ColorHSV());
        }

        Vector3 initialPos = routeManager.GetPointPosition(0);
        for (int i = 0; i < unitCount; i++)
        {
            //生成单个车厢，并初始化
            UnitMove unit = Instantiate(unitPrefab, initialPos, Quaternion.identity, transform);
            unit.SetTrainManager(this);
            //初始化车厢颜色
            MeshRenderer renderer = unit.GetComponentInChildren<MeshRenderer>();
            renderer.material.color = unitColors[i];
            //初始化移动信息
            UnitMovementInfo info = new UnitMovementInfo(routeManager.GetPointPosition(0), routeManager.GetPointPosition(1));
            unit.SetInfo(info);
            //初始化车厢信息
            UnitInfo unitInfo = unit.GetComponent<UnitInfo>();
            unitInfo.SetHealth(unitMaxHealth);
            unitInfo.SetOnDeath(RearrangeUnits);
            unitInfo.SetOnHurt(UnitHealthChange);

            //记录车厢引用
            Transform unitTF = unit.transform;
            trainUnits.Add(unitTF);

            //先隐藏车厢
            unit.gameObject.SetActive(false);
        }

        StartCoroutine(InstantiateTrain(initialPos));

        //初始化路线，先设定好两个初始节点
        pointIndices = new List<int>
        {
            0, 1
        };
    }

    /// <summary>
    /// 生成列车各车厢
    /// </summary>
    /// <param name="initialPos">车厢生成点</param>
    /// <returns></returns>
    private IEnumerator InstantiateTrain(Vector3 initialPos)
    {
        //等到321倒计时结束后再生成
        //while(GameManager.Instance.roundStart == false)
        //{
        //    yield return null;
        //}

        //为防止车厢之间间距不均衡，等待start结束后再生成
        //yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < unitCount; i++)
        {
            Transform unitTF = trainUnits[i];
            unitTF.gameObject.SetActive(true);

            //等待至此车厢走出一段距离后再生成下一节车厢
            while (Vector3.Distance(unitTF.position, initialPos) <= unitRadius + unitRadius + unitInterval)
            {
                yield return null;
            }
        }
    }

    /// <summary>
    /// 有车厢被消灭时重排车厢
    /// </summary>
    /// <param name="deadUnitTF">死亡的车厢</param>
    private void RearrangeUnits(Transform deadUnitTF)
    {
        //Time.timeScale = 0.1f;
        int deadUnitIndex = trainUnits.IndexOf(deadUnitTF);
        aliveIndices.Remove(deadUnitIndex);
        if(aliveIndices.Count == 0)
        {
            GameManager.Instance.GameEnd(false);
        }
        else
        {
            //由于即时重排容易出BUG（不知道），因此采用协程延时重排
            StartCoroutine(StartRearrangement(deadUnitIndex));
            //for (int i = unitCount - 2; i >= deadUnitIndex; i--)
            //{
            //    UnitMovementInfo info = trainUnits[i].GetComponent<UnitMove>().GetInfo();
            //    trainUnits[i + 1].GetComponent<UnitMove>().SetInfo(info);
            //}
        }
    }

    /// <summary>
    /// 重排车厢
    /// </summary>
    /// <param name="index">死亡（重排）车厢位置</param>
    /// <returns></returns>
    private IEnumerator StartRearrangement(int index)
    {
        //等到物理判断结束后重排，防止BUG
        yield return new WaitForFixedUpdate();

        //将此车厢之后的所有车厢向前移动一个
        for (int i = unitCount - 2; i >= index; i--)
        {
            yield return null;
            UnitMovementInfo info = trainUnits[i].GetComponent<UnitMove>().GetInfo();
            trainUnits[i + 1].GetComponent<UnitMove>().SetInfo(info);
        }
    }

    /// <summary>
    /// 在车厢生命值改变时通知UI
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="currentHealth"></param>
    private void UnitHealthChange(UnitInfo unit, int currentHealth)
    {
        int index = trainUnits.IndexOf(unit.transform);
        UIManager.Instance.ChangeUnitHealthInfo(index, currentHealth);
    }

    private void Update()
    {
        //获取两个输入
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    /// <summary>
    /// 根据当前路线节点获取下一个节点的位置
    /// </summary>
    /// <param name="pointIndex">当前车厢将要到达的路线节点索引</param>
    /// <returns>下一个节点位置</returns>
    public Vector3 GetNextRoutePointPosition(ref int pointIndex)
    {
        //找到该节点在目前火车路线中的位置
        //考虑到可能与以前经过的节点重复，获取其第一及最后一次出现的位置
        int indexLastPosition = pointIndices.LastIndexOf(pointIndex);
        int indexFirstPosition = pointIndices.IndexOf(pointIndex);
        
        //如果这是当前路线最前沿的节点，就要更新路线的下一个节点
        if(indexLastPosition == pointIndices.Count - 1)
        {
            GetNextDestination();
        }

        //节点更新为路线中下一个节点
        pointIndex = pointIndices[indexLastPosition + 1];

        //如果路线中此节点重复，删除掉以前的节点
        if (indexFirstPosition != indexLastPosition)
        {
            pointIndices.RemoveAt(indexFirstPosition);
        }

        //返回节点位置
        return routeManager.GetPointPosition(pointIndex);
    }

    /// <summary>
    /// 获取列车路线的下一个节点
    /// </summary>
    private void GetNextDestination()
    {
        //根据当前路线最后两个节点及转向输入获取下一个节点
        int pointCount = pointIndices.Count;
        int nextDestinationIndex = 
            routeManager.GetNextIndex(pointIndices[pointCount - 2], pointIndices[pointCount - 1], horizontalInput);
        pointIndices.Add(nextDestinationIndex);
    }

    /// <summary>
    /// 获得指定索引车厢的位置
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Vector3 GetTrainUnitPos(int index = 0)
    {
        return CheckIndex(index) ? trainUnits[index].position : Vector3.up;
    }

    /// <summary>
    /// 获得列车现存车厢的随机引用
    /// </summary>
    /// <returns>在现存车厢中随机的一个引用</returns>
    public int GetRandomIndexOfUnits()
    {
        if (GameManager.roundEnd)
        {
            return -1;
        }
        else
        {
            int randomIndex = aliveIndices[Random.Range(0, aliveIndices.Count)];
            return randomIndex;
        }
    }

    /// <summary>
    /// 根据索引返回车厢信息引用（用于敌人设置车厢的死亡回调函数）
    /// </summary>
    /// <param name="index">指定车厢引用</param>
    /// <returns>指定车厢的信息</returns>
    public UnitInfo GetUnitByIndex(int index)
    {
        CheckIndex(index);

        return trainUnits[index].GetComponent<UnitInfo>();
    }

    /// <summary>
    /// 在用索引获取车厢前先判断索引的合理性
    /// </summary>
    /// <param name="index">要获取车厢的索引</param>
    private bool CheckIndex(int index)
    {
        //如果该车厢不存在
        if(index >= unitCount)
        {
            Debug.LogError("Try to access to none exist unit!");
        }
        //如果该车厢已死亡
        else if (!aliveIndices.Contains(index))
        {
            //Debug.LogError("Try to access to dead unit!");
            return false;
        }

        return true;
    }
}