using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RouteManager))]
public class RouteGraphEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RouteManager route = (RouteManager)target;
        //EditorGUILayout.Space();

        EditorGUILayout.TextField("备注：更改路线后需要手动调整此处路线节点数量");

        //面板上第一个是节点数量，更新路线后需要自己手动调整
        route.pointCount = EditorGUILayout.IntSlider("Point Count", route.pointCount, 1, 15);

        //面板上第二个是邻接矩阵可视化图
        route.showGraph = EditorGUILayout.Foldout(route.showGraph, "Graph");
        if (route.showGraph)
        {
            EditorGUI.indentLevel = 0;

            GUIStyle tableStyle = new GUIStyle("box");
            tableStyle.padding = new RectOffset(10, 10, 10, 10);
            tableStyle.margin.left = 32;

            GUIStyle headerColumnStyle = new GUIStyle();
            headerColumnStyle.fixedWidth = 40;

            GUIStyle columnStyle = new GUIStyle();
            columnStyle.fixedWidth = 20;

            GUIStyle rowStyle = new GUIStyle();
            rowStyle.fixedHeight = 25;
            rowStyle.fixedWidth = 65;

            GUIStyle rowHeaderStyle = new GUIStyle();
            rowHeaderStyle.fixedWidth = columnStyle.fixedWidth - 1;

            GUIStyle columnHeaderStyle = new GUIStyle();
            columnHeaderStyle.fixedWidth = 30;
            columnHeaderStyle.fixedHeight = 25.5f;

            GUIStyle columnLabelStyle = new GUIStyle();
            columnLabelStyle.fixedWidth = rowHeaderStyle.fixedWidth - 6;
            columnLabelStyle.alignment = TextAnchor.MiddleCenter;
            columnLabelStyle.fontStyle = FontStyle.Bold;
            columnLabelStyle.normal.textColor = Color.white;

            GUIStyle cornerLabelStyle = new GUIStyle();
            cornerLabelStyle.fixedWidth = 35;
            cornerLabelStyle.stretchWidth = true;
            cornerLabelStyle.alignment = TextAnchor.MiddleRight;
            cornerLabelStyle.fontStyle = FontStyle.BoldAndItalic;
            cornerLabelStyle.fontSize = 14;
            cornerLabelStyle.padding.top = -5;
            cornerLabelStyle.normal.textColor = Color.white;

            GUIStyle rowLabelStyle = new GUIStyle();
            rowLabelStyle.fixedWidth = 25;
            rowLabelStyle.stretchWidth = true;
            rowLabelStyle.alignment = TextAnchor.MiddleRight;
            rowLabelStyle.fontStyle = FontStyle.Bold;
            rowLabelStyle.normal.textColor = Color.white;

            EditorGUILayout.BeginHorizontal(tableStyle);
            for (int x = -1; x < route.pointCount; x++)
            {
                EditorGUILayout.BeginVertical((x == -1) ? headerColumnStyle : columnStyle);
                for (int y = -1; y < route.pointCount; y++)
                {
                    if (x == -1 && y == -1)
                    {
                        EditorGUILayout.BeginVertical(rowHeaderStyle);
                        EditorGUILayout.LabelField("Points", cornerLabelStyle);
                        EditorGUILayout.EndHorizontal();
                    }
                    else if (x == -1)
                    {
                        EditorGUILayout.BeginVertical(columnHeaderStyle);
                        EditorGUILayout.LabelField(y.ToString(), rowLabelStyle);
                        EditorGUILayout.EndHorizontal();
                    }
                    else if (y == -1)
                    {
                        EditorGUILayout.BeginVertical(rowHeaderStyle);
                        EditorGUILayout.LabelField(x.ToString(), columnLabelStyle);
                        EditorGUILayout.EndHorizontal();
                    }

                    if (x >= 0 && y >= 0)
                    {
                        EditorGUILayout.BeginHorizontal(rowStyle);
                                                
                        //toggle的二维数组，因为是双向图的邻接矩阵，所以所有的改变应当对称进行
                        route.graph[y, x] = route.graph[x, y] = EditorGUILayout.Toggle(route.graph[y, x], GUILayout.Width(16));
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();

            //强制根据数据更新Inspector面板上显示结果
            //Repaint();
        }
    }
}
