using UnityEditor;

namespace GameState
{
    public abstract class GameState
    {
        protected EGameStates _gameState;

        public EGameStates GameState1 => _gameState;

        public abstract void Enable();

        public abstract void OnUpdate();

        public abstract void Disable();
    }
    
    public enum EGameStates
    {
        Fight,
        CardChoose,
        PostFight,
    }
}