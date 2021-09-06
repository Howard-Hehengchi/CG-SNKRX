using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����·�ߣ����г��ἰ·�ߵ�����
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

    [Header("�ⲿ����")]
    //[Tooltip("����֪ͨ������Ϣ��ʾ����"), SerializeField]
    //private UIManager uiManager;

    [Tooltip("���ڻ�ȡ����ʵ��"), SerializeField]
    private UnitFactory unitFactory;

    [SerializeField, Tooltip("���ڻ�ȡ�ڵ����λ�ú�·�߹滮")]
    private RouteManager routeManager;

    [SerializeField, Tooltip("����prefab")]
    private UnitMove unitPrefab;

    [Header("�г�����")]
    [Tooltip("���ƶ��Ļ����ٶ�")]
    public float speed = 5f;

    [SerializeField, Tooltip("�𳵳���뾶�����ڳ�ʼ����")]
    private float unitRadius = 0.5f;
    [SerializeField, Tooltip("�𳵳����࣬���ڳ�ʼ����")]
    private float unitInterval = 0.05f;
    [SerializeField, Tooltip("��ÿ������ֱ���ʲô��������ʱ����")]
    private List<UnitType> unitTypes;
    [HideInInspector, Tooltip("�𳵳�������")]
    public int unitCount;

    [HideInInspector, Tooltip("���г��������")]
    public List<Transform> trainUnits;
    [Tooltip("Ŀǰ���ĳ��ᣬ���ڸ������ṩ��λ")]
    private List<int> aliveIndices;
    //public List<Transform> deadUnits;
    [Tooltip("·�����нڵ������")]
    private List<int> pointIndices;

    [Tooltip("�����ڽ��ȵ������֮�����ȷ����һ���ڵ�")]
    public const float searchAheadProgress = 0.9f;

    [Tooltip("ת�������")]
    private float horizontalInput = 0f;
    [Tooltip("��/���ٵ�����"), HideInInspector]
    public float verticalInput = 0f;

    [HideInInspector, Tooltip("��������ɫ��������UI�ṩ��ʾ��Ϣ")]
    public List<Color> unitColors;
    [HideInInspector, Tooltip("����������ֵ��������UI�ṩ��ʾ��Ϣ")]
    public List<int> unitMaxHealths;

    private void Awake()
    {
        instance = this;
        unitCount = unitTypes.Count;
    }

    //private void Start()
    //{
    //    RoundStart();
    //}

    private void OnEnable()
    {
        if(GameManager.train == null)
        {
            GameManager.train = unitTypes;
        }
        RoundStart();
    }

    /// <summary>
    /// �غϿ�ʼ����ʼ���г���Ϣ
    /// </summary>
    public void RoundStart()
    {
        unitTypes = GameManager.train;
        unitCount = unitTypes.Count;

        trainUnits = new List<Transform>();
        //deadUnits = new List<Transform>();
        aliveIndices = new List<int>();
        unitColors = new List<Color>();
        unitMaxHealths = new List<int>();

        //���ǵ���ʼ���ɵ㲻һ����ԭ�㣬Ϊ��ר�ż�¼λ��
        Vector3 initialPos = routeManager.GetPointPosition(0);
        //һ��������������ɺͳ�ʼ��
        for (int i = 0; i < unitCount; i++)
        {
            //��¼����
            aliveIndices.Add(i);

            //���ɵ������ᣬ����ʼ��
            Unit unit = unitFactory.Get(unitTypes[i]);
            //��¼��������
            Transform unitTF = unit.transform;
            trainUnits.Add(unitTF);
            //����transform�йز���
            unitTF.SetPositionAndRotation(initialPos, Quaternion.identity);
            unitTF.SetParent(transform);

            //��ʼ��������ɫ
            MeshRenderer renderer = unit.GetComponentInChildren<MeshRenderer>();
            renderer.material.color = unit.color;
            unitColors.Add(unit.color);

            //��ȡ��������ֵ��Ϣ
            unitMaxHealths.Add(unit.maxHealth);

            //��ʼ������ص�����
            //Unit unit = unitMove.GetComponent<Unit>();
            unit.AddOnDeath(RearrangeUnits);
            unit.AddOnHurt(UnitHealthChange);

            //���ó����˶��ű�
            UnitMove unitMove = unit.GetComponent<UnitMove>();
            unitMove.SetTrainManager(this);
            //��ʼ���ƶ���Ϣ
            UnitMovementInfo info = new UnitMovementInfo(routeManager.GetPointPosition(0), routeManager.GetPointPosition(1));
            unitMove.SetInfo(info);
            
            //�����س���
            unitMove.gameObject.SetActive(false);
        }

        StartCoroutine(InstantiateTrain(initialPos));

        //��ʼ��·�ߣ����趨��������ʼ�ڵ�
        pointIndices = new List<int>
        {
            0, 1
        };
    }

    /// <summary>
    /// �����г�������
    /// </summary>
    /// <param name="initialPos">�������ɵ�</param>
    /// <returns></returns>
    private IEnumerator InstantiateTrain(Vector3 initialPos)
    {
        //�ȵ�321����ʱ������������
        //while(GameManager.Instance.roundStart == false)
        //{
        //    yield return null;
        //}

        //Ϊ��ֹ����֮���಻���⣬�ȴ�start������������
        //yield return new WaitForEndOfFrame();

        //�ӳ�һ�㿪ʼ���ɣ����õ�321����ʱ֮���ٳ���
        yield return new WaitForSeconds(0.5f);

        //һ������ʾ����
        for (int i = 0; i < unitCount; i++)
        {
            Transform unitTF = trainUnits[i];
            unitTF.gameObject.SetActive(true);

            //�ȴ����˳����߳�һ�ξ������������һ�ڳ���
            while (Vector3.Distance(unitTF.position, initialPos) <= unitRadius + unitRadius + unitInterval)
            {
                yield return null;
            }
        }
    }

    /// <summary>
    /// �г��ᱻ����ʱ���ų���
    /// </summary>
    /// <param name="deadUnitTF">�����ĳ���</param>
    private void RearrangeUnits(Transform deadUnitTF)
    {
        //Time.timeScale = 0.1f;
        int deadUnitIndex = trainUnits.IndexOf(deadUnitTF);
        aliveIndices.Remove(deadUnitIndex);
        if(aliveIndices.Count == 0)
        {
            GameManager.Instance.RoundFinish(false);
        }
        else
        {
            //���ڼ�ʱ�������׳�BUG����֪��������˲���Э����ʱ����
            StartCoroutine(StartRearrangement(deadUnitIndex));
            //for (int i = unitCount - 2; i >= deadUnitIndex; i--)
            //{
            //    UnitMovementInfo info = trainUnits[i].GetComponent<UnitMove>().GetInfo();
            //    trainUnits[i + 1].GetComponent<UnitMove>().SetInfo(info);
            //}
        }
    }

    /// <summary>
    /// ���ų���
    /// </summary>
    /// <param name="index">���������ţ�����λ��</param>
    /// <returns></returns>
    private IEnumerator StartRearrangement(int index)
    {
        //�ȵ������жϽ��������ţ���ֹBUG
        yield return new WaitForFixedUpdate();

        //���˳���֮������г�����ǰ�ƶ�һ��
        for (int i = unitCount - 2; i >= index; i--)
        {
            yield return null;
            UnitMovementInfo info = trainUnits[i].GetComponent<UnitMove>().GetInfo();
            trainUnits[i + 1].GetComponent<UnitMove>().SetInfo(info);
        }
    }

    /// <summary>
    /// �ڳ�������ֵ�ı�ʱ֪ͨUI
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="currentHealth"></param>
    private void UnitHealthChange(Unit unit, int currentHealth)
    {
        int index = trainUnits.IndexOf(unit.transform);
        UIManager.Instance.ChangeUnitHealthInfo(index, currentHealth);
    }

    private void Update()
    {
        //��ȡ��������
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    /// <summary>
    /// ���ݵ�ǰ·�߽ڵ��ȡ��һ���ڵ��λ��
    /// </summary>
    /// <param name="pointIndex">��ǰ���ὫҪ�����·�߽ڵ�����</param>
    /// <returns>��һ���ڵ�λ��</returns>
    public Vector3 GetNextRoutePointPosition(ref int pointIndex)
    {
        //�ҵ��ýڵ���Ŀǰ��·���е�λ��
        //���ǵ���������ǰ�����Ľڵ��ظ�����ȡ���һ�����һ�γ��ֵ�λ��
        int indexLastPosition = pointIndices.LastIndexOf(pointIndex);
        int indexFirstPosition = pointIndices.IndexOf(pointIndex);
        
        //������ǵ�ǰ·����ǰ�صĽڵ㣬��Ҫ����·�ߵ���һ���ڵ�
        if(indexLastPosition == pointIndices.Count - 1)
        {
            GetNextDestination();
        }

        //�ڵ����Ϊ·������һ���ڵ�
        pointIndex = pointIndices[indexLastPosition + 1];

        //���·���д˽ڵ��ظ���ɾ������ǰ�Ľڵ�
        if (indexFirstPosition != indexLastPosition)
        {
            pointIndices.RemoveAt(indexFirstPosition);
        }

        //���ؽڵ�λ��
        return routeManager.GetPointPosition(pointIndex);
    }

    /// <summary>
    /// ��ȡ�г�·�ߵ���һ���ڵ�
    /// </summary>
    private void GetNextDestination()
    {
        //���ݵ�ǰ·����������ڵ㼰ת�������ȡ��һ���ڵ�
        int pointCount = pointIndices.Count;
        int nextDestinationIndex = 
            routeManager.GetNextIndex(pointIndices[pointCount - 2], pointIndices[pointCount - 1], horizontalInput);
        pointIndices.Add(nextDestinationIndex);
    }

    /// <summary>
    /// ���ָ�����������λ��
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Vector3 GetTrainUnitPos(int index = 0)
    {
        return CheckIndex(index) ? trainUnits[index].position : Vector3.up;
    }

    /// <summary>
    /// ����г��ִ泵����������
    /// </summary>
    /// <returns>���ִ泵���������һ������</returns>
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
    /// �����������س�����Ϣ���ã����ڵ������ó���������ص�������
    /// </summary>
    /// <param name="index">ָ����������</param>
    /// <returns>ָ���������Ϣ</returns>
    public Unit GetUnitByIndex(int index)
    {
        CheckIndex(index);

        return trainUnits[index].GetComponent<Unit>();
    }

    /// <summary>
    /// ����������ȡ����ǰ���ж������ĺ�����
    /// </summary>
    /// <param name="index">Ҫ��ȡ���������</param>
    private bool CheckIndex(int index)
    {
        //����ó��᲻����
        if(index >= unitCount)
        {
            Debug.LogError("Try to access to none exist unit!");
        }
        //����ó���������
        else if (!aliveIndices.Contains(index))
        {
            //Debug.LogError("Try to access to dead unit!");
            return false;
        }

        return true;
    }
}