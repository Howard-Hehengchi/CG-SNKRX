using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 负责车厢的攻击
/// </summary>
public class UnitAction : MonoBehaviour
{
    [Tooltip("车厢攻击的目标，默认没有")]
    private Transform attackTarget = null;

    [Tooltip("用来发射的子弹预制体")]
    private Bullet bulletPrefab;

    [Tooltip("攻击范围，采用trigger检测")]
    private SphereCollider attackDetector;
    [Tooltip("攻击范围内的所有敌人")]
    private List<Enemy> accessibleEnemies;

    [Tooltip("车厢攻击的间隔"), Range(0.1f, 2f)]
    private float attackInterval = 1f;
    [Tooltip("车厢攻击计时器")]
    private float attackTimer = 0f;

    [Tooltip("攻击范围大小，在面板调整并应用于实际运行"), Range(5f, 20f)]
    private float attackRange = 10f;

    private void Start()
    {
        Unit unit = GetComponent<Unit>();
        bulletPrefab = unit.GetBulletPrefab();
        attackInterval = unit.GetAttackInterval();
        attackRange = unit.GetAttackRange();

        //赋值攻击范围
        attackDetector = GetComponent<SphereCollider>();
        attackDetector.radius = attackRange;

        accessibleEnemies = new List<Enemy>();
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;

        //如果攻击范围内有敌人
        if (accessibleEnemies.Count > 0)
        {
            //如果到了攻击的时间
            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;

                //如果没有攻击目标，找到当前攻击范围内最近的一个敌人
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

                //对攻击目标进行攻击
                //考虑到在判断期间可能敌人已经死亡，只有在攻击目标存活时进行攻击
                if(attackTarget != null)
                {
                    //从车厢瞄准敌人，计算方向、与正前方夹角
                    Vector3 attackDir = attackTarget.position - transform.position;
                    attackDir.Normalize();
                    float angle = Vector3.Angle(Vector3.forward, attackDir);

                    //计算子弹的旋转，如果敌人在左边需要将旋转轴上下翻转
                    Vector3 axis = Vector3.Cross(Vector3.forward, attackDir).y < 0 ? Vector3.down : Vector3.up;
                    Quaternion rotation = Quaternion.AngleAxis(angle, axis);

                    //发射子弹
                    Bullet bullet = Instantiate(bulletPrefab, transform.position, rotation);
                    bullet.SetInitialInfo(attackDir);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //敌人进入攻击范围，登记
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            accessibleEnemies.Add(enemy);
            enemy.AddOnDeath(RemoveEnemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //敌人离开攻击范围，取消登记
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
    /// 当敌人死亡时，将敌人从车厢的敌人列表中去掉
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
