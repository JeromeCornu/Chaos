using DefaultNamespace.UI;
using Mirror;

namespace GameState
{
    public class MapEditing : GameState
    {
        [SyncVar] public ulong SelectedPlayerSteamID;
        
        public MapEditing(GameManager manager) : base( manager )
        {
            _gameState = EGameStates.MapEditing;
        }

        public bool CanInteract()
        {
            LobbyController.Instance.LocalPlayerObject.TryGetComponent( out PlayerObjectController playerObjectController);
            return playerObjectController.PlayerSteamID == SelectedPlayerSteamID;
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