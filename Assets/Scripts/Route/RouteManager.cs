using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 持有路线节点引用，用邻接矩阵储存节点连接信息
/// </summary>
public class RouteManager : MonoBehaviour
{
    [Tooltip("路线所有节点")]
    private List<Transform> routePoints = new List<Transform>();

    private LineRenderer routeLines;

#if UNITY_EDITOR
    //此处放自定义Inspector面板专用参数

    [Tooltip("是否展开此图（默认展开）")]
    public bool showGraph = true;

    [Tooltip("控制Inspector上可视化邻接矩阵显示大小")]
    public int pointCount = 10;

#endif
    
    [Tooltip("用邻接矩阵储存路线图信息")]
    public bool[,] graph = new bool[15, 15];
    [Tooltip("用于比对路线信息哪里有变更，并进行调整")]
    private bool[,] bufferGraph = new bool[15, 15];

    private void Awake()
    {
        //拿到路线所有节点引用
        int routePointCount = transform.childCount;
        for(int i = 0; i < routePointCount; i++)
        {
            routePoints.Add(transform.GetChild(i));
        }
        
        //初始化路线图，按顺序相连，0-1-2-3-4-5-6-0
        for(int i = 0; i < routePointCount - 1; i++)
        {
            RoutePoint routePoint = routePoints[i].GetComponent<RoutePoint>();
            routePoint.AddNeighbor(routePoints[i + 1].GetComponent<RoutePoint>());
            graph[i, i + 1] = graph[i + 1, i] = true;
        }

        for(int i = 1; i < routePointCount;i++)
        {
            RoutePoint routePoint = routePoints[i].GetComponent<RoutePoint>();
            routePoint.AddNeighbor(routePoints[i - 1].GetComponent<RoutePoint>());
            graph[i, i - 1] = graph[i - 1, i] = true;
        }
        routePoints[routePointCount - 1].GetComponent<RoutePoint>().AddNeighbor(routePoints[0].GetComponent<RoutePoint>());
        graph[0, routePointCount - 1] = graph[routePointCount - 1, 0] = true;

        routePoints[3].GetComponent<RoutePoint>().AddNeighbor(routePoints[0].GetComponent<RoutePoint>());
        graph[0, 3] = graph[3, 0] = true;

        //复制一份拷贝
        System.Array.Copy(graph, bufferGraph, graph.Length);
    }

    private Vector3 offset = Vector3.up * 0.5f;
    private void Start()
    {
        routeLines = GetComponent<LineRenderer>();
        routeLines.positionCount = 8;

        SetLinePosition(0);
        SetLinePosition(1);
        SetLinePosition(2);
        SetLinePosition(3);
        SetLinePosition(0);
        SetLinePosition(5);
        SetLinePosition(4);
        SetLinePosition(3);
    }

    private int lineIndex = 0;
    private void SetLinePosition(int index)
    {
        routeLines.SetPosition(lineIndex++, routePoints[index].position + offset);
    }

    /*
    private void OnDrawGizmos()
    {

        //路线简单可视化
        Gizmos.color = Color.black;
        for(int i = 0; i < routePoints.Count; i++)
        {
            for(int j = i + 1; j < routePoints.Count; j++)
            {
                if(graph[i, j])
                {
                    Gizmos.DrawLine(routePoints[i].position, routePoints[j].position);
                }
            }
        }

        //比对路线信息是否有变更，并进行数据存储上相应的调整
        for (int i = 0; i < routePoints.Count; i++)
        {
            for (int j = i + 1; j < routePoints.Count; j++)
            {
                if (bufferGraph[i, j] != graph[i, j])
                {
                    RoutePoint currentPoint = routePoints[i].GetComponent<RoutePoint>();
                    if (graph[i, j])
                    {
                        currentPoint.AddNeighbor(routePoints[j].GetComponent<RoutePoint>());
                    }
                    else
                    {
                        currentPoint.RemoveNeighbor(routePoints[j].GetComponent<RoutePoint>());
                    }
                }
                bufferGraph[i, j] = graph[i, j];
            }
        }
        
    }
    */

    /// <summary>
    /// A->B->C，现确定C点
    /// </summary>
    /// <param name="fromIndex">A点索引</param>
    /// <param name="currentIndex">B点索引</param>
    /// <param name="directionParam">转向输入，如果没有默认为0</param>
    /// <returns></returns>
    public int GetNextIndex(int fromIndex, int currentIndex, float directionParam = 0)
    {
        //拿到节点数据
        RoutePoint currentPoint = routePoints[currentIndex].GetComponent<RoutePoint>();
        //拿到此节点所有相邻节点（除了来时的节点）
        RoutePoint[] nextPoints = currentPoint.GetNextPoints(routePoints[fromIndex].GetComponent<RoutePoint>());

        int nextIndex = -1;
        //如果只有一个选择，或者没有转向输入，就默认下一个
        if(nextPoints.Length == 1 || directionParam == 0)
        {
            nextIndex = routePoints.IndexOf(nextPoints[0].transform);
        }
        else if(nextPoints.Length == 2)
        {
            Vector3 fromPosition = routePoints[fromIndex].position;
            Vector3 currentPosition = routePoints[currentIndex].position;
            Vector3 nextPositionA = nextPoints[0].transform.position;
            Vector3 nextPositionB = nextPoints[1].transform.position;

            //F->O，O与AB相邻
            Vector3 vOF = fromPosition - currentPosition;
            Vector3 vOA = nextPositionA - currentPosition;
            Vector3 vOB = nextPositionB - currentPosition;

            //做叉乘，确定AB左右的相对位置（对于屏幕来说）
            Vector3 cross = Vector3.Cross(vOA, vOB);
            //此时判断值是AB左右相对位置和转向输入的综合考虑
            float value = cross.y * directionParam;

            //判断OF是否在OA和OB之间，如果是的话需要将判断值反向（因此AB相对位置变为对于火车来说）
            float angleAB = Vector3.Angle(vOA, vOB);
            float angleAF = Vector3.Angle(vOA, vOF);
            float angleBF = Vector3.Angle(vOB, vOF);
            float difference = angleAB - angleAF - angleBF;

            if (difference > -0.1f && difference < 1f)
            {
                value *= -1;
            }

            //根据判断值确定下一个节点位置
            if(value < 0)
            {
                nextIndex = routePoints.IndexOf(nextPoints[0].transform);
            }
            else if(value > 0)
            {
                nextIndex = routePoints.IndexOf(nextPoints[1].transform);
            }
            else
            {
                //如果很不巧OA和OB共线，将OA稍微向OF偏移一点点，再重复上述过程
                vOA += vOF * 0.0001f;
                cross = Vector3.Cross(vOA, vOB);
                value = cross.y * directionParam;

                //这里为什么是反过来的我也不清楚，反正测出来是这样
                nextIndex = routePoints.IndexOf(nextPoints[value > 0 ? 0 : 1].transform);
            }
        }
        else
        {
            //不允许节点与四个以上节点相连，因为这样会搅乱玩家判断
            Debug.LogError("Up to 4 adjacent points not allowed!");
        }

        return nextIndex;
    }

    /// <summary>
    /// 根据节点索引获取节点具体位置
    /// </summary>
    /// <param name="pointIndex">节点索引</param>
    /// <returns>该节点的具体位置</returns>
    public Vector3 GetPointPosition(int pointIndex)
    {
        return routePoints[pointIndex].position;
    }
}