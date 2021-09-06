using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����·�߽ڵ����ã����ڽӾ��󴢴�ڵ�������Ϣ
/// </summary>
public class RouteManager : MonoBehaviour
{
    [Header("·�߽ڵ㼰�༭")]
    [SerializeField, Tooltip("·�����нڵ�")]
    private List<Transform> routePoints;
    private int[] oldPointIndices = new int[1];
    [Tooltip("·�߽ڵ�����")]
    private int routePointCount;

    [SerializeField, Tooltip("���нڵ㼰�����ӷ�ʽ��ʾ�������༭ʹ�ã�")]
    private List<PointForInspector> points;

    [Header("·�߿��ӻ�")]
    [Tooltip("һ����")]
    public LineRenderer linePrefab;

    [Tooltip("�ڻ���ʱ���㷨Ѱ·�����ɢ�б�")]
    private List<List<RoutePoint>> routeLines;
    [Tooltip("���������������ڵ������Ľڵ㣬�㷨��Ҫ")]
    private List<RoutePoint> jointPoints;
    [Tooltip("��������������")]
    private List<LineRenderer> drawLines;

    [SerializeField, Tooltip("·�߿��"), Range(0f, 1f)]
    private float width;
    [SerializeField, Tooltip("���ڵ���Line Rendererת��ƽ����"), Range(0, 10)]
    private int cornerVertices;
    [SerializeField, Tooltip("���ڵ���Line Renderer��βƽ����"), Range(0, 10)]
    private int endCapVertices;

    [Tooltip("���ڽӾ��󴢴�·��ͼ��Ϣ")]
    private bool[,] graph = new bool[15, 15];
    [Tooltip("���ڱȶ�·����Ϣ�����б���������е���")]
    private bool[,] bufferGraph = new bool[15, 15];

