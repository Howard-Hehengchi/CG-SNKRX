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

    /*
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
    }*/

    /// <summary>
    /// 拿到与此节点相连的所有节点
    /// </summary>
    /// <returns>一个一维数组，装有所有相连节点</returns>
    public RoutePoint[] GetNeighbors()
    {
        return adjacentPoints.ToArray();
    }

    /// <summary>
    /// 拿到除了指定节点以外的相邻节点
    /// </summary>
    /// <param name="fromPoint">车厢从这个节点来，不需要考虑这个节点</param>
    /// <param name="anotherPoint">如果此节点与三个节点相连，则应该还有一个节点</param>
    /// <returns></returns>
    public RoutePoint GetNextPoint(RoutePoint fromPoint, out RoutePoint anotherPoint)
    {
        List<RoutePoint> nextPoints = new List<RoutePoint>();
        foreach (var point in adjacentPoints)
        {
            if (point != fromPoint)
            {
                nextPoints.Add(point);
            }
        }

        anotherPoint = null;
        if(nextPoints.Count > 1)
        {
            anotherPoint = nextPoints[1];
        }
        return nextPoints[0];
    }
}
