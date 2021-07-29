using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���˲�������
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

    [Tooltip("�ܹ���������Ĳ���"), SerializeField, Range(0, 7)]
    private int waveCount = 4;

    [Tooltip("��ǰ��������0��ʼ����")]
    private int currentWave = 0;

    [Tooltip("ÿһ����������Щ���������ɶ��ٹ���"), SerializeField]
    private EnemySpawnerWaveSet[] spawnerSets;

    [Tooltip("��ǰ���ε��õĵ�������������")]
    private int spawnerCount;
    [Tooltip("��ǰ�����Ѿ���յĵ�������������")]
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
    /// ��321����ʱ���������Ͽ�ʼ���ɵ���
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartFirstSpawn()
    {
        //�ȴ�321����ʱ�����ٿ�ʼ����
        while (!GameManager.roundStart)
        {
            yield return null;
        }

        SpawnEnemies();
    }

    /// <summary>
    /// ���ɵ�ǰ���εĵ���
    /// </summary>
    private void SpawnEnemies()
    {
        //ȫ��������ռ�Ϊ��Ϸ����
        if(currentWave >= waveCount)
        {
            //TODO:������н�������
            GameManager.Instance.GameEnd(true);
            return;
        }

        //��ȡ��ǰ�����ĵ���������Ϣ
        EnemySpawnerWaveSet currentSet = spawnerSets[currentWave];
        spawnerCount = currentSet.spawnerSet.Length;
        clearSpawnerCount = 0;
        //��ÿ�������������������
        for(int i = 0;i < spawnerCount; i++)
        {
            EnemySpawnerInfo currentSpawnerInfo = currentSet.spawnerSet[i];
            currentSpawnerInfo.spawner.SetOnEnemyClear(SpawnerClear);
            currentSpawnerInfo.spawner.Spawn(currentSpawnerInfo.spawnCount);
        }

        //�����޸Ĳ�����Ϣ���´ξ�ֱ������
        currentWave++;
    }

    /// <summary>
    /// ĳ���������ĵ��˶�������ˣ�������
    /// ���ȫ����գ�������һ������
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
/// һ�������е�����������Ϣ����
/// </summary>
[System.Serializable]
public class EnemySpawnerWaveSet
{
    [Tooltip("��ǰ���ε����е�����������Ϣ")]
    public EnemySpawnerInfo[] spawnerSet;
}

/// <summary>
/// ������������������Ϣ��������������
/// </summary>
[System.Serializable]
public class EnemySpawnerInfo
{
    [Tooltip("������")]
    public EnemySpawner spawner;
    [Tooltip("������������")]
    public int spawnCount;
}
