using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人波数管理
/// </summary>
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance
    {
        get
        {
            return instance;
        }
    }
    private static WaveManager instance;

    [Tooltip("总共怪物进攻的波数"), SerializeField, Range(0, 7)]
    private int waveCount = 4;

    [Tooltip("当前波数，从0开始计数")]
    private int currentWave = 0;

    [Tooltip("每一波怪物由哪些生成器生成多少怪物"), SerializeField]
    private EnemySpawnerWaveSet[] spawnerSets;

    [Tooltip("当前波次调用的敌人生成器数量")]
    private int spawnerCount;
    [Tooltip("当前波次已经清空的敌人生成器数量")]
    private int clearSpawnerCount = 0;

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
        currentWave = 0;
        StartCoroutine(StartFirstSpawn());
    }

    /// <summary>
    /// 在321倒计时结束后马上开始生成敌人
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartFirstSpawn()
    {
        //等待321倒计时结束再开始生成
        while (!GameManager.roundStart)
        {
            yield return null;
        }

        SpawnEnemies();
    }

    /// <summary>
    /// 生成当前波次的敌人
    /// </summary>
    private void SpawnEnemies()
    {
        //全部波数清空即为游戏结束
        if(currentWave >= waveCount)
        {
            //TODO:这里进行结束操作
            GameManager.Instance.GameEnd(true);
            return;
        }

        //获取当前波数的敌人生成信息
        EnemySpawnerWaveSet currentSet = spawnerSets[currentWave];
        spawnerCount = currentSet.spawnerSet.Length;
        clearSpawnerCount = 0;
        //对每个生成器进行逐个处理
        for(int i = 0;i < spawnerCount; i++)
        {
            EnemySpawnerInfo currentSpawnerInfo = currentSet.spawnerSet[i];
            currentSpawnerInfo.spawner.SetOnEnemyClear(SpawnerClear);
            currentSpawnerInfo.spawner.Spawn(currentSpawnerInfo.spawnCount);
        }

        //这里修改波数信息，下次就直接生成
        currentWave++;
    }

    /// <summary>
    /// 某个生成器的敌人都被清空了，计数；
    /// 如果全场清空，生成下一波敌人
    /// </summary>
    private void SpawnerClear()
    {
        clearSpawnerCount++;
        if(clearSpawnerCount == spawnerCount)
        {
            SpawnEnemies();
        }
    }
}

/// <summary>
/// 一波内所有敌人生成器信息集合
/// </summary>
[System.Serializable]
public class EnemySpawnerWaveSet
{
    [Tooltip("当前波次的所有敌人生成器信息")]
    public EnemySpawnerInfo[] spawnerSet;
}

/// <summary>
/// 单个敌人生成器的信息，包括生成数量
/// </summary>
[System.Serializable]
public class EnemySpawnerInfo
{
    [Tooltip("生成器")]
    public EnemySpawner spawner;
    [Tooltip("本次生成数量")]
    public int spawnCount;
}
