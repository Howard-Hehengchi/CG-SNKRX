using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("外部引用")]
    [Tooltip("为了给生成的敌人列车的引用"), SerializeField]
    private TrainManager train;

    [Tooltip("敌人预制体"), SerializeField]
    private Enemy enemyPrefab;

    [Tooltip("敌人出现前的警示标识")]
    private GameObject sign;
    [Header("生成前警示")]
    [Tooltip("警示标识闪烁几次"), SerializeField, Range(0, 5)]
    private int flashCount = 3;
    [Tooltip("警示标识亮的时长"), SerializeField, Range(0f, 1f)]
    private float flashOnTime = 0.5f;
    [Tooltip("警示标识灭的时长"), SerializeField, Range(0f, 1f)]
    private float flashOffTime = 0.3f;

    [Header("敌人生成")]
    //[Tooltip("一次生成敌人的数量，测试用"), SerializeField, Range(1, 6)]
    //private int enemySpawnCount = 2;

    [Tooltip("生成敌人之间的间隔时间"), SerializeField, Range(0f, 2f)]
    private float spawnInterval = 1f;

    [Tooltip("生成的所有敌人的引用")]
    private List<Enemy> enemies;

    [Tooltip("本次生成的敌人数量")]
    private int spawnCount;
    [Tooltip("本次生成的敌人中死亡的数量")]
    private int deadCount;

    private void Start()
    {
        enemies = new List<Enemy>();
        sign = transform.GetChild(0).gameObject;
        sign.SetActive(false);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    StartCoroutine(Spawn());
        //}    
    }

    [Tooltip("本生成器的敌人被清空后通知订阅者")]
    private System.Action onEnemyClear;
    public void SetOnEnemyClear(System.Action onEnemyClear)
    {
        this.onEnemyClear += onEnemyClear;
    }

    /// <summary>
    /// 生成指定数量的敌人
    /// </summary>
    /// <param name="enemyCount"></param>
    public void Spawn(int enemyCount)
    {
        spawnCount = enemyCount;
        deadCount = 0;
        StartCoroutine(SpawnEnemies(enemyCount));
    }

    /// <summary>
    /// 以指定间隔生成指定数量的敌人，并初始化
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnEnemies(int enemyCount)
    {
        for(int i = 0; i < flashCount; i++)
        {
            sign.SetActive(true);
            yield return new WaitForSeconds(flashOnTime);
            sign.SetActive(false);
            yield return new WaitForSeconds(flashOffTime);
        }


        for(int i = 0; i < enemyCount; i++)
        {
            Enemy enemy = Instantiate(enemyPrefab, transform);
            enemy.SetTrainManager(train);
            enemy.SetOnDeath(OnEnemyDie);
            enemies.Add(enemy);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void OnEnemyDie(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            deadCount++;
            if (deadCount == spawnCount)
            {
                onEnemyClear();
            }
        }
    }
}
