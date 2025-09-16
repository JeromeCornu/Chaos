using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GameState
{
    public class FightState : GameState
    {
        
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
            if(!_gameManager.isServer) return;
            
            var networkManager = CustomNetworkManager.singleton as CustomNetworkManager;

            if (networkManager == null) return;
            List<PlayerObjectController> playerList = networkManager.GamePlayers;

            foreach ( PlayerObjectController player in playerList)
            {
                player.TryGetComponent(out Health health);
                if (health.currentHealth <= 0)
                {
                    ((ChoosePowerUp)_gameManager.GameStates[EGameStates.CardChoose]).SelectedPlayerSteamID =
                        player.PlayerSteamID;
                    ((MapEditing)_gameManager.GameStates[EGameStates.MapEditing]).PlayerSteamIDs.Insert(0, player.PlayerSteamID);
                }
                else
                {
                    ((MapEditing)_gameManager.GameStates[EGameStates.MapEditing]).PlayerSteamIDs.Add(player.PlayerSteamID);
                }
            }
        }
    }
}