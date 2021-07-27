using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����·�߽ڵ����ã����ڽӾ��󴢴�ڵ�������Ϣ
/// </summary>
public class RouteManager : MonoBehaviour
{
    [Tooltip("·�����нڵ�")]
    private List<Transform> routePoints = new List<Transform>();

    private LineRenderer routeLines;

#if UNITY_EDITOR
    //�˴����Զ���Inspector���ר�ò���

    [Tooltip("�Ƿ�չ����ͼ��Ĭ��չ����")]
    public bool showGraph = true;

    [Tooltip("����Inspector�Ͽ��ӻ��ڽӾ�����ʾ��С")]
    public int pointCount = 10;

#endif
    
    [Tooltip("���ڽӾ��󴢴�·��ͼ��Ϣ")]
    public bool[,] graph = new bool[15, 15];
    [Tooltip("���ڱȶ�·����Ϣ�����б���������е���")]
    private bool[,] bufferGraph = new bool[15, 15];

    private void Awake()
    {
        //�õ�·�����нڵ�����
        int routePointCount = transform.childCount;
        for(int i = 0; i < routePointCount; i++)
        {
            routePoints.Add(transform.GetChild(i));
        }
        
        //��ʼ��·��ͼ����˳��������0-1-2-3-4-5-6-0
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

        //����һ�ݿ���
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

        //·�߼򵥿��ӻ�
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

        //�ȶ�·����Ϣ�Ƿ��б�������������ݴ洢����Ӧ�ĵ���
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
        RoutePoint[] nextPoints = currentPoint.GetNextPoints(routePoints[fromIndex].GetComponent<RoutePoint>());

        int nextIndex = -1;
        //���ֻ��һ��ѡ�񣬻���û��ת�����룬��Ĭ����һ��
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
                //����ܲ���OA��OB���ߣ���OA��΢��OFƫ��һ��㣬���ظ���������
                vOA += vOF * 0.0001f;
                cross = Vector3.Cross(vOA, vOB);
                value = cross.y * directionParam;

                //����Ϊʲô�Ƿ���������Ҳ����������������������
                nextIndex = routePoints.IndexOf(nextPoints[value > 0 ? 0 : 1].transform);
            }
        }
        else
        {
            //������ڵ����ĸ����Ͻڵ���������Ϊ�������������ж�
            Debug.LogError("Up to 4 adjacent points not allowed!");
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