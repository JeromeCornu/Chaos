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
            var temp = LobbyController.Instance.LocalPlayerObject.GetComponent<PlayerMovementController>();
        }

        public override void OnUpdate()
        {
            
        }

        public override void Disable()
        {
            
        }
    }
}