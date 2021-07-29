using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance
    {
        get
        {
            return instance;
        }
    }
    private static UIManager instance;

    [Header("外部引用")]
    [Tooltip("用于获取列车车厢生命值信息"), SerializeField]
    private TrainManager train;

    [Header("UI组件")]
    [Tooltip("游戏开始时的提示文本"), SerializeField]
    private Text startHintText;

    [Tooltip("左下角车厢信息显示管理，每单元代表一个车厢（包括车厢图像及生命值显示）"), SerializeField]
    private Transform[] unitInfoIcons;

    [Tooltip("结束文本显示面板"), SerializeField]
    private EndPanel endPanel;

    [Header("开始文本显示参数")]
    [Tooltip("开始文本每次显示多久"), SerializeField, Range(0f, 1f)]
    private float startHintTextShowTime = 1f;
    [Tooltip("开始文本每次隐藏多久"), SerializeField, Range(0f, 0.5f)]
    private float startHintTextHideTime = 0.3f;

    private void Awake()
    {
        instance = this;
    }

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
        startHintText.gameObject.SetActive(false);

        int trainUnitCount = train.unitCount;

        //设置生命值文本数值
        //目前还是统一采用最大生命值
        for (int i = 0; i < trainUnitCount; i++)
        {
            unitInfoIcons[i].gameObject.SetActive(true);
            Image image = unitInfoIcons[i].GetComponentsInChildren<Image>()[1];
            image.color = train.unitColors[i];
            Text text = unitInfoIcons[i].GetComponentInChildren<Text>();
            text.text = train.unitMaxHealth.ToString();
        }

        //根据车厢数量决定左下角车厢信息板块数量
        for (int i = trainUnitCount; i <= 5; i++)
        {
            unitInfoIcons[i].gameObject.SetActive(false);
        }

        endPanel.gameObject.SetActive(false);

        StartCoroutine(ShowStartHint());
    }

    private void Update()
    {
        //仅在游戏开始时执行一次
        //if (isStart)
        //{
            
        //    isStart = false;
        //}
    }

    /// <summary>
    /// 显示开头的321倒计时文本
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowStartHint()
    {
        yield return new WaitForSeconds(startHintTextShowTime);

        for(int i = 3; i >= 1; i--)
        {
            startHintText.text = i.ToString();
            startHintText.gameObject.SetActive(true);
            yield return new WaitForSeconds(startHintTextShowTime);

            startHintText.gameObject.SetActive(false);
            if(i != 1)
            {
                yield return new WaitForSeconds(startHintTextHideTime);
            }
        }
        //startHintText.text = "3";
        //startHintText.gameObject.SetActive(true);
        //yield return new WaitForSeconds(startHintTextShowTime);

        //startHintText.gameObject.SetActive(false);
        //yield return new WaitForSeconds(startHintTextHideTime);

        //startHintText.text = "2";
        //startHintText.gameObject.SetActive(true);
        //yield return new WaitForSeconds(startHintTextShowTime);

        //startHintText.gameObject.SetActive(false);
        //yield return new WaitForSeconds(startHintTextHideTime);

        //startHintText.text = "1";
        //startHintText.gameObject.SetActive(true);
        //yield return new WaitForSeconds(startHintTextShowTime);

        //startHintText.gameObject.SetActive(false);

        GameManager.roundStart = true;
    }

    /// <summary>
    /// 调整生命值文本数值
    /// </summary>
    /// <param name="index">车厢</param>
    /// <param name="currentHealth">车厢当前生命值</param>
    public void ChangeUnitHealthInfo(int index, int currentHealth)
    {
        unitInfoIcons[index].GetComponentInChildren<Text>().text = currentHealth.ToString();
    }

    /// <summary>
    /// 游戏结束，显示信息
    /// </summary>
    /// <param name="success"></param>
    public void ShowEndText(bool success)
    {
        endPanel.gameObject.SetActive(true);
        endPanel.ShowEndText(success);
    }
}
