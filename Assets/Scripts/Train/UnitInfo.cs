using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfo : MonoBehaviour
{
    private int health;

    private System.Action<Transform> onDeath;
    private System.Action<UnitInfo, int> onHurt;

    /// <summary>
    /// 初始化时设置车厢生命值
    /// </summary>
    /// <param name="maxHealth">车厢最大生命值</param>
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

        //死亡的时候通知列车管理器和跟踪此车厢的敌人，隐藏当前车厢
        if(health <= 0)
        {
            onDeath(transform);
            gameObject.SetActive(false);
        }

        //受伤时通知UI改变生命值显示
        onHurt(this, health);
    }
}
