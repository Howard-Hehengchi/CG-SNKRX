using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField, Range(0, 30f), Tooltip("�ӵ������ٶ�")]
    private float speed = 10f;

    [SerializeField, Range(0f, 10f), Tooltip("�ӵ�����ʱ��ɵ��˺�")]
    private float damage = 5f;

    [Tooltip("�ӵ�ǰ���ķ���")]
    private Vector3 forward;

    [SerializeField, Tooltip("�ӵ��ܻ��еĲ㼶")]
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

        //�������߼�⣬�Է��ٶȹ������û�м����
        if (Physics.Raycast(transform.position, forward, out RaycastHit hitInfo, speed * 1.5f * Time.deltaTime, hitLayer))
        {
            Enemy enemy = hitInfo.transform.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage, transform.position);
            }

            //ֻҪײ�������������Լ�
            Destroy(gameObject);   
        }

        transform.Translate(speed * Time.deltaTime * forward, Space.World);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //������ײ��⣬�Է��ӵ��������˶��������߼��
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if(enemy != null)
        {
            enemy.TakeDamage(damage, transform.position);
        }
        Destroy(gameObject);
    }
}
