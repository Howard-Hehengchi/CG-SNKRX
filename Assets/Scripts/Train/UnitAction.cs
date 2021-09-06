using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������Ĺ���
/// </summary>
public class UnitAction : MonoBehaviour
{
    [Tooltip("���ṥ����Ŀ�꣬Ĭ��û��")]
    private Transform attackTarget = null;

    [Tooltip("����������ӵ�Ԥ����")]
    private Bullet bulletPrefab;

    [Tooltip("������Χ������trigger���")]
    private SphereCollider attackDetector;
    [Tooltip("������Χ�ڵ����е���")]
    private List<Enemy> accessibleEnemies;

    [Tooltip("���ṥ���ļ��"), Range(0.1f, 2f)]
    private float attackInterval = 1f;
    [Tooltip("���ṥ����ʱ��")]
    private float attackTimer = 0f;

    [Tooltip("������Χ��С������������Ӧ����ʵ������"), Range(5f, 20f)]
    private float attackRange = 10f;

    private void Start()
    {
        Unit unit = GetComponent<Unit>();
        bulletPrefab = unit.GetBulletPrefab();
        attackInterval = unit.GetAttackInterval();
        attackRange = unit.GetAttackRange();

        //��ֵ������Χ
        attackDetector = GetComponent<SphereCollider>();
        attackDetector.radius = attackRange;

        accessibleEnemies = new List<Enemy>();
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;

        //���������Χ���е���
        if (accessibleEnemies.Count > 0)
        {
            //������˹�����ʱ��
            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;

                //���û�й���Ŀ�꣬�ҵ���ǰ������Χ�������һ������
                if (attackTarget == null)
                {
                    float minDistance = float.MaxValue;
                    int minIndex = 0;
                    for (int i = 0; i < accessibleEnemies.Count; i++)
                    {
                        if (accessibleEnemies[i] == null)
                        {
                            continue;
                        }
                        float distance = (accessibleEnemies[i].transform.position - transform.position).sqrMagnitude;
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            minIndex = i;
                        }
                    }

                    if (accessibleEnemies[minIndex] == null)
                    {
                        attackTarget = null;
                    }
                    else
                    {
                        attackTarget = accessibleEnemies[minIndex].transform;
                    }
                }

                //�Թ���Ŀ����й���
                //���ǵ����ж��ڼ���ܵ����Ѿ�������ֻ���ڹ���Ŀ����ʱ���й���
                if(attackTarget != null)
                {
                    //�ӳ�����׼���ˣ����㷽������ǰ���н�
                    Vector3 attackDir = attackTarget.position - transform.position;
                    attackDir.Normalize();
                    float angle = Vector3.Angle(Vector3.forward, attackDir);

                    //�����ӵ�����ת����������������Ҫ����ת�����·�ת
                    Vector3 axis = Vector3.Cross(Vector3.forward, attackDir).y < 0 ? Vector3.down : Vector3.up;
                    Quaternion rotation = Quaternion.AngleAxis(angle, axis);

                    //�����ӵ�
                    Bullet bullet = Instantiate(bulletPrefab, transform.position, rotation);
                    bullet.SetInitialInfo(attackDir);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //���˽��빥����Χ���Ǽ�
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            accessibleEnemies.Add(enemy);
            enemy.AddOnDeath(RemoveEnemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //�����뿪������Χ��ȡ���Ǽ�
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.RemoveOnDeath(RemoveEnemy);
            if (attackTarget == enemy.transform)
            {
                attackTarget = null;
            }
            accessibleEnemies.Remove(enemy);
        }
    }

    /// <summary>
    /// ����������ʱ�������˴ӳ���ĵ����б���ȥ��
    /// </summary>
    /// <param name="enemy"></param>
    private void RemoveEnemy(Enemy enemy)
    {
        if (accessibleEnemies.Contains(enemy))
        {
            if (attackTarget == enemy.transform)
            {
                attackTarget = null;
            }
            accessibleEnemies.Remove(enemy);
        }
    }
}
