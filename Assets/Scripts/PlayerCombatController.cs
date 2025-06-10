using System;
using UnityEngine;
using Mirror;
using System.Collections;
using UnityEngine.InputSystem;
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
    
    public bool canAim = true;
    
    private PlayerMovementController movement;
    private string controlScheme = "";

    private void OnEnable()
    {
        TryGetComponent(out movement);
    }

    private void Update()
    {
        if(!canAim)return;

        if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
        {
            controlScheme = "PlayerGamepad";
        }

        if (Keyboard.current.wasUpdatedThisFrame || Mouse.current.wasUpdatedThisFrame)
        {
            controlScheme = "PlayerKeyboard";
        }

        if (!hasAuthority || gun == null) return;

        HandleFireInput();

        Vector2 dir = default;
        
        //Mouse and keyboard
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = 0f;
        //Gamepad

        Vector2 playerCameraPos = Camera.main.WorldToScreenPoint(transform.position);

        Vector3 WorldPosGamepad =
            Camera.main.ScreenToWorldPoint(playerCameraPos + movement.input.PlayerInputs.Direction.ReadValue<Vector2>());
        
        //Choose dir according to control scheme
        dir = controlScheme == "PlayerKeyboard" ? 
            (mouseWorld - transform.position).normalized: 
            (WorldPosGamepad - transform.position).normalized;
        
        CmdSendAimDirection(dir);

    }

    private void HandleFireInput()
    {
        if (movement.input.PlayerInputs.Shoot.triggered)
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
