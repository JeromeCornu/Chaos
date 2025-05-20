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

        public Dictionary<EGameStates, GameState> GameStates => _gameStates;

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
            if (_isInitialized) return;
            
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
            Init();
            StartCoroutine(NotifyServerReady());
        }
        
        private IEnumerator NotifyServerReady()
        {
            yield return new WaitUntil(() => NetworkClient.ready);
            yield return null;
            LobbyController.Instance.LocalPlayerObject.GetComponent<ClientToServerComands>().NotifyServerReady();
        }

        public void StartStateMachine()
        {
            Init();
            ChangeState(GameLoop[_currentIndex]);
        }

        public void GoToNextState()
        { 
            ChangeState(GetNextStateKey());
        }

        public void GoToPreviousState()
        {
            ChangeState(GetPreviousStateKey());
        }

        private EGameStates GetNextStateKey() =>
            GameLoop[(_currentIndex + 1) % GameLoop.Count];

        public GameState GetNextGameState() =>
            _gameStates[GetNextStateKey()];

        private EGameStates GetPreviousStateKey() =>
            GameLoop[(_currentIndex - 1 + GameLoop.Count) % GameLoop.Count];

        public GameState GetPreviousGameState() =>
            _gameStates[GetPreviousStateKey()];

        private void ChangeState(EGameStates newState)
        {
            if (isServer)
            {
                RpcChangeState_Server(newState);
            }
        }
        
        [ClientRpc]
        private void RpcChangeState_Server(EGameStates newState)
        {
            ChangeState_Client(newState);
        }

        private void ChangeState_Client(EGameStates newState)
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