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

    [SerializeField, Tooltip("ÿһ���ж��ٲ������ÿһ�ֽ��е�������")]
    private List<int> waveInRound;

    [SerializeField, Tooltip("һ���ڿ���ѡ������������")]
    private EnemySpawnerGroup[] spawnerGroups;

    private int currentLevel;


    [Tooltip("�ܹ���������Ĳ���")]
    private int waveCount;

    [Tooltip("��ǰ��������0��ʼ����")]
    private int currentWave = 0;

    /*
    [Tooltip("ÿһ����������Щ���������ɶ��ٹ���"), SerializeField]
    private EnemySpawnerWaveSet[] spawnerSets;

    */

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
            GameManager.Instance.RoundFinish(true);
            return;
        }

        UIManager.Instance.SetWaveText(currentWave, waveCount);

        //ѡ����ǰ��������Щ���������ɵ���
        EnemySpawnerGroup spawnerGroup = spawnerGroups[Random.Range(0, spawnerGroups.Length)];
        spawnerCount = spawnerGroup.spawners.Count;
        clearSpawnerCount = 0;
        //����ƽ��ÿ��������Ҫ���ɶ��ٵ���
        int averageCount = (currentLevel * 3) / spawnerCount;
        for(int i = 0; i < spawnerCount; i++)
        {
            //��ÿ�����������������������
            int spawnCount = averageCount + 
                (Random.Range(0, 10) > 4 ? 1 : -1) * Random.Range(0, currentLevel + 1);
            if(spawnCount <=0)
            {
                spawnCount = currentLevel;
            }

            spawnerGroup.spawners[i].SetOnEnemyClear(OnSpawnerClear);
            spawnerGroup.spawners[i].Spawn(spawnCount);
        }

        //�����޸Ĳ�����Ϣ���´ξ�ֱ������
        currentWave++;

        /*
        //��ȡ��ǰ�����ĵ���������Ϣ
        EnemySpawnerWaveSet currentSet = spawnerSets[currentWave];
        spawnerCount = currentSet.spawnerSet.Count;
        clearSpawnerCount = 0;
        //��ÿ�������������������
        for(int i = 0;i < spawnerCount; i++)
        {
            EnemySpawnerInfo currentSpawnerInfo = currentSet.spawnerSet[i];
            currentSpawnerInfo.spawner.SetOnEnemyClear(OnSpawnerClear);
            currentSpawnerInfo.spawner.Spawn(currentSpawnerInfo.spawnCount);
        }

        //�����޸Ĳ�����Ϣ���´ξ�ֱ������
        currentWave++;
        */
    }

    /// <summary>
    /// ĳ���������ĵ��˶�������ˣ�������
    /// ���ȫ����գ�������һ������
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
/// һ�������ڿ���ѡ������������
/// </summary>
[System.Serializable]
public class EnemySpawnerGroup
{
    public List<EnemySpawner> spawners;
}

/*
/// <summary>
/// һ�������е�����������Ϣ����
/// </summary>
[System.Serializable]
public class EnemySpawnerWaveSet
{
    [HideInInspector, Tooltip("�����޸�Inspector������б�Ԫ�����ƣ�������")]
    public string name = "Spawner settings in this wave";
    [Tooltip("��ǰ���ε����е�����������Ϣ")]
    public List<EnemySpawnerInfo> spawnerSet;
}

/// <summary>
/// ������������������Ϣ��������������
/// </summary>
[System.Serializable]
public class EnemySpawnerInfo
{
    [HideInInspector, Tooltip("�����޸�Inspector������б�Ԫ�����ƣ�������")]
    public string name = "Settings for this spawner";
    [Tooltip("������")]
    public EnemySpawner spawner;
    [Tooltip("������������")]
    public int spawnCount;
}
*/