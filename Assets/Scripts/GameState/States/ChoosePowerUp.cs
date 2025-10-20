
namespace GameState
{
    public class ChoosePowerUp : GameState
    {
        public uint InteractorNetId { get; set; }

        public ChoosePowerUp(GameManager manager) : base( manager )
        {
            _gameState = EGameStates.CardChoose;
        }

        public override void Enable()
        {
            if (GameManager.Instance.isServer)
            {
                CardSelectionHandler.Instance.cardChooserNetId = InteractorNetId;
                CardSelectionHandler.Instance.GenerateCardChoices();
                CardSelectionHandler.Instance.RpcShowCardSelectionUI(); 
            }
            
            LobbyController.Instance.LocalPlayerObject.GetComponent<PlayerMovementController>().canMove = false;
            LobbyController.Instance.LocalPlayerObject.GetComponent<PlayerCombatController>().canAim = false;
            
        }

        public override void OnUpdate()
        {
            
        }

        public override void Disable()
        {
            CardSelectionHandler.Instance.HideCards();
        }
    }
}