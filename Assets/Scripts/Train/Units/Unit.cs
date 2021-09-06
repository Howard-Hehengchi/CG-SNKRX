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
/// ���������Ϣ�洢���ⲿ���ù���
/// </summary>
public class Unit : MonoBehaviour
{
    [SerializeField, Tooltip("�ӵ�Ԥ���壬����ʹ�ö���ص�ʱ���滻���ӵ�����")]
    private Bullet bulletPrefab;

    [Tooltip("����������ͣ��൱��IDһ����")]
    protected UnitType type;
    
    [HideInInspector, Tooltip("�����������ֵ")]
    public int maxHealth;
    [Tooltip("���ᵱǰ����ֵ")]
    protected int health;
    [HideInInspector, Tooltip("������ɫ")]
    public Color color;

    [Tooltip("���ṥ�����")]
    protected float attackInterval;
    [Tooltip("���ṥ����Χ")]
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

    [Tooltip("��������ʱ�Ļص�����")]
    private System.Action<Transform> onDeath;
    [Tooltip("��������ʱ�Ļص�����")]
    private System.Action<Unit, int> onHurt;

    /// <summary>
    /// �������ɵ�ʱ������ֵ����������ֵ
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
    /// ֪ͨ�����Ѫ
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(int amount)
    {
        health -= amount;

        //������ʱ��֪ͨ�г��������͸��ٴ˳���ĵ��ˣ����ص�ǰ����
        if(health <= 0)
        {
            onDeath(transform);
            gameObject.SetActive(false);
        }

        //����ʱ֪ͨUI�ı�����ֵ��ʾ
        onHurt(this, health);
    }

    public static Color GetColor(UnitType type)
    {
        return typeColorPair[type];
    }
}
