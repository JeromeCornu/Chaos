using System.Collections;
using Mirror;
using UnityEngine;

namespace GameState
{
    public class PreGameState : GameState
    {
        public PreGameState(GameManager manager) : base( manager )
        {
            _gameState = EGameStates.PreGame;
        }

        public override void Enable()
        {
            _gameManager.StartCoroutine(Countdown());
            LobbyController.Instance.LocalPlayerObject.GetComponent<PlayerMovementController>().input.Disable();
        }

        private IEnumerator Countdown()
        {
            yield return new WaitForSeconds(3);
            GameManager.Instance.GoToNextState();
        }
        
        public override void OnUpdate()
        {
            
        }

        public override void Disable()
        {
            LobbyController.Instance.LocalPlayerObject.GetComponent<PlayerMovementController>().input.Enable();
        }
    }
}