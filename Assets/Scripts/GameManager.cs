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

    public void GameStart()
    {
        Time.timeScale = 1f;
        roundStart = false;
        if(SceneManager.GetActiveScene().name != "TestScene")
        {
            SceneManager.LoadScene("TestScene");
        }
        //WaveManager.Instance?.RoundStart();
        //TrainManager.Instance?.RoundStart();
        //UIManager.Instance?.RoundStart();
    }

    [Tooltip("���ؿ���ʼ������Э����ʼ321����ʱ����Ϸ��ʼ���ж�"), HideInInspector]
    public static bool roundStart = false;

    [Tooltip("���ؿ�����������֪ͨ���ű�"), HideInInspector]
    public static bool roundEnd = false;

    public void GameEnd(bool success)
    {
        Time.timeScale = 0.1f;
        roundEnd = true;
        UIManager.Instance.ShowEndText(success);
    }

    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ReturnToStartMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }
}
