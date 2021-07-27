using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����ƶ�ֻ��������һ�ߣ�����ǰ������һ���ڵ��λ��
/// </summary>
public class UnitMove : MonoBehaviour
{
    [Tooltip("�г������ã����ڻ�ȡ·����Ϣ")]
    private TrainManager manager;

    public void SetTrainManager(TrainManager manager)
    {
        this.manager = manager;
    }

    [Tooltip("A->B��B����·��������")]
    private int toPointIndex = 1;
    [Tooltip("A->B��A��λ��")]
    private Vector3 fromPosition;
    [Tooltip("A->B��B��λ��")]
    private Vector3 toPosition;
    [Tooltip("A->B��AB����")]
    private float distance;

    [Tooltip("A->B->C��C��λ��")]
    private Vector3 nextPosition;

    //public void SetFromToPosition(Vector3 fromPosition, Vector3 toPosition)
    //{
    //    this.fromPosition = fromPosition;
    //    this.toPosition = toPosition;
    //    nextPosition = toPosition;
    //    distance = Vector3.Distance(fromPosition, toPosition);
    //}

    [Tooltip("A->B���ٷֱȽ���")]
    private float progress = 0f;
    [Tooltip("�Ƿ��Ѿ��Һ���һ���ڵ�")]
    private bool foundNext = false;

    /// <summary>
    /// ��ó��ᵱǰ�ƶ���Ϣ
    /// </summary>
    /// <returns></returns>
    public UnitMovementInfo GetInfo()
    {
        return new UnitMovementInfo(toPointIndex, fromPosition, toPosition, nextPosition, progress, foundNext);
    }

    /// <summary>
    /// ���ó������е��ƶ���Ϣ
    /// </summary>
    /// <param name="info">���õ��ƶ���Ϣ</param>
    public void SetInfo(UnitMovementInfo info)
    {
        toPointIndex = info.toPointIndex;
        fromPosition = info.fromPosition;
        toPosition = info.toPosition;
        distance = info.distance;
        nextPosition = info.nextPosition;
        progress = info.progress;
        foundNext = info.foundNext;
    }

    private void Update()
    {
        //����Ӧ��ǰ�����پ���
        float verticalInput = manager.verticalInput;
        float moveDistance = manager.speed * Time.deltaTime;
        moveDistance *= (verticalInput + 1.5f) / 2f;

        //ȷ����һ���ڵ�λ��
        if(progress >= TrainManager.searchAheadProgress && !foundNext)
        {
            nextPosition = manager.GetNextRoutePointPosition(ref toPointIndex);
            foundNext = true;
        }

        //A->B->C������B�㣬��·�߱��ΪB->C
        if (progress >= 1f)
        {
            UpdateFromToPosition();
            progress = 0f;
            foundNext = false;
        }

        //���ӽ��ȣ����ݽ��ȸ��³���λ��
        progress += moveDistance / distance;
        transform.position = Vector3.Lerp(fromPosition, toPosition, progress);
    }

    /// <summary>
    /// �������·�ߣ�A->B->C����A->B��ΪB->C
    /// </summary>
    private void UpdateFromToPosition()
    {
        fromPosition = toPosition;
        toPosition = nextPosition;
        //�ǵ����¼������˾���
        distance = Vector3.Distance(fromPosition, toPosition);
    }
}

/// <summary>
/// ����UnitMove�������ű��������ݣ����ڿ����Ƿ���Ҫ�ڲ�Ҳʹ��
/// </summary>
public class UnitMovementInfo
{
    public int toPointIndex;
    public Vector3 fromPosition;
    public Vector3 toPosition;
    public float distance;
    public Vector3 nextPosition;
    public float progress;
    public bool foundNext;

    /// <summary>
    /// ��ͨ�����ݹ��죬һ�������и�ֵ
    /// </summary>
    /// <param name="toPointIndex"></param>
    /// <param name="fromPosition"></param>
    /// <param name="toPosition"></param>
    /// <param name="nextPosition"></param>
    /// <param name="progress"></param>
    /// <param name="foundNext"></param>
    public UnitMovementInfo(int toPointIndex,
        Vector3 fromPosition, Vector3 toPosition, Vector3 nextPosition, 
        float progress, bool foundNext)
    {
        this.toPointIndex = toPointIndex;
        this.fromPosition = fromPosition;
        this.toPosition = toPosition;
        distance = Vector3.Distance(fromPosition, toPosition);
        this.nextPosition = nextPosition;
        this.progress = progress;
        this.foundNext = foundNext;
    }

    /// <summary>
    /// ��ʼ��ʱ�����ݹ��죬ֻ��Ҫ��ʼ�͵���λ��
    /// </summary>
    /// <param name="fromPosition"></param>
    /// <param name="toPosition"></param>
    public UnitMovementInfo(Vector3 fromPosition, Vector3 toPosition) 
        : this(1, fromPosition, toPosition, toPosition, 0f, false)
    {
        
    }
}