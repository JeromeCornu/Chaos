using UnityEngine;

public class ArmController : MonoBehaviour
{
    public Transform gun;
    public Transform playerBody;
    public float armLength = 1f;

    private Vector2 currentDirection = Vector2.right;
    private Vector2 smoothedDirection = Vector2.right;
    private bool isLocal = false;

    public void SetAimDirection(Vector2 direction, bool isLocalPlayer)
    {
        currentDirection = direction;
        isLocal = isLocalPlayer;

        if (isLocal)
        {
            smoothedDirection = currentDirection;
            UpdateArm(smoothedDirection);
        }
    }

    private void LateUpdate()
    {
        if (!Application.isPlaying) return;

        // For remote players, smooth the arm direction
        if (!isLocal)
        {
            smoothedDirection = Vector2.Lerp(smoothedDirection, currentDirection, Time.deltaTime * 20f);
            UpdateArm(smoothedDirection);
        }
    }

    private void UpdateArm(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        gun.rotation = Quaternion.Euler(0, 0, angle);
        gun.localPosition = new Vector3(dir.x * armLength, dir.y * armLength, 0f);

        Vector3 bodyPos = playerBody.localPosition;
        playerBody.localPosition = new Vector3(bodyPos.x, bodyPos.y, 0f);

        gun.localScale = dir.x < 0 ? new Vector3(1, -1, 1) : new Vector3(1, 1, 1);
    }
}
