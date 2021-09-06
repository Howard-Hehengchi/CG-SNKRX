using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于在游戏开始时给摄像机边界加上碰撞体限制物体运动范围
/// </summary>
public class Border : MonoBehaviour
{
    [Tooltip("以这个摄像机作为边界的判断标准，一般是主相机")]
    private Camera mainCamera;

    [SerializeField, Tooltip("单面墙的预制体，用于边界生成")]
    private BoxCollider wallPrefab;

    private float wallHeight = 4f;

    private void OnEnable()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Not found main camera!");
        }

        //先找到屏幕的四角在场地上的投影点
        Vector3 upperLeft, upperRight, bottomLeft, bottomRight;
        upperLeft = GetBorderPoint(0, mainCamera.pixelHeight - 1);
        bottomLeft = GetBorderPoint(0, 0);
        upperRight = GetBorderPoint(mainCamera.pixelWidth - 1, mainCamera.pixelHeight - 1);
        bottomRight = GetBorderPoint(mainCamera.pixelWidth - 1, 0);

        //然后计算长和宽
        Vector3 verticalOffset, horizontalOffset;
        verticalOffset = (upperLeft + upperRight - bottomLeft - bottomRight).normalized;
        horizontalOffset = (upperRight + bottomRight - upperLeft - bottomLeft).normalized;

        //然后找到四条边的中心并加以对应的偏移，用作生成点的中心
        Vector3 upCenter, bottomCenter, leftCenter, rightCenter;
        upCenter = (upperLeft + upperRight) / 2f + verticalOffset;
        bottomCenter = (bottomLeft + bottomRight) / 2f - verticalOffset;
        leftCenter = (upperLeft + bottomLeft) / 2f - horizontalOffset;
        rightCenter = (upperRight + bottomRight) / 2f + horizontalOffset;

        //上下左右墙（Box Collider）的碰撞体大小
        Vector3 upSize, bottomSize, leftSize, rightSize;
        upSize = new Vector3((upperRight - upperLeft).x, wallHeight, 2f);
        bottomSize = new Vector3((bottomRight - bottomLeft).x, wallHeight, 2f);
        leftSize = new Vector3(2f, wallHeight, (upperLeft - bottomLeft).z);
        rightSize = new Vector3(2f, wallHeight, (upperRight - bottomRight).z);

        //生成四面墙
        BoxCollider upCollider = Instantiate(wallPrefab, upCenter, Quaternion.identity, transform);
        upCollider.size = upSize;
        BoxCollider bottomCollider = Instantiate(wallPrefab, bottomCenter, Quaternion.identity, transform);
        bottomCollider.size = bottomSize;
        BoxCollider leftCollider = Instantiate(wallPrefab, leftCenter, Quaternion.identity, transform);
        leftCollider.size = leftSize;
        BoxCollider rightCollider = Instantiate(wallPrefab, rightCenter, Quaternion.identity, transform);
        rightCollider.size = rightSize;
    }

    /// <summary>
    /// 根据屏幕坐标获取场地中对应投影点位置
    /// </summary>
    /// <param name="x">屏幕横坐标，单位：像素</param>
    /// <param name="y">屏幕纵坐标，单位：像素</param>
    /// <returns></returns>
    private Vector3 GetBorderPoint(float x, float y)
    {
        //屏幕坐标
        Vector2 screenCoord = new Vector2(x, y);
        //相机投影
        if(Physics.Raycast(mainCamera.ScreenPointToRay(screenCoord), out RaycastHit hitInfo))
        {
            return hitInfo.point;
        }
        Debug.LogError("Could not find a border point for " + screenCoord + " !");
        return Vector3.zero;
    }
}