    private void OnValidate()
    {
        /* ��ע�⣺������Build�����ʽ�汾��
         * ��OnValidate�жԲ��������޸�ֻ�ܶ���Inspector����е������Ĳ�����Ч
         * ���磺�б���AB��Inspector���޸�A��OnValidate���޸�AB����ֻ��A��Ч��B���·���
         * ���BUG��Build���֣���������ã��ּ�¼�ڴ�
         */

        routePointCount = routePoints.Count;
        //��ǰ�ж��ٽڵ㣬������������ֹ�ڵ�ı�������仯���±��������⣬����û��������ǣ����Ǳ�������
        int currentCount = points.Count;

        //����ڵ�����û�з����仯���Ҿ͵�ֻ�����ӷ�ʽ����
        if (currentCount == routePointCount)
        {
            //�Ȼ�ȡ���е�������Ϣ
            for (int i = 0; i < currentCount; i++)
            {
                bool[] connectionInfo = points[i].GetConnectionInfo();
                for (int j = 0; j < currentCount; j++)
                {
                    graph[i, j] = connectionInfo[j];
                }
            }

            //�ȶ�·����Ϣ�Ƿ��б�������������ݴ洢����Ӧ�ĵ���
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

        //���»�ȡ��ǰ���нڵ����
        List<int> pointIndices = new List<int>(routePointCount);
        for (int i = 0; i < routePointCount; i++)
        {
            pointIndices.Add(GetRoutePointIndex(routePoints[i].name));
        }

        //�������˽ڵ�
        if (currentCount < routePointCount)
        {
            //�������еĽڵ�����������Ľڵ�������Ϣ
            for (int i = 0; i < currentCount; i++)
            {
                PointForInspector currentPoint = points[i];
                //���ÿ��������Ľڵ㣬��Ϊ��Ĭ��������б�ĩβ�ģ�����ֱ�Ӵӽӽ�ĩβ�ĵط���ʼ�����Ϳ�����
                for (int j = currentCount; j < routePointCount; j++)
                {
                    currentPoint.Add(pointIndices[j]);
                }
                //�������б��ڵ����(Hierarchy������)��С�����������
                currentPoint.Sort();
            }

            //��������Ľڵ�
            for (int i = currentCount; i < routePointCount; i++)
            {
                points.Add(new PointForInspector(pointIndices[i], pointIndices.ToArray()));
            }
            //�������б��ڵ����(Hierarchy������)��С�����������
            points.Sort((a, b) => a.index.CompareTo(b.index));
        }
        //���ɾȥ�˽ڵ�
        else if (currentCount > routePointCount)
        {
            int length = oldPointIndices.Length;
            //�ȱ����Ա��¾ɽڵ��б��ҵ�ɾȥ���ĸ���Щ���ڵ�
            for (int i = 0; i < length; i++)
            {
                //�ҵ���
                if (!pointIndices.Contains(oldPointIndices[i]))
                {
                    int removeIndex = oldPointIndices[i];
                    PointForInspector pointToRemove = null;
                    //�����ڵ��б���ɾȥ���Ǹ��ڵ�ɾ�����޸������ڵ�������ڵ��б�
                    for (int j = 0; j < currentCount; j++)
                    {
                        PointForInspector currentPoint = points[j];
                        if (currentPoint.index == removeIndex)
                        {
                            //����ڵ���Ҫɾȥ�Ľڵ�
                            pointToRemove = currentPoint;
                        }
                        else
                        {
                            //�޸������ڵ�������ڵ�
                            currentPoint.RemoveOfIndex(removeIndex);
                        }
                    }
                    points.Remove(pointToRemove);
                }
            }
        }

        //���±���Ŀǰ���нڵ����
        pointIndices.Sort();
        oldPointIndices = pointIndices.ToArray();
    }

    /// <summary>
    /// ���ݽڵ���Hierarchy�е����Ƹ��������
    /// </summary>
    /// <param name="name">�ڵ���Hierarchy�е�����</param>
    /// <returns>�ڵ�����</returns>
    private int GetRoutePointIndex(string name)
    {
        //�ڵ����Ƹ�ʽ��
        //Route Point (x)
        //��Route Point (xx)
        //���� x(��xx) Ϊ���֣����˽ڵ����
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

        //�Ҳ��� (x) ���ǳ�������
        Debug.LogError("Point name format not legal!\nName: " + name);
        return -1;
    }

    private void Awake()
    {
        routePointCount = routePoints.Count;

        //�Ȼ�ȡ���е�������Ϣ
        for (int i = 0; i < routePointCount; i++)
        {
            bool[] connectionInfo = points[i].GetConnectionInfo();
            for (int j = 0; j < routePointCount; j++)
            {
                graph[i, j] = connectionInfo[j];
            }
        }

        //�ڿ�ʼ��ʱ�����ڽӾ���Ϊÿ���㸳�������ڵ�
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

    [Tooltip("ɢ�б�����ָʾ�ߵ�����")]
    private int lineId = 0;
    [Tooltip("ɢ�б�����ָʾ�������")]
    private int dotId = 1;
    private void Start()
    {
        /* �㷨��Ҫ��
         * ������·���з�Ϊ����û�з�֧��·�߽��л���
         * ÿ��������·�ߵ���㶼���������ڵ������ĵ㣬��Щ�㶼��ר�ŵ��б��¼
         * ����·�߼�¼��һ��ɢ�б��У�ÿ��·����ɢ�б��һ��Ԫ�أ�ÿ��·�߰�����·�ߵ����нڵ�
         * 
         * ��������ͼ򻯳��˷�������ʼ�����������·��
         * ����ÿ��·�ߣ��������죬����������з����µ���ʼ��ͼ�¼����������Ľ��������ǵ����ѱ���¼����ʼ��
         * һ��·��������Ϻ󣬶���һ��·�߽������죬ֱ������·�߶����������
         * 
         * ԭ�����㷨�����ж���·�ߵڶ�����������е���ʼ�������д�����������´�·�߽����ᱻ����
         * �����ڵ��Թ����з�����ʵӦ�������߶��Ƿ��ظ������������㷨�����ʮ�ָ��ӣ�����ʡȥ�����⴦����һ����
         * ��Ҳ�������ڻ���ʱ�������������ظ�·�ߣ�Ŀǰ��������Ӱ��ʹ�ã�Ϊ�˱�֤·��ȫ�������Ƴ�����ֻ��������Э
         * 
         * ���⣺��ʹ�������⴦��Ҳ�����ظ���·��
         * ����������ʼ�㻥��֮�����ӣ����Դ�A->B��Ҳ���Դ�B->A�������������ʼ��֮�仹���νӵ㣬��ô������·�߾��ᱻ����
         * //TODO:�����õķ�������дһ��·�߲��أ���������·���������ǵ�·��ɾȥ�������Ժ�����������ӣ�
         */

        routeLines = new List<List<RoutePoint>>();
        drawLines = new List<LineRenderer>();
        jointPoints = new List<RoutePoint>();

        //�ȶԵ�һ������в���
        //�õ���һ����������ھ�
        RoutePoint[] firstNeighbors = routePoints[0].GetComponent<RoutePoint>().GetNeighbors();
        //�ж�������ǲ��ǿ�����Ϊ��ʼ�㣨�������������ڵ������ĵ㣩
        if (firstNeighbors.Length == 3)
        {
            jointPoints.Add(routePoints[0].GetComponent<RoutePoint>());
        }
        //�ӵ�һ�������ÿ������·�ߵĳ�ʼ��
        foreach (var point in firstNeighbors)
        {
            List<RoutePoint> newLine = new List<RoutePoint>();
            newLine.Add(routePoints[0].GetComponent<RoutePoint>());
            newLine.Add(point);
            routeLines.Add(newLine);
        }

        //����ÿ��·�ߣ���������
        while (lineId < routeLines.Count)
        {
            //��·�ߵĵڶ����㿪ʼ��鲢����
            dotId = 1;

            List<RoutePoint> currentLine = routeLines[lineId];
            RoutePoint currentPoint = currentLine[1];
            //bool lineExist = false;

            //�������죬�����ϵ�Ѱ����һ���㣬ֱ�������ѱ���¼����ʼ��
            while (!jointPoints.Contains(currentPoint))
            {
                //lineExist = true;
                RoutePoint nextPoint = currentPoint.GetNextPoint(currentLine[dotId - 1], out RoutePoint anotherPoint);
                //���������һ����ı�ѡ�˵��������Ǹ���ʼ�㣬Ӧ�ü�¼����
                if (anotherPoint != null)
                {
                    jointPoints.Add(currentPoint);
                    //�ǵó�ʼ��һ���µ�·�߿���
                    List<RoutePoint> newLine = new List<RoutePoint>
                    {
                        currentPoint,
                        anotherPoint
                    };
                    routeLines.Add(newLine);
                }

                //����·��
                currentLine.Add(nextPoint);

                //����Ҫ�жϵĽڵ���ǰ�ƶ��������ͱ�֤��·����ǰ��һ���㣩
                dotId++;
                currentPoint = currentLine[dotId];
            }

            //if (lineExist)
            //{

            //����·���������ˣ����Կ�ʼ������
            //����Ԥ���岢��ʼ������
            LineRenderer lineRenderer = Instantiate(linePrefab, transform);
            lineRenderer.startWidth = lineRenderer.endWidth = width;
            lineRenderer.numCornerVertices = cornerVertices;
            lineRenderer.numCapVertices = endCapVertices;
            lineRenderer.positionCount = currentLine.Count;

            //һ�����ڵ����
            dotId = 0;
            foreach (var point in currentLine)
            {
                lineRenderer.SetPosition(dotId++, point.transform.position);
            }

            //��¼���Ѿ����Ƶ�·��
            drawLines.Add(lineRenderer);
            //}

            //ѡ����һ��·�߽�������
            lineId++;
        }
    }

    /// <summary>
    /// A->B->C����ȷ��C��
    /// </summary>
    /// <param name="fromIndex">A������</param>
    /// <param name="currentIndex">B������</param>
    /// <param name="directionParam">ת�����룬���û��Ĭ��Ϊ0</param>
    /// <returns></returns>
    public int GetNextIndex(int fromIndex, int currentIndex, float directionParam = 0)
    {
        //�õ��ڵ�����
        RoutePoint currentPoint = routePoints[currentIndex].GetComponent<RoutePoint>();
        //�õ��˽ڵ��������ڽڵ㣨������ʱ�Ľڵ㣩
        RoutePoint nextPoint = currentPoint.GetNextPoint(routePoints[fromIndex].GetComponent<RoutePoint>(), out RoutePoint anotherPoint);

        int nextIndex = -1;
        //����˽ڵ��������ڵ�������Ҳ���ǳ�����ʱ�Ľڵ�ֻʣһ����������û��ת�����룬��Ĭ����һ��
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

            //F->O��O��AB����
            Vector3 vOF = fromPosition - currentPosition;
            Vector3 vOA = nextPositionA - currentPosition;
            Vector3 vOB = nextPositionB - currentPosition;

            //����ˣ�ȷ��AB���ҵ����λ�ã�������Ļ��˵��
            Vector3 cross = Vector3.Cross(vOA, vOB);
            //��ʱ�ж�ֵ��AB�������λ�ú�ת��������ۺϿ���
            float value = cross.y * directionParam;

            //�ж�OF�Ƿ���OA��OB֮�䣬����ǵĻ���Ҫ���ж�ֵ�������AB���λ�ñ�Ϊ���ڻ���˵��
            float angleAB = Vector3.Angle(vOA, vOB);
            float angleAF = Vector3.Angle(vOA, vOF);
            float angleBF = Vector3.Angle(vOB, vOF);
            float difference = angleAB - angleAF - angleBF;

            if (difference > -0.1f && difference < 1f)
            {
                value *= -1;
            }

            //�����ж�ֵȷ����һ���ڵ�λ��
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
                //����ܲ���OA��OB���ߣ���OA��΢��OFƫ��һ��㣬���ظ���������
                vOA += vOF * 0.0001f;
                cross = Vector3.Cross(vOA, vOB);
                value = cross.y * directionParam;

                //����Ϊʲô�Ƿ���������Ҳ����������������������
                nextIndex = routePoints.IndexOf((value > 0 ? nextPoint : anotherPoint).transform);
            }
        }

        return nextIndex;
    }

