using UnityEngine;
using Mirror;
using System.Collections;

public class PlayerCombatController : NetworkBehaviour
{
    public Gun gun;

    [SyncVar(hook = nameof(OnAmmoChanged))]
    private int syncedAmmo;

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
                syncedAmmo = gun.GetCurrentAmmo();
                CmdShoot(pos, rot);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ReloadAndSync());
        }
    }

    private void OnAmmoChanged(int oldValue, int newValue)
    {
        gun?.UpdateAmmoDisplay(newValue);
    }


    private IEnumerator ReloadAndSync()
    {
        yield return StartCoroutine(gun.Reload());
        syncedAmmo = gun.GetCurrentAmmo();
    }


    [Command]
    private void CmdShoot(Vector3 position, Quaternion rotation)
    {
        if (gun != null)
            gun.ShootBullet(position, rotation);
    }

}
