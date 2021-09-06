using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����ڵ㣬���������ڽڵ�����
/// </summary>
public class RoutePoint : MonoBehaviour
{
    [Tooltip("���ڵĽڵ���")]
    private List<RoutePoint> adjacentPoints = new List<RoutePoint>();

    /// <summary>
    /// ������ڽڵ㣬ͬʱ�����Ǹ��ڵ�����ڽڵ�
    /// </summary>
    /// <param name="neighbor">Ҫ��ӵ����ڽڵ�</param>
    public void AddNeighbor(RoutePoint neighbor)
    {
        if (!adjacentPoints.Contains(neighbor))
        {
            adjacentPoints.Add(neighbor);
            neighbor.AddNeighbor(this);
        }
    }

    /// <summary>
    /// �Ƴ����ڽڵ�
    /// </summary>
    /// <param name="neighbor">��Ҫ�Ƴ������ڽڵ�</param>
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
    /// �õ�����ָ���ڵ�������������ڽڵ�
    /// </summary>
    /// <param name="fromPoint">���������ڵ���������Ҫ��������ڵ�</param>
    /// <returns>�޳�����������ڽڵ�</returns>
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
    /// �õ���˽ڵ����������нڵ�
    /// </summary>
    /// <returns>һ��һά���飬װ�����������ڵ�</returns>
    public RoutePoint[] GetNeighbors()
    {
        return adjacentPoints.ToArray();
    }

    /// <summary>
    /// �õ�����ָ���ڵ���������ڽڵ�
    /// </summary>
    /// <param name="fromPoint">���������ڵ���������Ҫ��������ڵ�</param>
    /// <param name="anotherPoint">����˽ڵ��������ڵ���������Ӧ�û���һ���ڵ�</param>
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
