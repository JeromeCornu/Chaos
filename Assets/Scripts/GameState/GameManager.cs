using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

namespace GameState
{
    public class GameManager : NetworkBehaviour
    {
        private GameState gameState;
        private Dictionary<EGameStates, GameState> gameStates = new ();

        public GameState GameState => gameState;
        
        List<PlayerMovementController> Players;

        private void Init()
        {
            //TODO : I need to know what phases are there and how many will there be 
                        
            gameStates[EGameStates.Fight] = new FightState();
        }
        
        [ClientRpc]
        public void RpcChangeState(EGameStates states)
        {
            gameState.Disable();
            
            gameState = gameStates[states];
            
            gameState.Enable();
        }
        private void OnEnable()
        {
            Init();
            
            RpcChangeState(EGameStates.Fight);
        }
        
        void Update()
        {
            gameState.OnUpdate();
        }
    }
}



