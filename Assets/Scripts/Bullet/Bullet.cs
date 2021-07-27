using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField, Range(0, 20f)]
    private float speed = 10f;

    [SerializeField, Range(0f, 10f)]
    private float damage = 5f;

    private Vector3 forward;

    [SerializeField]
    private LayerMask hitLayer;

    public void SetInitialInfo(Vector3 forward)
    {
        this.forward = forward.normalized;
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, forward, out RaycastHit hitInfo, speed * 1.5f * Time.deltaTime, hitLayer))
        {
            Enemy enemy = hitInfo.transform.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage, transform.position);
            }
            Destroy(gameObject);   
        }

        transform.Translate(speed * Time.deltaTime * forward, Space.World);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if(enemy != null)
        {
            enemy.TakeDamage(damage, transform.position);
        }
        Destroy(gameObject);
    }
}
