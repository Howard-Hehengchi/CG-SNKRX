using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    /// <summary>
    /// ������ͶӰ��xOzƽ����
    /// </summary>
    /// <param name="vector">��ҪͶӰ�ı���</param>
    /// <returns></returns>
    public static Vector3 ProjectedOnPlane(this Vector3 vector)
    {
        return new Vector3(vector.x, 0f, vector.z);
    }

    /// <summary>
    /// ���������ڷ������Գ�
    /// </summary>
    /// <param name="vector">��Ҫ�ԳƵı���</param>
    /// <param name="normal">���ߣ����Բ���һ����</param>
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
