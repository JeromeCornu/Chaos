using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

namespace GameState
{
    public class GameManager : NetworkBehaviour
    {
        private EGameStates _gameState;
        private Dictionary<EGameStates, GameState> _gameStates = new ();
        List<PlayerMovementController> _players;
        public GameState GetGameState => _gameStates[_gameState];

        private void Init()
        {
            _gameStates[EGameStates.Fight] = new FightState();
            _gameStates[EGameStates.CardChoose] = new ChoosePowerUp();
            _gameStates[EGameStates.MapEditing] = new FightState();
        }
        
        [ClientRpc]
        public void RpcChangeState(EGameStates state)
        {
            _gameStates[_gameState].Disable();
            _gameState = state;
            _gameStates[_gameState].Enable();
        }
        private void OnEnable()
        {
            Init();
            
            RpcChangeState(EGameStates.Fight);
        }
        
        void Update()
        {
            _gameStates[_gameState].OnUpdate();
        }
    }
}



