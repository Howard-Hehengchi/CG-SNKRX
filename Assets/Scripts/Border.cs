using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField]
    private BoxCollider wallPrefab;

    private float wallHeight = 4f;

    private void OnEnable()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Not found main camera!");
        }

        Vector3 upperLeft, upperRight, bottomLeft, bottomRight;
        upperLeft = GetBorderPoint(0, mainCamera.pixelHeight - 1);
        bottomLeft = GetBorderPoint(0, 0);
        upperRight = GetBorderPoint(mainCamera.pixelWidth - 1, mainCamera.pixelHeight - 1);
        bottomRight = GetBorderPoint(mainCamera.pixelWidth - 1, 0);

        Vector3 verticalOffset, horizontalOffset;
        verticalOffset = (upperLeft + upperRight - bottomLeft - bottomRight).normalized;
        horizontalOffset = (upperRight + bottomRight - upperLeft - bottomLeft).normalized;

        Vector3 upCenter, bottomCenter, leftCenter, rightCenter;
        upCenter = (upperLeft + upperRight) / 2f + verticalOffset;
        bottomCenter = (bottomLeft + bottomRight) / 2f - verticalOffset;
        leftCenter = (upperLeft + bottomLeft) / 2f - horizontalOffset;
        rightCenter = (upperRight + bottomRight) / 2f + horizontalOffset;

        Vector3 upSize, bottomSize, leftSize, rightSize;
        upSize = new Vector3((upperRight - upperLeft).x, wallHeight, 2f);
        bottomSize = new Vector3((bottomRight - bottomLeft).x, wallHeight, 2f);
        leftSize = new Vector3(2f, wallHeight, (upperLeft - bottomLeft).z);
        rightSize = new Vector3(2f, wallHeight, (upperRight - bottomRight).z);

        BoxCollider upCollider = Instantiate(wallPrefab, upCenter, Quaternion.identity, transform);
        upCollider.size = upSize;
        BoxCollider bottomCollider = Instantiate(wallPrefab, bottomCenter, Quaternion.identity, transform);
        bottomCollider.size = bottomSize;
        BoxCollider leftCollider = Instantiate(wallPrefab, leftCenter, Quaternion.identity, transform);
        leftCollider.size = leftSize;
        BoxCollider rightCollider = Instantiate(wallPrefab, rightCenter, Quaternion.identity, transform);
        rightCollider.size = rightSize;
    }

    private Vector3 GetBorderPoint(float x, float y)
    {
        Vector2 screenCoord = new Vector2(x, y);
        if(Physics.Raycast(mainCamera.ScreenPointToRay(screenCoord), out RaycastHit hitInfo))
        {
            return hitInfo.point;
        }
        Debug.LogError("Could not find a border point for " + screenCoord + " !");
        return Vector3.zero;
    }
}
