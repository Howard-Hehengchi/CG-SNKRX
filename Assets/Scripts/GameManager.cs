using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Tooltip("���ؿ���ʼ������Э����ʼ321����ʱ����Ϸ��ʼ���ж�"), HideInInspector]
    public bool roundStart = false;

    public void GameEnd()
    {

    }
}
