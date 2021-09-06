using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("�ⲿ����")]
    //[Tooltip("Ϊ�˸����ɵĵ����г�������"), SerializeField]
    //private TrainManager train;

    [Tooltip("����Ԥ����"), SerializeField]
    private Enemy enemyPrefab;

    [Tooltip("���˳���ǰ�ľ�ʾ��ʶ")]
    private GameObject sign;
    [Header("����ǰ��ʾ")]
    [Tooltip("��ʾ��ʶ��˸����"), SerializeField, Range(0, 10)]
    private int flashCount = 3;
    [Tooltip("��ʾ��ʶ����ʱ��"), SerializeField, Range(0f, 1f)]
    private float flashOnTime = 0.5f;
    [Tooltip("��ʾ��ʶ���ʱ��"), SerializeField, Range(0f, 1f)]
    private float flashOffTime = 0.3f;

    [Header("��������")]
    //[Tooltip("һ�����ɵ��˵�������������"), SerializeField, Range(1, 6)]
    //private int enemySpawnCount = 2;

    [Tooltip("���ɵ���֮��ļ��ʱ��"), SerializeField, Range(0f, 2f)]
    private float spawnInterval = 1f;

    [Tooltip("���ɵ����е��˵�����")]
    private List<Enemy> enemies;
    private List<Enemy> deadEnemies;

    [Tooltip("�������ɵĵ�������")]
    private int spawnCount;
    [Tooltip("�������ɵĵ���������������")]
    private int deadCount;

    private void Start()
    {
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

    [Tooltip("���������ĵ��˱���պ�֪ͨ������")]
    private System.Action onEnemyClear;
    public void SetOnEnemyClear(System.Action onEnemyClear)
    {
        this.onEnemyClear = onEnemyClear;
    }

    /// <summary>
    /// ����ָ�������ĵ���
    /// </summary>
    /// <param name="enemyCount"></param>
    public void Spawn(int enemyCount)
    {
        spawnCount = enemyCount;
        enemies = new List<Enemy>();
        deadCount = 0;
        deadEnemies = new List<Enemy>();
        StartCoroutine(SpawnEnemies(enemyCount));
    }

    /// <summary>
    /// ��ָ���������ָ�������ĵ��ˣ�����ʼ��
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnEnemies(int enemyCount)
    {
        int x = (int)(1f / flashOnTime);
        float p = x * flashOffTime;

        //for(int i = 0; i < flashCount; i++)
        //{
        //    sign.SetActive(true);
        //    yield return new WaitForSeconds(flashOnTime);
        //    sign.SetActive(false);
        //    yield return new WaitForSeconds(flashOffTime);
        //}
        for(int i = x; i < flashCount + x; i++)
        {
            sign.SetActive(true);
            yield return new WaitForSeconds(1f / i);
            sign.SetActive(false);
            yield return new WaitForSeconds(p / i);
        }


        for(int i = 0; i < enemyCount; i++)
        {
            Enemy enemy = Instantiate(enemyPrefab, transform);
            //enemy.SetTrainManager(train);
            enemy.AddOnDeath(OnEnemyDie);
            enemies.Add(enemy);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void OnEnemyDie(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
            if (!deadEnemies.Contains(enemy))
            {
                deadEnemies.Add(enemy);
                deadCount++;
                if (deadCount == spawnCount)
                {
                    onEnemyClear();
                }
            }
        }
    }
}
