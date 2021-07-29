using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Tooltip("���ڻ�ȡ�г���Ϣ")]
    private TrainManager train;

    [Tooltip("�����������ֵ"), SerializeField]
    private float maxHealth = 5f;
    private float health;

    [Tooltip("�����ƶ��ٶ�"), SerializeField]
    private float speed = 6f;

    //[SerializeField, Range(0f, 3f)]
    //private float waitTime = 3f;
    //private float waitTimer = 0f;
    //private bool canMove = true;

    [Tooltip("����Ŀ�공�������")]
    private int followIndex;

    private Rigidbody body;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        health = maxHealth;
    }

    public void SetTrainManager(TrainManager train)
    {
        this.train = train;

        //�����ȡһ����������
        followIndex = train.GetRandomIndexOfUnits();
        train.GetUnitByIndex(followIndex).SetOnDeath(ChangeFollowIndex);
        //followIndex = Random.Range(0, train.unitCount);
    }

    private void FixedUpdate()
    {
        if (GameManager.roundEnd)
        {
            return;
        }

        //if (canMove)
        //{
        Vector3 destination = train.GetTrainUnitPos(followIndex);
        if (destination == Vector3.up)
        {
            ChangeFollowIndex();
            destination = train.GetTrainUnitPos(followIndex);
        }

        //����TransformӲ�ƶ���δ���ø���
        //TODO:�����������ƶ���ʹ�ø���
        Vector3 translation = destination - transform.position;
        translation = speed * Time.deltaTime * translation.normalized;
        transform.Translate(translation);

        //transform.LookAt(destination);

        //Vector3 vector = destination - transform.position;
        //float percent = speed * Time.deltaTime / vector.magnitude;
        //Vector3 actualDestination = Vector3.Lerp(transform.position, destination, percent);

        //body.MovePosition(actualDestination);
        //}
        //else
        //{
        //    waitTimer += Time.deltaTime;
        //    if(waitTimer > waitTime)
        //    {
        //        waitTimer = 0f;
        //        canMove = true;
        //    }
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        //ײ���г�
        if (other.CompareTag("Player"))
        {
            //�����˴�������
            Vector3 normal = collision.GetContact(0).normal;
            body.AddForce(normal * 7f, ForceMode.Impulse);

            //����˺�
            UnitInfo info = other.GetComponent<UnitInfo>();
            info.TakeDamage(1);
            //canMove = false;
        }
    }

    private System.Action<Enemy> onDeath;
    public void SetOnDeath(System.Action<Enemy> onDeath)
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
            StartCoroutine(Die());
        }

        StartCoroutine(ChangeColor());
        Vector3 dir = (transform.position - bulletPosition).ProjectedOnPlane();
        dir.Normalize();
        body.AddForce(dir * 1f, ForceMode.Impulse);
    } 

    private IEnumerator Die()
    {
        yield return new WaitForFixedUpdate();
        Destroy(gameObject);
    }

    private IEnumerator ChangeColor()
    {
        Color originalColor = meshRenderer.material.color;
        meshRenderer.material.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        meshRenderer.material.color = originalColor;
    }

    /// <summary>
    /// �ڳ������������Ŀ��
    /// </summary>
    /// <param name="deadUnitTF">ûɶ�ã����ղ�����</param>
    private void ChangeFollowIndex(Transform deadUnitTF = null)
    {
        followIndex = train.GetRandomIndexOfUnits();
    }
}
