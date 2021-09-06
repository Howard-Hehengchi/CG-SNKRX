using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 持有路线节点引用，用邻接矩阵储存节点连接信息
/// </summary>
public class RouteManager : MonoBehaviour
{
    [Header("路线节点及编辑")]
    [SerializeField, Tooltip("路线所有节点")]
    private List<Transform> routePoints;
    private int[] oldPointIndices = new int[1];
    [Tooltip("路线节点数量")]
    private int routePointCount;

    [SerializeField, Tooltip("所有节点及其连接方式显示（仅供编辑使用）")]
    private List<PointForInspector> points;

    [Header("路线可视化")]
    [Tooltip("一条线")]
    public LineRenderer linePrefab;

    [Tooltip("在画线时用算法寻路所需的散列表")]
    private List<List<RoutePoint>> routeLines;
    [Tooltip("所有与三个其他节点相连的节点，算法需要")]
    private List<RoutePoint> jointPoints;
    [Tooltip("画出来的所有线")]
    private List<LineRenderer> drawLines;

    [SerializeField, Tooltip("路线宽度"), Range(0f, 1f)]
    private float width;
    [SerializeField, Tooltip("用于调节Line Renderer转角平滑度"), Range(0, 10)]
    private int cornerVertices;
    [SerializeField, Tooltip("用于调节Line Renderer首尾平滑度"), Range(0, 10)]
    private int endCapVertices;

    [Tooltip("用邻接矩阵储存路线图信息")]
    private bool[,] graph = new bool[15, 15];
    [Tooltip("用于比对路线信息哪里有变更，并进行调整")]
    private bool[,] bufferGraph = new bool[15, 15];

    private void OnValidate()
    {
        /* 请注意：（对于Build后的正式版本）
         * 在OnValidate中对参数进行修改只能对于Inspector面板中调整过的参数起效
         * 例如：有变量AB，Inspector中修改A，OnValidate中修改AB，则只能A起效，B无事发生
         * 这个BUG在Build后发现，并调试许久，现记录在此
         */

        routePointCount = routePoints.Count;
        //当前有多少节点，本来是用来防止节点改变后数量变化导致遍历出问题，现在没有这个顾虑，但是变量保留
        int currentCount = points.Count;

        //如果节点数量没有发生变化，我就当只有连接方式改了
        if (currentCount == routePointCount)
        {
            //先获取所有的连接信息
            for (int i = 0; i < currentCount; i++)
            {
                bool[] connectionInfo = points[i].GetConnectionInfo();
                for (int j = 0; j < currentCount; j++)
                {
                    graph[i, j] = connectionInfo[j];
                }
            }

            //比对路线信息是否有变更，并进行数据存储上相应的调整
            for (int i = 0; i < currentCount; i++)
            {
                for (int j = 0; j < currentCount; j++)
                {
                    if (bufferGraph[i, j] != graph[i, j])
                    {
                        points[j].ConnectTo(i, graph[i, j]);
                        bufferGraph[i, j] = bufferGraph[j, i] = graph[j, i] = graph[i, j];
                    }
                }
            }
            return;
        }

        //重新获取当前所有节点序号
        List<int> pointIndices = new List<int>(routePointCount);
        for (int i = 0; i < routePointCount; i++)
        {
            pointIndices.Add(GetRoutePointIndex(routePoints[i].name));
        }

        //如果添加了节点
        if (currentCount < routePointCount)
        {
            //对于已有的节点添加上新增的节点连接信息
            for (int i = 0; i < currentCount; i++)
            {
                PointForInspector currentPoint = points[i];
                //添加每个多出来的节点，因为是默认添加在列表末尾的，所以直接从接近末尾的地方开始遍历就可以了
                for (int j = currentCount; j < routePointCount; j++)
                {
                    currentPoint.Add(pointIndices[j]);
                }
                //添加完对列表按节点序号(Hierarchy窗口中)从小到大进行排序
                currentPoint.Sort();
            }

            //添加新增的节点
            for (int i = currentCount; i < routePointCount; i++)
            {
                points.Add(new PointForInspector(pointIndices[i], pointIndices.ToArray()));
            }
            //添加完对列表按节点序号(Hierarchy窗口中)从小到大进行排序
            points.Sort((a, b) => a.index.CompareTo(b.index));
        }
        //如果删去了节点
        else if (currentCount > routePointCount)
        {
            int length = oldPointIndices.Length;
            //先遍历对比新旧节点列表，找到删去了哪个（些）节点
            for (int i = 0; i < length; i++)
            {
                //找到了
                if (!pointIndices.Contains(oldPointIndices[i]))
                {
                    int removeIndex = oldPointIndices[i];
                    PointForInspector pointToRemove = null;
                    //遍历节点列表，把删去的那个节点删掉，修改其他节点的相连节点列表
                    for (int j = 0; j < currentCount; j++)
                    {
                        PointForInspector currentPoint = points[j];
                        if (currentPoint.index == removeIndex)
                        {
                            //这个节点是要删去的节点
                            pointToRemove = currentPoint;
                        }
                        else
                        {
                            //修改其他节点的相连节点
                            currentPoint.RemoveOfIndex(removeIndex);
                        }
                    }
                    points.Remove(pointToRemove);
                }
            }
        }

        //重新保存目前所有节点序号
        pointIndices.Sort();
        oldPointIndices = pointIndices.ToArray();
    }

