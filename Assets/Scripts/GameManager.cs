using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }
    private static GameManager instance;

    public static int currentLevel = 0;
    private const int totalLevel = 5;

    public static List<UnitType> train;
    public static int maxUnit = 6;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    //private void Start()
    //{
    //    DontDestroyOnLoad(gameObject);
    //}

    /// <summary>
    /// ��Ӧ����ʼ��Ϸ����ť�Ĺ���
    /// </summary>
    public void RoundStart()
    {
        currentLevel++;

        Time.timeScale = 1f;
        roundStart = false;
        roundEnd = false;
        if(SceneManager.GetActiveScene().name != "MainScene")
        {
            SceneManager.LoadScene("MainScene");
        }
        //WaveManager.Instance?.RoundStart();
        //TrainManager.Instance?.RoundStart();
        //UIManager.Instance?.RoundStart();
    }

    [Tooltip("���ؿ���ʼ������Э����ʼ321����ʱ����Ϸ��ʼ���ж�"), HideInInspector]
    public static bool roundStart = false;

    [Tooltip("���ؿ�����������֪ͨ���ű�"), HideInInspector]
    public static bool roundEnd = false;

    /// <summary>
    /// ��Ϸ����ʱ����
    /// </summary>
    /// <param name="success">���ʤ���˻���ʧ����</param>
    public void RoundFinish(bool success)
    {
        //������
        //Time.timeScale = 0.3f;

        roundEnd = true;
        if (currentLevel == totalLevel)
        {
            UIManager.Instance.ShowGameEndText();
            StartCoroutine(EndGame());
        }
        else
        {
            UIManager.Instance.ShowEndText(success);
            if (success)
            {
                StartCoroutine(SwitchToShopScene());
            }
            else
            {
                StartCoroutine(EndGame());
            }
        }        
    }

    private IEnumerator SwitchToShopScene()
    {
        yield return new WaitForSeconds(2.3f);
        //Time.timeScale = 1f;
        SceneManager.LoadScene("Shop");
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(3f);
        GameExit();
    }

    /// <summary>
    /// ��Ӧ���˳���Ϸ����ť
    /// </summary>
    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// ��Ӧ���������˵�����ť
    /// </summary>
    public void ReturnToStartMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public static void MoveUnit(int fromIndex, int toIndex)
    {
        if(fromIndex == toIndex)
        {
            return;
        }

        UnitType type = train[fromIndex];
        train.RemoveAt(fromIndex);
        train.Insert(toIndex, type);
    }

    public static void RemoveUnit(int index)
    {
        if (index >= 0 && index < train.Count)
        {
            train.RemoveAt(index);
        }
    }

    public static bool AddUnit(UnitType type)
    {
        if(train.Count == maxUnit -1)
        {
            return false;
        }

        train.Add(type);
        return true;
    }
}
