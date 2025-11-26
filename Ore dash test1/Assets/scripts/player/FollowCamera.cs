using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;
    public BoxCollider2D levelBounds;

    private float minX, maxX, minY, maxY;
    private float halfWidth;
    private float halfHeight;

    void Start()
    {
        Camera cam = Camera.main;
        halfHeight = cam.orthographicSize;
        halfWidth = cam.aspect * halfHeight;

        // Pobranie granic z BoxCollider2D
        Bounds b = levelBounds.bounds;
        minX = b.min.x;
        maxX = b.max.x;
        minY = b.min.y;
        maxY = b.max.y;
    }

    void LateUpdate()
    {
        Vector3 pos = target.position;

        pos.x = Mathf.Clamp(pos.x, minX + halfWidth, maxX - halfWidth);
        pos.y = Mathf.Clamp(pos.y, minY + halfHeight, maxY - halfHeight);

        pos.z = transform.position.z;

        transform.position = pos;
    }
}
