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

    [SerializeField, Tooltip("每一局有多少波怪物，对每一局进行单独配置")]
    private List<int> waveInRound;

    [SerializeField, Tooltip("一波内可以选择的生成器组合")]
    private EnemySpawnerGroup[] spawnerGroups;

    private int currentLevel;


    [Tooltip("总共怪物进攻的波数")]
    private int waveCount;

    [Tooltip("当前波数，从0开始计数")]
    private int currentWave = 0;

    /*
    [Tooltip("每一波怪物由哪些生成器生成多少怪物"), SerializeField]
    private EnemySpawnerWaveSet[] spawnerSets;

    */

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

    private void Start()
    {
        RoundStart();
    }

    /*
    public void RoundStart()
    {
        currentWave = 0;
        StartCoroutine(StartFirstSpawn());
    }
    */

    public void RoundStart()
    {
        currentLevel = GameManager.currentLevel;
        waveCount = waveInRound[currentLevel-1];
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
            GameManager.Instance.RoundFinish(true);
            return;
        }

        UIManager.Instance.SetWaveText(currentWave, waveCount);

        //选定当前波数用哪些生成器生成敌人
        EnemySpawnerGroup spawnerGroup = spawnerGroups[Random.Range(0, spawnerGroups.Length)];
        spawnerCount = spawnerGroup.spawners.Count;
        clearSpawnerCount = 0;
        //计算平均每个生成器要生成多少敌人
        int averageCount = (currentLevel * 3) / spawnerCount;
        for(int i = 0; i < spawnerCount; i++)
        {
            //对每个生成器的量进行随机浮动
            int spawnCount = averageCount + 
                (Random.Range(0, 10) > 4 ? 1 : -1) * Random.Range(0, currentLevel + 1);
            if(spawnCount <=0)
            {
                spawnCount = currentLevel;
            }

            spawnerGroup.spawners[i].SetOnEnemyClear(OnSpawnerClear);
            spawnerGroup.spawners[i].Spawn(spawnCount);
        }

        //这里修改波数信息，下次就直接生成
        currentWave++;

        /*
        //获取当前波数的敌人生成信息
        EnemySpawnerWaveSet currentSet = spawnerSets[currentWave];
        spawnerCount = currentSet.spawnerSet.Count;
        clearSpawnerCount = 0;
        //对每个生成器进行逐个处理
        for(int i = 0;i < spawnerCount; i++)
        {
            EnemySpawnerInfo currentSpawnerInfo = currentSet.spawnerSet[i];
            currentSpawnerInfo.spawner.SetOnEnemyClear(OnSpawnerClear);
            currentSpawnerInfo.spawner.Spawn(currentSpawnerInfo.spawnCount);
        }

        //这里修改波数信息，下次就直接生成
        currentWave++;
        */
    }

    /// <summary>
    /// 某个生成器的敌人都被清空了，计数；
    /// 如果全场清空，生成下一波敌人
    /// </summary>
    private void OnSpawnerClear()
    {
        clearSpawnerCount++;
        if(clearSpawnerCount == spawnerCount)
        {
            SpawnEnemies();
        }
    }
}

/// <summary>
/// 一个波次内可以选择的生成器组合
/// </summary>
[System.Serializable]
public class EnemySpawnerGroup
{
    public List<EnemySpawner> spawners;
}

/*
/// <summary>
/// 一波内所有敌人生成器信息集合
/// </summary>
[System.Serializable]
public class EnemySpawnerWaveSet
{
    [HideInInspector, Tooltip("仅供修改Inspector面板中列表元素名称，无意义")]
    public string name = "Spawner settings in this wave";
    [Tooltip("当前波次的所有敌人生成器信息")]
    public List<EnemySpawnerInfo> spawnerSet;
}

/// <summary>
/// 单个敌人生成器的信息，包括生成数量
/// </summary>
[System.Serializable]
public class EnemySpawnerInfo
{
    [HideInInspector, Tooltip("仅供修改Inspector面板中列表元素名称，无意义")]
    public string name = "Settings for this spawner";
    [Tooltip("生成器")]
    public EnemySpawner spawner;
    [Tooltip("本次生成数量")]
    public int spawnCount;
}
*/