    /// <summary>
    /// ���ݽڵ�������ȡ�ڵ����λ��
    /// </summary>
    /// <param name="pointIndex">�ڵ�����</param>
    /// <returns>�ýڵ�ľ���λ��</returns>
    public Vector3 GetPointPosition(int pointIndex)
    {
        return routePoints[pointIndex].position;
    }
}

/// <summary>
/// ����Inspector����б༭�Ľڵ㣬�ṩ�������ڵ��������Ϣ
/// </summary>
[System.Serializable]
public class PointForInspector
{
    [HideInInspector, Tooltip("�ڵ����ƣ������޸�Inspector������б�Ԫ����ʾ")]
    public string name;
    [HideInInspector, Tooltip("�ڵ���ţ���Hierarchy�����ͳһ")]
    public int index;
    [SerializeField, Tooltip("������ڵ��Ƿ�����������������Լ������ǲ���������")]
    private List<PointConnectionInfo> connectedPoints;

    /// <summary>
    /// ���ڵ���ź͵�ǰ�ܽڵ�������ʼ���ڵ�
    /// </summary>
    /// <param name="index">�˽ڵ���ţ���Hierarchy��ͳһ��</param>
    /// <param name="pointIndices">��ǰ���нڵ㣬�������������ڵ��б�</param>
    public PointForInspector(int index, int[] pointIndices)
    {
        this.index = index;
        name = "Point " + index.ToString();
        int pointCount = pointIndices.Length;
        //·�߽ڵ�ֻ��һ���ǻ��㴸��
        if (pointCount == 1)
        {
            Debug.LogError("Not allow only 1 point in route!");
        }

        //�������Լ������нڵ���б���Ϊ�����ڵ��б�
        connectedPoints = new List<PointConnectionInfo>(pointCount - 1);
        foreach (int pointIndex in pointIndices)
        {
            connectedPoints.Add(new PointConnectionInfo(pointIndex));
        }
    }

