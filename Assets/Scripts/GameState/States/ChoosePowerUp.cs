
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
            if (GameManager.Instance.isServer)
            {
                GameManager.Instance.GenerateCardChoices();
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