    /// <summary>
    /// 根据节点在Hierarchy中的名称给出其序号
    /// </summary>
    /// <param name="name">节点在Hierarchy中的名称</param>
    /// <returns>节点的序号</returns>
    private int GetRoutePointIndex(string name)
    {
        //节点名称格式：
        //Route Point (x)
        //或Route Point (xx)
        //其中 x(或xx) 为数字，即此节点序号
        for (int i = 0; i < name.Length; i++)
        {
            if (name[i] == '(')
            {
                if (name[i + 2] == ')')
                {
                    return name[i + 1] - '0';
                }
                else if (name[i + 3] == ')')
                {
                    return (name[i + 1] - '0') * 10 + (name[i + 2] - '0');
                }
                else
                {
                    Debug.LogError("Undentified route point name format!");
                }
            }
        }

        //找不到 (x) 就是出问题了
        Debug.LogError("Point name format not legal!\nName: " + name);
        return -1;
    }

    private void Awake()
    {
        routePointCount = routePoints.Count;

        //先获取所有的连接信息
        for (int i = 0; i < routePointCount; i++)
        {
            bool[] connectionInfo = points[i].GetConnectionInfo();
            for (int j = 0; j < routePointCount; j++)
            {
                graph[i, j] = connectionInfo[j];
            }
        }

        //在开始的时候按照邻接矩阵为每个点赋上相连节点
        for (int i = 0; i < routePointCount; i++)
        {
            RoutePoint routePoint = routePoints[i].GetComponent<RoutePoint>();
            for (int j = i + 1; j < routePointCount; j++)
            {
                if (graph[i, j])
                {
                    routePoint.AddNeighbor(routePoints[j].GetComponent<RoutePoint>());
                }
            }
        }
    }

