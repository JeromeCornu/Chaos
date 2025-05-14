using UnityEditor;
using UnityEngine;

namespace GameState
{
    public abstract class GameState
    {
        protected EGameStates _gameState;
        protected GameManager _gameManager;

        protected GameState(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public EGameStates GameState1 => _gameState;

        public abstract void Enable();

        public abstract void OnUpdate();

        public abstract void Disable();
    }
    
    public enum EGameStates
    {
        PreGame,
        Fight,
        CardChoose,
        MapEditing,
    }
}