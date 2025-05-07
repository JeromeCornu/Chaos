using System.Collections;
using UnityEngine;

namespace GameState
{
    public class PreGameState : GameState
    {
        public PreGameState()
        {
            _gameState = EGameStates.PreGame;
        }

        public override void Enable()
        {
            StartCoroutine(Countdown());
        }

        private IEnumerator Countdown()
        {
            yield return new WaitForSeconds(3);
        }
        
        public override void OnUpdate()
        {
            
        }

        public override void Disable()
        {
            
        }
    }
}