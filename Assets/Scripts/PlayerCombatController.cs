using UnityEngine;
using Mirror;
using System.Collections;

public class PlayerCombatController : NetworkBehaviour
{
    public Gun gun;

    [SyncVar(hook = nameof(OnAmmoChanged))]
    private int syncedAmmo;

    [SyncVar(hook = nameof(OnReloadingChanged))]
    private bool isReloading;

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
    }

    private void OnAmmoChanged(int oldValue, int newValue)
    {
        gun?.UpdateAmmoDisplay(newValue);
    }

    private void OnReloadingChanged(bool oldValue, bool newValue)
    {
        if (gun != null)
        {
            if (gun.GetCurrentAmmo() == 0)
                gun.SetReloadUIVisible(newValue);
            else
                gun.SetReloadUIVisible(false); // keep bullet icons visible
        }
    }

    [Command]
    public void CmdUpdateAmmo(int value)
    {
        syncedAmmo = value;
    }

    [Command]
    public void CmdSetReloading(bool value)
    {
        isReloading = value;
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
