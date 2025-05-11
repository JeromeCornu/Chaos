using UnityEngine;
using Mirror;

public class PlayerCombatController : NetworkBehaviour
{
    public Gun gun;

    private void Update()
    {
        if (!hasAuthority || gun == null) return;

        HandleFireInput();
    }

    private void HandleFireInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (gun.TryFireLocal(out Vector3 pos, out Quaternion rot))
            {
                CmdShoot(pos, rot);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            gun.StartReload();
        }
    }


    [Command]
    private void CmdShoot(Vector3 position, Quaternion rotation)
    {
        Debug.Log("CmdShoot() called on server");

        if (gun != null)
            gun.ShootBullet(position, rotation);
    }

}