    [Tooltip("散列表用于指示线的索引")]
    private int lineId = 0;
    [Tooltip("散列表用于指示点的索引")]
    private int dotId = 1;
    private void Start()
    {
        /* 算法简要：
         * 将所有路线切分为多条没有分支的路线进行绘制
         * 每条单独的路线的起点都是与三个节点相连的点，这些点都有专门的列表记录
         * 所有路线记录在一个散列表中，每个路线是散列表的一个元素，每个路线包含本路线的所有节点
         * 
         * 于是问题就简化成了发现新起始点和延伸已有路线
         * 对于每条路线，进行延伸，在延伸过程中发现新的起始点就记录下来，延伸的结束条件是到达已被记录的起始点
         * 一条路线延伸完毕后，对下一条路线进行延伸，直到所有路线都被延伸完毕
         * 
         * 原本的算法方案中对于路线第二个点就是已有的起始点的情况有处理，这种情况下此路线将不会被绘制
         * 但是在调试过程中发现其实应当考虑线段是否重复，但是这样算法将变得十分复杂，于是省去了特殊处理这一步骤
         * 这也导致了在绘制时会有许多冗余的重复路线，目前看来并不影响使用，为了保证路线全部被绘制出来，只能做此妥协
         * 
         * 另外：即使做了特殊处理，也会有重复的路线
         * 即，两个起始点互相之间连接，可以从A->B，也可以从B->A，如果这两个起始点之间还有衔接点，那么这两条路线均会被保留
         * //TODO:因此最好的方案是再写一段路线查重，将被其他路线整个覆盖的路线删去（此条以后有需求再添加）
         */

        routeLines = new List<List<RoutePoint>>();
        drawLines = new List<LineRenderer>();
        jointPoints = new List<RoutePoint>();

        //先对第一个点进行操作
        //拿到第一个点的所有邻居
        RoutePoint[] firstNeighbors = routePoints[0].GetComponent<RoutePoint>().GetNeighbors();
        //判断这个点是不是可以作为起始点（即与其他三个节点相连的点）
        if (firstNeighbors.Length == 3)
        {
            jointPoints.Add(routePoints[0].GetComponent<RoutePoint>());
        }
        //从第一个点进行每条可能路线的初始化
        foreach (var point in firstNeighbors)
        {
            List<RoutePoint> newLine = new List<RoutePoint>();
            newLine.Add(routePoints[0].GetComponent<RoutePoint>());
            newLine.Add(point);
            routeLines.Add(newLine);
        }

        //对于每条路线，进行延伸
        while (lineId < routeLines.Count)
        {
            //从路线的第二个点开始检查并延伸
            dotId = 1;

            List<RoutePoint> currentLine = routeLines[lineId];
            RoutePoint currentPoint = currentLine[1];
            //bool lineExist = false;

            //进行延伸，即不断地寻找下一个点，直到碰到已被记录的起始点
            while (!jointPoints.Contains(currentPoint))
            {
                //lineExist = true;
                RoutePoint nextPoint = currentPoint.GetNextPoint(currentLine[dotId - 1], out RoutePoint anotherPoint);
                //如果还有下一个点的备选项，说明这个点是个起始点，应该记录下来
                if (anotherPoint != null)
                {
                    jointPoints.Add(currentPoint);
                    //记得初始化一条新的路线开端
                    List<RoutePoint> newLine = new List<RoutePoint>
                    {
                        currentPoint,
                        anotherPoint
                    };
                    routeLines.Add(newLine);
                }

                //延伸路线
                currentLine.Add(nextPoint);

                //将需要判断的节点向前移动（这样就保证是路线最前端一个点）
                dotId++;
                currentPoint = currentLine[dotId];
            }

            //if (lineExist)
            //{

            //这条路线延伸完了，可以开始绘制了
            //生成预制体并初始化参数
            LineRenderer lineRenderer = Instantiate(linePrefab, transform);
            lineRenderer.startWidth = lineRenderer.endWidth = width;
            lineRenderer.numCornerVertices = cornerVertices;
            lineRenderer.numCapVertices = endCapVertices;
            lineRenderer.positionCount = currentLine.Count;

            //一个个节点绘制
            dotId = 0;
            foreach (var point in currentLine)
            {
                lineRenderer.SetPosition(dotId++, point.transform.position);
            }

            //记录下已经绘制的路线
            drawLines.Add(lineRenderer);
            //}

            //选择下一条路线进行延伸
            lineId++;
        }
    }

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
        RoutePoint nextPoint = currentPoint.GetNextPoint(routePoints[fromIndex].GetComponent<RoutePoint>(), out RoutePoint anotherPoint);

