
using DefaultNamespace.CardSelection;

namespace GameState
{
    public class ChoosePowerUp : GameState
    {
        public ChoosePowerUp(GameManager manager) : base( manager )
        {
            _gameState = EGameStates.CardChoose;
        }

        public override void Enable()
        {
            CardSelectionManager.Instance.Init();
            
            var winnerID = ((FightState)_gameManager.GetPreviousGameState()).Winner;
            
            CardSelectionManager.Instance.RpcShowCardSelectionUI(winnerID, 0);
            
            if (GameManager.Instance.isServer)
            {
                CardSelectionManager.Instance.GenerateCardChoices();
            }
        }

        public override void OnUpdate()
        {
            
        }

        public override void Disable()
        {
            
        }
    }
}