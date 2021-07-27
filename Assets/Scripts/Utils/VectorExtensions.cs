using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 ProjectedOnPlane(this Vector3 vector)
    {
        return new Vector3(vector.x, 0f, vector.z);
    }
}
