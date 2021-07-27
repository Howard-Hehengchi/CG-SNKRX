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

    [Tooltip("本关卡开始，用于协调开始321倒计时与游戏开始的判定"), HideInInspector]
    public bool roundStart = false;

    public void GameEnd()
    {

    }
}
