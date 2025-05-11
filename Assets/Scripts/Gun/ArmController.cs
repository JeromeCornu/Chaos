#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[ExecuteAlways]
public class ArmController : MonoBehaviour
{
    public Transform gun;
    public Transform playerBody;
    public float armLength = 1f;

    private void Update()
    {
        if (!Application.isPlaying) return;

        UpdateArm();
    }

    private void UpdateArm()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector3 direction = (mouseWorld - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gun.rotation = Quaternion.Euler(0, 0, angle);

        // Position the gun directly with Z = 0
        gun.localPosition = new Vector3(direction.x * armLength, direction.y * armLength, 0f);

        // Force player's visual Z to stay at 0
        Vector3 bodyPos = playerBody.localPosition;
        playerBody.localPosition = new Vector3(bodyPos.x, bodyPos.y, 0f);

        // Flip player horizontally and optionally the gun (visually)
        if (direction.x < 0)
        {
            gun.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            gun.localScale = new Vector3(1, 1, 1);
        }
    }

    private void OnDrawGizmos()
    {
        if (gun == null || playerBody == null) return;

        // Simulate aim direction in editor mode
        Vector3 mouseWorld = Camera.main != null ?
            Camera.main.ScreenToWorldPoint(Input.mousePosition) :
            transform.position + transform.right;
        mouseWorld.z = 0f;

        Vector3 direction = (mouseWorld - transform.position).normalized;

        // Move the gun visually in the editor
        gun.localPosition = direction * armLength;

        // Display the arm line in the Scene view
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, gun.position);
        Gizmos.DrawSphere(gun.position, 0.05f);
    }
}
