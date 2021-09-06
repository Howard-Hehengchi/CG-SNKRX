using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField, Range(0, 30f), Tooltip("子弹飞行速度")]
    private float speed = 10f;

    [SerializeField, Range(0f, 10f), Tooltip("子弹命中时造成的伤害")]
    private float damage = 5f;

    [Tooltip("子弹前进的方向")]
    private Vector3 forward;

    [SerializeField, Tooltip("子弹能击中的层级")]
    private LayerMask hitLayer;

    private float lifeTime = 6f;
    private float lifeTimer = 0f;

    public void SetInitialInfo(Vector3 forward)
    {
        this.forward = forward.normalized;
    }

    private void FixedUpdate()
    {
        lifeTimer += Time.deltaTime;
        if(lifeTimer >= lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        //进行射线检测，以防速度过快刚体没有检测结果
        if (Physics.Raycast(transform.position, forward, out RaycastHit hitInfo, speed * 1.5f * Time.deltaTime, hitLayer))
        {
            Enemy enemy = hitInfo.transform.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage, transform.position);
            }

            //只要撞到东西都销毁自己
            Destroy(gameObject);   
        }

        transform.Translate(speed * Time.deltaTime * forward, Space.World);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //进行碰撞检测，以防子弹擦过敌人而不受射线检测
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if(enemy != null)
        {
            enemy.TakeDamage(damage, transform.position);
        }
        Destroy(gameObject);
    }
}
