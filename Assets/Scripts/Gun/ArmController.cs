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
        if (!Application.isPlaying) return; // Ne tourne qu'en Play Mode

        UpdateArm();
    }

    private void UpdateArm()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector3 direction = (mouseWorld - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gun.rotation = Quaternion.Euler(0, 0, angle);

        // Positionner le gun avec Z = 0 directement
        gun.localPosition = new Vector3(direction.x * armLength, direction.y * armLength, 0f);

        // Forcer le Z du corps aussi
        Vector3 bodyPos = playerBody.localPosition;
        playerBody.localPosition = new Vector3(bodyPos.x, bodyPos.y, 0f);

        // Flip horizontal du joueur et éventuel flip du gun (si visuel)
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

        // Simule la direction souris même en mode éditeur
        Vector3 mouseWorld = Camera.main != null ?
            Camera.main.ScreenToWorldPoint(Input.mousePosition) :
            transform.position + transform.right;
        mouseWorld.z = 0f;

        Vector3 direction = (mouseWorld - transform.position).normalized;

        // Déplace localement le gun
        gun.localPosition = direction * armLength;

        // Affiche visuellement la ligne du bras
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, gun.position);
        Gizmos.DrawSphere(gun.position, 0.05f);
    }

}
