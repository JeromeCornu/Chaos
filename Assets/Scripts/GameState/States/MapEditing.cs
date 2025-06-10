namespace GameState
{
    public class MapEditing : GameState
    {
        public MapEditing(GameManager manager) : base( manager )
        {
            _gameState = EGameStates.MapEditing;
        }

        public override void Enable()
        {
            LobbyController.Instance.LocalPlayerObject.GetComponent<PlayerMovementController>().canMove = false;
            LobbyController.Instance.LocalPlayerObject.GetComponent<PlayerCombatController>().canAim = false;
        }

        public override void OnUpdate()
        {
            
        }

        public override void Disable()
        {
            
        }
    }
}