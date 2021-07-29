using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Tooltip("用于获取列车信息")]
    private TrainManager train;

    [Tooltip("敌人最大生命值"), SerializeField]
    private float maxHealth = 5f;
    private float health;

    [Tooltip("敌人移动速度"), SerializeField]
    private float speed = 6f;

    //[SerializeField, Range(0f, 3f)]
    //private float waitTime = 3f;
    //private float waitTimer = 0f;
    //private bool canMove = true;

    [Tooltip("敌人目标车厢的索引")]
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

        //随机获取一个车厢索引
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

        //采用Transform硬移动，未采用刚体
        //TODO:后面调整这个移动，使用刚体
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
        //撞到列车
        if (other.CompareTag("Player"))
        {
            //将敌人大力反弹
            Vector3 normal = collision.GetContact(0).normal;
            body.AddForce(normal * 7f, ForceMode.Impulse);

            //造成伤害
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

            //为了防止立即死亡造成多次判断，等待到物理帧结束后再销毁
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
    /// 在车厢死亡后更换目标
    /// </summary>
    /// <param name="deadUnitTF">没啥用，来凑参数的</param>
    private void ChangeFollowIndex(Transform deadUnitTF = null)
    {
        followIndex = train.GetRandomIndexOfUnits();
    }
}
