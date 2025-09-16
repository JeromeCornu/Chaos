using DefaultNamespace.CardSelection;
using GameState;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardSelectionHandler : NetworkBehaviour
{
    private Inputs inputs;

    void Awake()
    {
        inputs = new Inputs();
    }

    void OnEnable()
    {
        inputs.Enable();

        inputs.PlayerInputs.LeftRight.performed += OnLeftRightClicked;
        inputs.PlayerInputs.Tab.performed += OnTabClicked;

    }

    private void OnLeftRightClicked(InputAction.CallbackContext ctx)
    {
        if (!isLocalPlayer) return;
        if (Mathf.Abs(ctx.ReadValue<float>()) > 0.5f)
            CmdMoveCursor(Mathf.Sign(ctx.ReadValue<float>()));
    }

    private void OnTabClicked(InputAction.CallbackContext ctx)
    {
        if (!isLocalPlayer) return;
        CmdRequestOpenCardSelection();
    }
    
    void OnDisable()
    {
        inputs.PlayerInputs.LeftRight.performed -= OnLeftRightClicked;
        inputs.PlayerInputs.Tab.performed -= OnTabClicked;
        
        inputs.Disable();
    }

    [Command]
    void CmdRequestOpenCardSelection()
    {
        CardSelectionManager.Instance.TriggerCardSelectionForPlayer(connectionToClient.identity.netId);
    }

    [Command]
    void CmdMoveCursor(float direction)
    {
        CardSelectionManager.Instance.MoveCardCursor((int)direction);
    }
}
