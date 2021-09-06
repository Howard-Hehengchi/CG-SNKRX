using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    Normal,
    Shooter,
    Shield
}

[RequireComponent(typeof(UnitMove), typeof(UnitAction))]
/// <summary>
/// 负责车厢的信息存储和外部引用管理
/// </summary>
public class Unit : MonoBehaviour
{
    [SerializeField, Tooltip("子弹预制体，后面使用对象池的时候替换成子弹工厂")]
    private Bullet bulletPrefab;

    [Tooltip("本车厢的类型，相当于ID一样的")]
    protected UnitType type;
    
    [HideInInspector, Tooltip("车厢最大生命值")]
    public int maxHealth;
    [Tooltip("车厢当前生命值")]
    protected int health;
    [HideInInspector, Tooltip("车厢颜色")]
    public Color color;

    [Tooltip("车厢攻击间隔")]
    protected float attackInterval;
    [Tooltip("车厢攻击范围")]
    protected float attackRange;

    private static Dictionary<UnitType, Color> typeColorPair = new Dictionary<UnitType, Color>
    {
        { UnitType.Normal, Color.gray },
        { UnitType.Shield, Color.red },
        { UnitType.Shooter, Color.cyan }
    };

    public Bullet GetBulletPrefab()
    {
        return bulletPrefab;
    }

    public float GetAttackInterval()
    {
        if(attackInterval <= 0f)
        {
            Debug.LogError("Zero Attack Interval! Please check initialization sequence.");
        }
        return attackInterval;
    }

    public float GetAttackRange()
    {
        if(attackRange <= 0f)
        {
            Debug.LogError("Zero Attack Range! Please check initialization sequence.");
        }
        return attackRange;
    }

    [Tooltip("车厢死亡时的回调函数")]
    private System.Action<Transform> onDeath;
    [Tooltip("车厢受伤时的回调函数")]
    private System.Action<Unit, int> onHurt;

    /// <summary>
    /// 车厢生成的时候将生命值设成最大生命值
    /// </summary>
    public virtual void Initialize()
    {
        health = maxHealth;
        color = typeColorPair[type];
    }

    public void AddOnDeath(System.Action<Transform> onDeath)
    {
        this.onDeath += onDeath;
    }

    public void AddOnHurt(System.Action<Unit, int> onHurt)
    {
        this.onHurt += onHurt;
    }

    /// <summary>
    /// 通知车厢扣血
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(int amount)
    {
        health -= amount;

        //死亡的时候通知列车管理器和跟踪此车厢的敌人，隐藏当前车厢
        if(health <= 0)
        {
            onDeath(transform);
            gameObject.SetActive(false);
        }

        //受伤时通知UI改变生命值显示
        onHurt(this, health);
    }

    public static Color GetColor(UnitType type)
    {
        return typeColorPair[type];
    }
}
