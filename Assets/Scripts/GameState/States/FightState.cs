using Unity.VisualScripting;
using UnityEngine;

namespace GameState
{
    public class FightState : GameState
    {

        public GameObject Winner;
        
        public FightState(GameManager manager) : base( manager )
        {
            _gameState = EGameStates.Fight;
        }

        public override void Enable()
        {
            var playerMovementController = LobbyController.Instance.LocalPlayerObject.GetComponent<PlayerMovementController>();
            playerMovementController.input.Enable();
            playerMovementController.FreezePlayer(false);
        }

        public override void OnUpdate()
        {
            
        }

        public override void Disable()
        {
            
        }
    }
}