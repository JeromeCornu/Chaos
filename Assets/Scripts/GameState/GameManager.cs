using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DefaultNamespace;
using Mirror;
using UnityEngine;

namespace GameState
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance;

        private Dictionary<EGameStates, GameState> _gameStates = new();
        public List<EGameStates> GameLoop { get; private set; }

        private int _currentIndex = 0;
        public EGameStates CurrentStateKey => GameLoop[_currentIndex];
        public GameState CurrentGameState => _gameStates[CurrentStateKey];

        private bool _isInitialized; 

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Init()
        {
            _gameStates = new Dictionary<EGameStates, GameState>
            {
                { EGameStates.PreGame, new PreGameState(this) },
                { EGameStates.Fight, new FightState(this) },
                { EGameStates.CardChoose, new ChoosePowerUp(this) },
                { EGameStates.MapEditing, new FightState(this) }
            };

            GameLoop = new List<EGameStates>
            {
                EGameStates.PreGame,
                EGameStates.Fight,
                EGameStates.CardChoose,
                EGameStates.MapEditing
            };

            _currentIndex = 0;
            
            _isInitialized = true;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            StartCoroutine(NotifyServerReady());
        }
        
        private IEnumerator NotifyServerReady()
        {
            yield return null;
            LobbyController.Instance.LocalPlayerObject.GetComponent<ClientToServerComands>().NotifyServerReady();
        }

        public void StartStateMachine()
        {
            Init();
            RpcChangeState(GameLoop[_currentIndex]);
        }

        public void GoToNextState()
        { 
            RpcChangeState(GetNextStateKey());
        }

        public void GoToPreviousState()
        {
            RpcChangeState(GetPreviousStateKey());
        }

        private EGameStates GetNextStateKey() =>
            GameLoop[(_currentIndex + 1) % GameLoop.Count];

        public GameState GetNextGameState() =>
            _gameStates[GetNextStateKey()];

        private EGameStates GetPreviousStateKey() =>
            GameLoop[(_currentIndex - 1 + GameLoop.Count) % GameLoop.Count];

        public GameState GetPreviousGameState() =>
            _gameStates[GetPreviousStateKey()];

        [ClientRpc]
        private void RpcChangeState(EGameStates newState)
        {
            CurrentGameState.Disable();
            _currentIndex = GameLoop.IndexOf(newState);
            CurrentGameState.Enable();
        }

        private void Update()
        {
            if (_isInitialized && _gameStates.ContainsKey(CurrentStateKey))
            {
                _gameStates[CurrentStateKey].OnUpdate();
            }
        }
    }
}