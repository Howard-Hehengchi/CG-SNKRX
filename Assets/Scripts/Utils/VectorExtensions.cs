using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    /// <summary>
    /// 将变量投影到xOz平面上
    /// </summary>
    /// <param name="vector">需要投影的变量</param>
    /// <returns></returns>
    public static Vector3 ProjectedOnPlane(this Vector3 vector)
    {
        return new Vector3(vector.x, 0f, vector.z);
    }

    /// <summary>
    /// 将变量关于法线做对称
    /// </summary>
    /// <param name="vector">需要对称的变量</param>
    /// <param name="normal">法线（可以不归一化）</param>
    /// <returns></returns>
    public static Vector3 ReflectedByVector(this Vector3 vector, Vector3 normal)
    {
        if(Vector3.Cross(vector, normal).sqrMagnitude <= 0.0001f)
        {
            return vector;
        }

        normal.Normalize();
        Vector3 projectedVector = Vector3.Dot(vector, normal) * normal;
        return 2f * projectedVector - vector;
    }
}
