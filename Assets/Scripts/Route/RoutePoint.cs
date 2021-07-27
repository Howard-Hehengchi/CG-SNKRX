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
    }
}
