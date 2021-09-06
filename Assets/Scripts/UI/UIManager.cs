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

    [Header("�ⲿ����")]
    [Tooltip("���ڻ�ȡ�г���������ֵ��Ϣ"), SerializeField]
    private TrainManager train;

    [Header("UI���")]
    [Tooltip("��Ϸ��ʼʱ����ʾ�ı�"), SerializeField]
    private Text startHintText;
    
    [Tooltip("��ʾ�ؿ������ı�"), SerializeField]
    private Text levelText;

    [Tooltip("��ʾ������Ϣ���ı�"), SerializeField]
    private Text waveText;

    [Tooltip("���½ǳ�����Ϣ��ʾ����ÿ��Ԫ����һ�����ᣨ��������ͼ������ֵ��ʾ��"), SerializeField]
    private Transform[] unitInfoIcons;

    [Tooltip("�����ı���ʾ���"), SerializeField]
    private EndPanel endPanel;

    [Tooltip("��Ϸ����ʤ�����ı�"), SerializeField]
    private Text gameEndText;

    [Header("��ʼ�ı���ʾ����")]
    [Tooltip("��ʼ�ı�ÿ����ʾ���"), SerializeField, Range(0f, 1f)]
    private float startHintTextShowTime = 1f;
    [Tooltip("��ʼ�ı�ÿ���л�ʱ��˸��ɫ��ʱ��"), SerializeField, Range(0f, 0.5f)]
    private float startHintTextFlashTime = 0.05f;
    [Tooltip("��ʼ�ı���ȥ��˸ʱ����ʾ���")]
    private float startHintTextRealShowTime;
    [Tooltip("��ʼ�ı�ÿ�����ض��"), SerializeField, Range(0f, 0.5f)]
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

        //��������ֵ�ı���ֵ
        //Ŀǰ����ͳһ�����������ֵ
        for (int i = 0; i < trainUnitCount; i++)
        {
            unitInfoIcons[i].gameObject.SetActive(true);
            Image image = unitInfoIcons[i].GetComponentsInChildren<Image>()[1];
            image.color = train.unitColors[i];
            Text text = unitInfoIcons[i].GetComponentInChildren<Text>();
            text.text = train.unitMaxHealths[i].ToString();
        }

        //���ݳ��������������½ǳ�����Ϣ�������
        for (int i = trainUnitCount; i <= 5; i++)
        {
            unitInfoIcons[i].gameObject.SetActive(false);
        }

        endPanel.gameObject.SetActive(false);

        StartCoroutine(ShowStartHint());
    }

    private void Update()
    {
        //������Ϸ��ʼʱִ��һ��
        //if (isStart)
        //{
            
        //    isStart = false;
        //}
    }

    /// <summary>
    /// ��ʾ��ͷ��321����ʱ�ı�
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

        levelText.text = "�ؿ� : " + GameManager.currentLevel;
        levelText.gameObject.SetActive(true);
        waveText.gameObject.SetActive(true);
    }

    /// <summary>
    /// 321�ĵ���ʱ�ı���Ҫ�ɴ�С�ı仯����
    /// </summary>
    /// <param name="time">�Ӵ��СҪ���</param>
    /// <param name="beginSize">�Ӷ��ʼ��</param>
    /// <param name="endSize">���ԭ�����ֺ�</param>
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
    /// ��������ֵ�ı���ֵ
    /// </summary>
    /// <param name="index">����</param>
    /// <param name="currentHealth">���ᵱǰ����ֵ</param>
    public void ChangeUnitHealthInfo(int index, int currentHealth)
    {
        unitInfoIcons[index].GetComponentInChildren<Text>().text = currentHealth.ToString();
    }

    /// <summary>
    /// ����������ʾ
    /// </summary>
    /// <param name="currentWave">��ǰ����</param>
    /// <param name="totalWave">�ܲ���</param>
    public void SetWaveText(int currentWave, int totalWave)
    {
        currentWave++;
        waveText.text = "���� : " + currentWave + " / " + totalWave;
    }

    /// <summary>
    /// ��Ϸ��������ʾ��Ϣ
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
