using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //[Tooltip("���ڻ�ȡ�г���Ϣ")]
    //private TrainManager train;

    private Color color = Color.red;

    [Tooltip("�����������ֵ"), SerializeField]
    private float maxHealth = 5f;
    private float health;

    [Tooltip("�����ƶ��ٶ�"), SerializeField]
    private float speed = 6f;

    [SerializeField, Range(0f, 3f)]
    private float waitTime = 2f;
    private float waitTimer = 0f;
    private bool canMove = true;
    private bool adjustVelocity = true;

    [Tooltip("����Ŀ�공�������")]
    private int followIndex;

    [Tooltip("���ڿ����ƶ�")]
    private Rigidbody body;
    [Tooltip("�����ܻ�ʱ�ı���ɫ")]
    private MeshRenderer meshRenderer;

    private void OnEnable()
    {
        body = GetComponent<Rigidbody>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        meshRenderer.material.color = color;
        health = maxHealth;

        //train = TrainManager.Instance;

        //�����ȡһ����������
        followIndex = TrainManager.Instance.GetRandomIndexOfUnits();
        TrainManager.Instance.GetUnitByIndex(followIndex).AddOnDeath(ChangeFollowIndex);
        //followIndex = Random.Range(0, train.unitCount);
    }

    private void FixedUpdate()
    {
        if (GameManager.roundEnd)
        {
            return;
        }

        if (canMove)
        {
            Vector3 destination = TrainManager.Instance.GetTrainUnitPos(followIndex);
            if (destination == Vector3.up)
            {
                ChangeFollowIndex();
                destination = TrainManager.Instance.GetTrainUnitPos(followIndex);
            }
            Vector3 translation = (destination - transform.position).normalized;


            if (!adjustVelocity)
            {
                float velMag = body.velocity.magnitude;
                //if(Vector3.Dot(body.velocity, translation) > 0f && velMag >= 4f && velMag <= 10f)
                //{
                //    adjustVelocity = true;
                //}
                //else
                //{
                body.velocity *= 0.912f;
                if (body.velocity.magnitude <= 1f)
                {
                    //float angle = Vector3.Angle(transform.forward, translation);
                    //if (angle > 10f)
                    //{
                    //    StartCoroutine(SmoothRotate(0.5f, -angle));
                    //}
                    //else
                    //{
                    adjustVelocity = true;
                    //}
                }
                //}
            }
            else
            {
                //����TransformӲ�ƶ���δ���ø���
                //TODO:�����������ƶ�
                transform.LookAt(destination);
                translation = speed * Time.deltaTime * translation.normalized;
                //transform.Translate(translation);
                transform.Translate(Vector3.forward * speed * Time.deltaTime);

                //transform.LookAt(destination);

                //Vector3 vector = destination - transform.position;
                //float percent = speed * Time.deltaTime / vector.magnitude;
                //Vector3 actualDestination = Vector3.Lerp(transform.position, destination, percent);
                //body.MovePosition(actualDestination);
            }
        }
        else
        {
            waitTimer += Time.deltaTime;
            if (waitTimer > waitTime)
            {
                waitTimer = 0f;
                canMove = true;
                adjustVelocity = false;
            }
        }
    }

    //private IEnumerator SmoothRotate(float time, float angle)
    //{
    //    for(float t = Time.fixedDeltaTime/time; t <= 1f; t+= Time.fixedDeltaTime/time)
    //    {
    //        transform.Rotate(Vector3.up, angle * Time.fixedDeltaTime/ time);
    //        yield return null;
    //    }
    //    adjustVelocity = true;
    //}

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        //ײ���г�
        if (other.CompareTag("Player"))
        {
            canMove = false;

            //�����˴�������
            Vector3 normal = collision.GetContact(0).normal;
            body.AddForce(normal * 14f, ForceMode.Impulse);

            //����˺�
            Unit unit = other.GetComponent<Unit>();
            unit.TakeDamage(1);
            
            //�����Լ�Ҳ�ܵ��˺�
            TakeDamage(0.5f, other.transform.position);
        }
    }

    private System.Action<Enemy> onDeath;
    public void AddOnDeath(System.Action<Enemy> onDeath)
    {
        this.onDeath += onDeath;
    }

    public void RemoveOnDeath(System.Action<Enemy> onDeath)
    {
        this.onDeath -= onDeath;
    }

    public void TakeDamage(float amount, Vector3 bulletPosition)
    {
        health -= amount;
        if (health <= 0f)
        {
            onDeath?.Invoke(this);

            //Ϊ�˷�ֹ����������ɶ���жϣ��ȴ�������֡������������
            //StartCoroutine(Die());
            Destroy(gameObject);
            return;
        }

        StartCoroutine(ChangeColor());
        Vector3 dir = (transform.position - bulletPosition).ProjectedOnPlane();
        dir.Normalize();
        body.AddForce(dir * 1f, ForceMode.Impulse);
    } 

    //private IEnumerator Die()
    //{
    //    yield return new WaitForFixedUpdate();
    //    Destroy(gameObject);
    //}

    private IEnumerator ChangeColor()
    {
        meshRenderer.material.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        meshRenderer.material.color = color;
    }

    /// <summary>
    /// �ڳ������������Ŀ��
    /// </summary>
    /// <param name="deadUnitTF">ûɶ�ã����ղ�����</param>
    private void ChangeFollowIndex(Transform deadUnitTF = null)
    {
        followIndex = TrainManager.Instance.GetRandomIndexOfUnits();
    }
}
