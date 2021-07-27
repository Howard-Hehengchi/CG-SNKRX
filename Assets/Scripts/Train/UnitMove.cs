using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 车厢移动只考虑两点一线，并提前储存下一个节点的位置
/// </summary>
public class UnitMove : MonoBehaviour
{
    [Tooltip("列车的引用，用于获取路线信息")]
    private TrainManager manager;

    public void SetTrainManager(TrainManager manager)
    {
        this.manager = manager;
    }

    [Tooltip("A->B，B点在路线中索引")]
    private int toPointIndex = 1;
    [Tooltip("A->B，A点位置")]
    private Vector3 fromPosition;
    [Tooltip("A->B，B点位置")]
    private Vector3 toPosition;
    [Tooltip("A->B，AB距离")]
    private float distance;

    [Tooltip("A->B->C，C点位置")]
    private Vector3 nextPosition;

    //public void SetFromToPosition(Vector3 fromPosition, Vector3 toPosition)
    //{
    //    this.fromPosition = fromPosition;
    //    this.toPosition = toPosition;
    //    nextPosition = toPosition;
    //    distance = Vector3.Distance(fromPosition, toPosition);
    //}

    [Tooltip("A->B，百分比进度")]
    private float progress = 0f;
    [Tooltip("是否已经找好下一个节点")]
    private bool foundNext = false;

    /// <summary>
    /// 获得车厢当前移动信息
    /// </summary>
    /// <returns></returns>
    public UnitMovementInfo GetInfo()
    {
        return new UnitMovementInfo(toPointIndex, fromPosition, toPosition, nextPosition, progress, foundNext);
    }

    /// <summary>
    /// 设置车厢所有的移动信息
    /// </summary>
    /// <param name="info">设置的移动信息</param>
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
        //计算应当前进多少距离
        float verticalInput = manager.verticalInput;
        float moveDistance = manager.speed * Time.deltaTime;
        moveDistance *= (verticalInput + 1.5f) / 2f;

        //确定下一个节点位置
        if(progress >= TrainManager.searchAheadProgress && !foundNext)
        {
            nextPosition = manager.GetNextRoutePointPosition(ref toPointIndex);
            foundNext = true;
        }

        //A->B->C，到达B点，将路线变更为B->C
        if (progress >= 1f)
        {
            UpdateFromToPosition();
            progress = 0f;
            foundNext = false;
        }

        //增加进度，根据进度更新车厢位置
        progress += moveDistance / distance;
        transform.position = Vector3.Lerp(fromPosition, toPosition, progress);
    }

    /// <summary>
    /// 变更车厢路线，A->B->C，从A->B变为B->C
    /// </summary>
    private void UpdateFromToPosition()
    {
        fromPosition = toPosition;
        toPosition = nextPosition;
        //记得重新计算两端距离
        distance = Vector3.Distance(fromPosition, toPosition);
    }
}

/// <summary>
/// 用于UnitMove与其他脚本交流数据，正在考虑是否需要内部也使用
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
    /// 普通的数据构造，一个个进行赋值
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
    /// 初始化时的数据构造，只需要起始和到达位置
    /// </summary>
    /// <param name="fromPosition"></param>
    /// <param name="toPosition"></param>
    public UnitMovementInfo(Vector3 fromPosition, Vector3 toPosition) 
        : this(1, fromPosition, toPosition, toPosition, 0f, false)
    {
        
    }
}