using Unity.VisualScripting;
using UnityEngine;

namespace GameState
{
    public class FightState : GameState
    {

        public GameObject Winner;
        
        public FightState() 
        {
            _gameState = EGameStates.Fight;
        }

        public override void Enable()
        {
            Winner.gameObject.GetComponent<PlayerMovementController>();
        }

        public override void OnUpdate()
        {
            
        }

        public override void Disable()
        {
            
        }
    }
}