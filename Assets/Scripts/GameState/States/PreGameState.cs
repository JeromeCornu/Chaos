using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace GameState
{
    public class PreGameState : GameState
    {
        private PlayerMovementController playerMovementController;
        
        private CustomNetworkManager Manager => CustomNetworkManager.singleton as CustomNetworkManager;


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
            LobbyController.Instance.LocalPlayerObject.GetComponent<PlayerCombatController>().canAim = true;
            playerMovementController.SetPosition();
            HealClient();
        }

        [Server]
        private void HealClient()
        {
            foreach (PlayerObjectController playerObjectController in Manager.GamePlayers)
            {
                playerObjectController.gameObject.TryGetComponent(out Health playerHealth);
                playerHealth.HealMaxHealth();
            }
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
            playerMovementController.FreezePlayer(false);
        }
    }
}