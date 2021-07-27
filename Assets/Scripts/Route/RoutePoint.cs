using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单个节点，持有其相邻节点引用
/// </summary>
public class RoutePoint : MonoBehaviour
{
    [Tooltip("相邻的节点们")]
    private List<RoutePoint> adjacentPoints = new List<RoutePoint>();

    /// <summary>
    /// 添加相邻节点，同时更新那个节点的相邻节点
    /// </summary>
    /// <param name="neighbor">要添加的相邻节点</param>
    public void AddNeighbor(RoutePoint neighbor)
    {
        if (!adjacentPoints.Contains(neighbor))
        {
            adjacentPoints.Add(neighbor);
            neighbor.AddNeighbor(this);
        }
    }

    /// <summary>
    /// 移除相邻节点
    /// </summary>
    /// <param name="neighbor">需要移除的相邻节点</param>
    public void RemoveNeighbor(RoutePoint neighbor)
    {
        if (adjacentPoints.Contains(neighbor))
        {
            adjacentPoints.Remove(neighbor);
            neighbor.RemoveNeighbor(this);
        }
    }

    /// <summary>
    /// 拿到除了指定节点以外的所有相邻节点
    /// </summary>
    /// <param name="fromPoint">车厢从这个节点来，不需要考虑这个节点</param>
    /// <returns>剔除后的所有相邻节点</returns>
    public RoutePoint[] GetNextPoints(RoutePoint fromPoint)
    {
        List<RoutePoint> nextPoints = new List<RoutePoint>();
        foreach (var point in adjacentPoints)
        {
            if(point != fromPoint)
            {
                nextPoints.Add(point);
            }
        }

        return nextPoints.ToArray();
    }
}
