using System.Collections;
using System.Collections.Generic;
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

        [SyncVar] private uint cardChooserNetId;
        private int currentCardIndex = 0;
        private int cardCount = 0;

        public uint CardChooserNetId => cardChooserNetId;

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
            LobbyController.Instance.LocalPlayerObject
                .GetComponent<ClientToServerComands>()
                .NotifyServerReady();
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

        // card selection UI

        public void TriggerCardSelectionForPlayer(uint loserNetId)
        {
            cardChooserNetId = loserNetId;
            currentCardIndex = 0;

            if (CardNavigationUI.Instance != null)
                cardCount = CardNavigationUI.Instance.TotalCards;

            RpcShowCardSelectionUI(loserNetId, currentCardIndex);
        }

        [ClientRpc]
        private void RpcShowCardSelectionUI(uint interactorNetId, int highlightIndex)
        {
            if (CardNavigationUI.Instance != null)
            {
                CardNavigationUI.Instance.Show(interactorNetId);
                CardNavigationUI.Instance.HighlightCard(highlightIndex);
            }
        }

        public void MoveCardCursor(int direction)
        {
            if (cardCount == 0 && CardNavigationUI.Instance != null)
                cardCount = CardNavigationUI.Instance.TotalCards;

            currentCardIndex = (currentCardIndex + direction + cardCount) % cardCount;
            RpcHighlightCard(currentCardIndex);
        }

        [ClientRpc]
        private void RpcHighlightCard(int index)
        {
            if (CardNavigationUI.Instance != null)
                CardNavigationUI.Instance.HighlightCard(index);
        }

        public void GoToCardChoosePhase(uint loserNetId)
        {
            _currentIndex = GameLoop.IndexOf(EGameStates.CardChoose);
            cardChooserNetId = loserNetId;

            RpcChangeState_Server(EGameStates.CardChoose); // update all clients
            RpcShowCardSelectionUI(loserNetId, 0); 
        }

    }
}
