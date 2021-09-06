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
    
    [Tooltip("显示关卡数的文本"), SerializeField]
    private Text levelText;

    [Tooltip("显示波数信息的文本"), SerializeField]
    private Text waveText;

    [Tooltip("左下角车厢信息显示管理，每单元代表一个车厢（包括车厢图像及生命值显示）"), SerializeField]
    private Transform[] unitInfoIcons;

    [Tooltip("结束文本显示面板"), SerializeField]
    private EndPanel endPanel;

    [Tooltip("游戏最终胜利的文本"), SerializeField]
    private Text gameEndText;

    [Header("开始文本显示参数")]
    [Tooltip("开始文本每次显示多久"), SerializeField, Range(0f, 1f)]
    private float startHintTextShowTime = 1f;
    [Tooltip("开始文本每次切换时闪烁白色的时间"), SerializeField, Range(0f, 0.5f)]
    private float startHintTextFlashTime = 0.05f;
    [Tooltip("开始文本除去闪烁时间显示多久")]
    private float startHintTextRealShowTime;
    [Tooltip("开始文本每次隐藏多久"), SerializeField, Range(0f, 0.5f)]
    private float startHintTextHideTime = 0.3f;

    private void OnValidate()
    {
        if(startHintTextFlashTime > startHintTextShowTime)
        {
            startHintTextFlashTime = startHintTextShowTime;
        }

        startHintTextRealShowTime = startHintTextShowTime - startHintTextFlashTime;
    }

    private void Awake()
    {
        startHintTextRealShowTime = startHintTextShowTime - startHintTextFlashTime;
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

    private void RoundStart()
    {
        startHintText.gameObject.SetActive(false);
        levelText.gameObject.SetActive(false);
        waveText.gameObject.SetActive(false);

        int trainUnitCount = train.unitCount;

        //设置生命值文本数值
        //目前还是统一采用最大生命值
        for (int i = 0; i < trainUnitCount; i++)
        {
            unitInfoIcons[i].gameObject.SetActive(true);
            Image image = unitInfoIcons[i].GetComponentsInChildren<Image>()[1];
            image.color = train.unitColors[i];
            Text text = unitInfoIcons[i].GetComponentInChildren<Text>();
            text.text = train.unitMaxHealths[i].ToString();
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
        //yield return new WaitForSeconds(startHintTextShowTime);

        for(int i = 3; i >= 1; i--)
        {
            startHintText.text = i.ToString();
            startHintText.gameObject.SetActive(true);
            if (i == 3)
            {
                yield return new WaitForSeconds(startHintTextShowTime);
            }
            else
            {
                startHintText.color = Color.white;
                int originalSize = startHintText.fontSize;
                startHintText.fontSize = originalSize + 16;
                yield return TextSizeShrinkOverTime(startHintTextFlashTime, originalSize + 16, originalSize);
                //yield return new WaitForSeconds(startHintTextFlashTime);
                startHintText.color = Color.black;
                startHintText.fontSize = originalSize;
                yield return new WaitForSeconds(startHintTextRealShowTime);
            }

            startHintText.gameObject.SetActive(false);
            if(i != 1)
            {
                yield return new WaitForSeconds(startHintTextHideTime);
            }
        }

        GameManager.roundStart = true;

        levelText.text = "关卡 : " + GameManager.currentLevel;
        levelText.gameObject.SetActive(true);
        waveText.gameObject.SetActive(true);
    }

    /// <summary>
    /// 321的倒计时文本需要由大到小的变化过程
    /// </summary>
    /// <param name="time">从大变小要多久</param>
    /// <param name="beginSize">从多大开始变</param>
    /// <param name="endSize">变回原来的字号</param>
    /// <returns></returns>
    private IEnumerator TextSizeShrinkOverTime(float time, int beginSize, int endSize)
    {
        for(float t = Time.deltaTime / time; t <= 1; t+= Time.deltaTime/time)
        {
            startHintText.fontSize = (int)Mathf.Lerp(beginSize, endSize, t);
            yield return null;
        }
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
    /// 调整波数显示
    /// </summary>
    /// <param name="currentWave">当前波数</param>
    /// <param name="totalWave">总波数</param>
    public void SetWaveText(int currentWave, int totalWave)
    {
        currentWave++;
        waveText.text = "波数 : " + currentWave + " / " + totalWave;
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

    public void ShowGameEndText()
    {
        endPanel.gameObject.SetActive(true);
        gameEndText.gameObject.SetActive(true);
    }
}
