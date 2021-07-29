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

    [Tooltip("���½ǳ�����Ϣ��ʾ����ÿ��Ԫ����һ�����ᣨ��������ͼ������ֵ��ʾ��"), SerializeField]
    private Transform[] unitInfoIcons;

    [Tooltip("�����ı���ʾ���"), SerializeField]
    private EndPanel endPanel;

    [Header("��ʼ�ı���ʾ����")]
    [Tooltip("��ʼ�ı�ÿ����ʾ���"), SerializeField, Range(0f, 1f)]
    private float startHintTextShowTime = 1f;
    [Tooltip("��ʼ�ı�ÿ�����ض��"), SerializeField, Range(0f, 0.5f)]
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

        //��������ֵ�ı���ֵ
        //Ŀǰ����ͳһ�����������ֵ
        for (int i = 0; i < trainUnitCount; i++)
        {
            unitInfoIcons[i].gameObject.SetActive(true);
            Image image = unitInfoIcons[i].GetComponentsInChildren<Image>()[1];
            image.color = train.unitColors[i];
            Text text = unitInfoIcons[i].GetComponentInChildren<Text>();
            text.text = train.unitMaxHealth.ToString();
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
    /// ��������ֵ�ı���ֵ
    /// </summary>
    /// <param name="index">����</param>
    /// <param name="currentHealth">���ᵱǰ����ֵ</param>
    public void ChangeUnitHealthInfo(int index, int currentHealth)
    {
        unitInfoIcons[index].GetComponentInChildren<Text>().text = currentHealth.ToString();
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
}
