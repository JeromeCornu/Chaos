using System.Collections;
using Mirror;
using UnityEngine;

namespace GameState
{
    public class PreGameState : GameState
    {
        GameObject player;
        
        public PreGameState(GameManager manager) : base( manager )
        {
            _gameState = EGameStates.PreGame;
        }

        public override void Enable()
        {
            Debug.Log("Start pre game state");
            _gameManager.StartCoroutine(Countdown());
            player = LobbyController.Instance.LocalPlayerObject;
            
            PlayerMovementController playerMovementController = player.GetComponent<PlayerMovementController>();
            
            playerMovementController.input.Disable();
            playerMovementController.SetPosition();
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
            Debug.Log("End pre game state");
            LobbyController.Instance.LocalPlayerObject.GetComponent<PlayerMovementController>().input.Enable();
        }
    }
}