        int nextIndex = -1;
        //如果此节点与两个节点相连（也就是除掉来时的节点只剩一个），或者没有转向输入，就默认下一个
        if (anotherPoint == null || directionParam == 0)
        {
            nextIndex = routePoints.IndexOf(nextPoint.transform);
        }
        else if (anotherPoint != null)
        {
            Vector3 fromPosition = routePoints[fromIndex].position;
            Vector3 currentPosition = routePoints[currentIndex].position;
            Vector3 nextPositionA = nextPoint.transform.position;
            Vector3 nextPositionB = anotherPoint.transform.position;

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
            if (value < 0)
            {
                nextIndex = routePoints.IndexOf(nextPoint.transform);
            }
            else if (value > 0)
            {
                nextIndex = routePoints.IndexOf(anotherPoint.transform);
            }
            else
            {
                //如果很不巧OA和OB共线，将OA稍微向OF偏移一点点，再重复上述过程
                vOA += vOF * 0.0001f;
                cross = Vector3.Cross(vOA, vOB);
                value = cross.y * directionParam;

                //这里为什么是反过来的我也不清楚，反正测出来是这样
                nextIndex = routePoints.IndexOf((value > 0 ? nextPoint : anotherPoint).transform);
            }
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

/// <summary>
/// 用于Inspector面板中编辑的节点，提供与其他节点的连接信息
/// </summary>
[System.Serializable]
public class PointForInspector
{
    [HideInInspector, Tooltip("节点名称，用于修改Inspector面板中列表元素显示")]
    public string name;
    [HideInInspector, Tooltip("节点序号，与Hierarchy面板中统一")]
    public int index;
    [SerializeField, Tooltip("与这个节点是否相连，这里包括了自己，但是并不起作用")]
    private List<PointConnectionInfo> connectedPoints;

    /// <summary>
    /// 给节点序号和当前总节点数，初始化节点
    /// </summary>
    /// <param name="index">此节点序号（与Hierarchy中统一）</param>
    /// <param name="pointIndices">当前所有节点，用于生成相连节点列表</param>
    public PointForInspector(int index, int[] pointIndices)
    {
        this.index = index;
        name = "Point " + index.ToString();
        int pointCount = pointIndices.Length;
        //路线节点只有一个那还搞锤子
        if (pointCount == 1)
        {
            Debug.LogError("Not allow only 1 point in route!");
        }

        //创建除自己外所有节点的列表，此为相连节点列表
        connectedPoints = new List<PointConnectionInfo>(pointCount - 1);
        foreach (int pointIndex in pointIndices)
        {
            connectedPoints.Add(new PointConnectionInfo(pointIndex));
        }
    }

    /// <summary>
    /// 为此节点添加新的相连节点
    /// </summary>
    /// <param name="index">相连节点的序号</param>
    public void Add(int index)
    {
        connectedPoints.Add(new PointConnectionInfo(index));
    }

    /// <summary>
    /// 从相连节点列表中删掉指定序号的节点
    /// </summary>
    /// <param name="index">要删掉的节点的序号(与Hierarchy中统一)</param>
    public void RemoveOfIndex(int index)
    {
        //遍历列表以找到要删掉的节点
        int i;
        for (i = 0; i < connectedPoints.Count; i++)
        {
            if (connectedPoints[i].index == index)
            {
                break;
            }
        }
        connectedPoints.RemoveAt(i);
    }

    /// <summary>
    /// 将相连节点列表按序号（与Hierarchy中统一）从小到大排序
    /// </summary>
    public void Sort()
    {
        connectedPoints.Sort((a, b) => a.index.CompareTo(b.index));
    }

    /// <summary>
    /// 获取此节点与其他节点的连接信息
    /// </summary>
    /// <returns>一个一维数组，表示此节点与各节点是否相连</returns>
    public bool[] GetConnectionInfo()
    {
        List<bool> info = new List<bool>();
        for (int i = 0; i < connectedPoints.Count; i++)
        {
            info.Add(connectedPoints[i].connected);
        }
        return info.ToArray();
    }

    /// <summary>
    /// 将节点与指定节点相连，注意这里的index是默认为0, 1, 2, 3, 4...的样式
    /// </summary>
    /// <param name="index"></param>
    /// <param name="connected">是否相连</param>
    public void ConnectTo(int index, bool connected)
    {
        connectedPoints[index].Connect(connected);
    }
}

/// <summary>
/// 是否与此节点相连
/// </summary>
[System.Serializable]
public class PointConnectionInfo
{
    [HideInInspector, Tooltip("节点名称，用于修改Inspector面板显示的列表中元素名称")]
    public string name;
    [HideInInspector, Tooltip("节点序号，与Hierarchy面板中统一")]
    public int index;
    [Tooltip("是否与这个节点相连")]
    public bool connected;

    /// <summary>
    /// 给定节点序号，创建此节点连接信息
    /// </summary>
    /// <param name="pointIndex">要创建的相连节点的序号</param>
    public PointConnectionInfo(int pointIndex)
    {
        name = "point " + pointIndex.ToString();
        index = pointIndex;
        connected = false;
    }

    public void Connect(bool connected)
    {
        this.connected = connected;
    }
}