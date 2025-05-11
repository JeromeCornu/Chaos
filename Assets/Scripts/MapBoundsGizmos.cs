using UnityEngine;

[ExecuteAlways]
public class MapBoundsGizmos : MonoBehaviour
{
    public Color boundsColor = Color.red;

    void OnDrawGizmos()
    {
        if (Camera.main == null) return;

        Camera cam = Camera.main;
        float z = 0f;

        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

        Vector3 topLeft = new Vector3(bottomLeft.x, topRight.y, z);
        Vector3 bottomRight = new Vector3(topRight.x, bottomLeft.y, z);

        Gizmos.color = boundsColor;
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
}
