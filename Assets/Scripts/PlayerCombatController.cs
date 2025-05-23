using UnityEngine;
using Mirror;
using System.Collections;
using UnityEngine.InputSystem.XR;

public class PlayerCombatController : NetworkBehaviour
{
    public Gun gun;
    public ArmController armController;

    [SyncVar(hook = nameof(OnAmmoChanged))]
    private int syncedAmmo;

    [SyncVar(hook = nameof(OnReloadingChanged))]
    private bool isReloading;

    [SyncVar(hook = nameof(OnAimDirectionChanged))]
    private Vector2 syncedAimDirection;


    private void Update()
    {
        if (!hasAuthority || gun == null) return;

        HandleFireInput();

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        Vector2 dir = (mouseWorld - transform.position).normalized;
        CmdSendAimDirection(dir);

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

    private void OnAimDirectionChanged(Vector2 oldDir, Vector2 newDir)
    {
        if (armController != null)
            armController.SetAimDirection(newDir);
    }

    public void HandleDeath()
    {
        Debug.Log("You are dead");
        CmdNotifyDeath();
    }

    [Command]
    private void CmdNotifyDeath()
    {
        GameState.GameManager.Instance.GoToCardChoosePhase(netIdentity.netId);
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

    [Command]
    private void CmdSendAimDirection(Vector2 dir)
    {
        syncedAimDirection = dir;
    }


    [Command]
    private void CmdShoot(Vector3 position, Quaternion rotation)
    {
        if (gun != null)
            gun.ShootBullet(position, rotation);
    }

}
