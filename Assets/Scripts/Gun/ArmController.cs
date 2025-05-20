using UnityEngine;

public class ArmController : MonoBehaviour
{
    public Transform gun;
    public Transform playerBody;
    public float armLength = 1f;

    private Vector2 currentDirection = Vector2.right;

    public void SetAimDirection(Vector2 direction)
    {
        currentDirection = direction;
        UpdateArm();
    }

    private void UpdateArm()
    {
        float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
        gun.rotation = Quaternion.Euler(0, 0, angle);
        gun.localPosition = new Vector3(currentDirection.x * armLength, currentDirection.y * armLength, 0f);

        Vector3 bodyPos = playerBody.localPosition;
        playerBody.localPosition = new Vector3(bodyPos.x, bodyPos.y, 0f);

        gun.localScale = currentDirection.x < 0 ? new Vector3(1, -1, 1) : new Vector3(1, 1, 1);
    }
}