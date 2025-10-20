using System.Collections;
using Mirror;
using UnityEngine;

namespace GameState
{
    public class PreGameState : GameState
    {
        
        private PlayerMovementController playerMovementController;
        
        public PreGameState(GameManager manager) : base( manager )
        {
            _gameState = EGameStates.PreGame;
        }

        public override void Enable()
        {
            Debug.Log("Start pre game state");
            _gameManager.StartCoroutine(Countdown());
            playerMovementController = LobbyController.Instance.LocalPlayerObject.GetComponent<PlayerMovementController>();

            playerMovementController.FreezePlayer(true);
            playerMovementController.canMove = false;
            playerMovementController.SetPosition();
            playerMovementController.gameObject.GetComponent<Health>().HealMaxHealth();
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
            if (playerMovementController == null) return;
            playerMovementController.canMove = true;
            LobbyController.Instance.LocalPlayerObject.GetComponent<PlayerCombatController>().canAim = true;
            playerMovementController.FreezePlayer(false);
        }
    }
}