    /// <summary>
    /// Ϊ�˽ڵ�����µ������ڵ�
    /// </summary>
    /// <param name="index">�����ڵ�����</param>
    public void Add(int index)
    {
        connectedPoints.Add(new PointConnectionInfo(index));
    }

    /// <summary>
    /// �������ڵ��б���ɾ��ָ����ŵĽڵ�
    /// </summary>
    /// <param name="index">Ҫɾ���Ľڵ�����(��Hierarchy��ͳһ)</param>
    public void RemoveOfIndex(int index)
    {
        //�����б����ҵ�Ҫɾ���Ľڵ�
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
    /// �������ڵ��б���ţ���Hierarchy��ͳһ����С��������
    /// </summary>
    public void Sort()
    {
        connectedPoints.Sort((a, b) => a.index.CompareTo(b.index));
    }

    /// <summary>
    /// ��ȡ�˽ڵ��������ڵ��������Ϣ
    /// </summary>
    /// <returns>һ��һά���飬��ʾ�˽ڵ�����ڵ��Ƿ�����</returns>
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
    /// ���ڵ���ָ���ڵ�������ע�������index��Ĭ��Ϊ0, 1, 2, 3, 4...����ʽ
    /// </summary>
    /// <param name="index"></param>
    /// <param name="connected">�Ƿ�����</param>
    public void ConnectTo(int index, bool connected)
    {
        connectedPoints[index].Connect(connected);
    }
}

/// <summary>
/// �Ƿ���˽ڵ�����
/// </summary>
[System.Serializable]
public class PointConnectionInfo
{
    [HideInInspector, Tooltip("�ڵ����ƣ������޸�Inspector�����ʾ���б���Ԫ������")]
    public string name;
    [HideInInspector, Tooltip("�ڵ���ţ���Hierarchy�����ͳһ")]
    public int index;
    [Tooltip("�Ƿ�������ڵ�����")]
    public bool connected;

    /// <summary>
    /// �����ڵ���ţ������˽ڵ�������Ϣ
    /// </summary>
    /// <param name="pointIndex">Ҫ�����������ڵ�����</param>
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