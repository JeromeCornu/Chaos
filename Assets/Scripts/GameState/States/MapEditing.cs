using System.Collections.Generic;
using DefaultNamespace.UI;
using Mirror;

namespace GameState
{
    public class MapEditing : GameState
    {
        [SyncVar] public List<ulong> PlayerSteamIDs = new List<ulong>();
        
        [SyncVar(hook = nameof(playerOneIsDone))]
        private bool _isSelectedPlayerDone;
        
        public MapEditing(GameManager manager) : base( manager )
        {
            _gameState = EGameStates.MapEditing;
        }

        public bool CanInteract()
        {
            LobbyController.Instance.LocalPlayerObject.TryGetComponent( out PlayerObjectController playerObjectController);
            return playerObjectController.PlayerSteamID == (_isSelectedPlayerDone ? PlayerSteamIDs[1] : PlayerSteamIDs[0]);
        }

        [Command]
        public void playerOneIsDone()
        {
            if (_isSelectedPlayerDone)
            {
                _gameManager.GoToNextState();
            }
            
            _isSelectedPlayerDone = true;
            UIManager.Instance.ShowMapObstacleRPC(CanInteract());
        }

        public override void Enable()
        {
            if(!_gameManager.isServer) return;
            UIManager.Instance.ShowMapObstacleRPC(CanInteract());
        }

        public override void OnUpdate()
        {
            
        }

        public override void Disable()
        {
            
        }
    }
}