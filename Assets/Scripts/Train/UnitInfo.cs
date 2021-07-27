using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfo : MonoBehaviour
{
    private int health;

    private System.Action<Transform> onDeath;
    private System.Action<UnitInfo, int> onHurt;

    /// <summary>
    /// ��ʼ��ʱ���ó�������ֵ
    /// </summary>
    /// <param name="maxHealth">�����������ֵ</param>
    public void SetHealth(int maxHealth)
    {
        health = maxHealth;
    }

    public void SetOnDeath(System.Action<Transform> onDeath)
    {
        this.onDeath += onDeath;
    }

    public void SetOnHurt(System.Action<UnitInfo, int> onHurt)
    {
        this.onHurt += onHurt;
    }

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
}
