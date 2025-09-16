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
    }

    void OnDisable()
    {
        inputs.Disable();
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            CmdRequestOpenCardSelection();
        }

        float moveInput = inputs.PlayerInputs.LeftRight.ReadValue<float>();
        if (Mathf.Abs(moveInput) > 0.5f)
        {
            CmdMoveCursor(Mathf.Sign(moveInput));
        }
    }

    [Command]
    void CmdRequestOpenCardSelection()
    {
        GameManager.Instance.TriggerCardSelectionForPlayer(connectionToClient.identity.netId);
    }

    [Command]
    void CmdMoveCursor(float direction)
    {
        GameManager.Instance.MoveCardCursor((int)direction);
    }
}
