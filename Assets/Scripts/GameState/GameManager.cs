using System.Collections;
using System.Collections.Generic;
using Mirror;

namespace GameState
{
    public class GameManager : NetworkBehaviour
    {
        private EGameStates _gameState;
        private Dictionary<EGameStates, GameState> _gameStates = new ();
        List<PlayerMovementController> _players;
        
        public static GameManager Instance;
        public GameState GetGameState => _gameStates[_gameState];

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Init()
        {
            _gameStates[EGameStates.Fight] = new FightState();
            _gameStates[EGameStates.CardChoose] = new ChoosePowerUp();
            _gameStates[EGameStates.MapEditing] = new FightState();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (isLocalPlayer)
            {
                StartCoroutine(NotifyServerReady());
            }
        }
        
        public void StartStateMachine()
        {
            Init();
            if(isServer)
                RpcChangeState(EGameStates.PreGame);
        }
        
        private IEnumerator NotifyServerReady()
        {
            yield return null;
            CmdPlayerSceneReady();
        }
        
        [Command]
        private void CmdPlayerSceneReady()
        {
            GameSync.Instance.PlayerReady(connectionToClient);
        }
                
        [ClientRpc] 
        private void RpcChangeState(EGameStates state) 
        {
            _gameStates[_gameState].Disable();
            _gameState = state;
            _gameStates[_gameState].Enable();
        }
        void Update()
        {
            _gameStates[_gameState].OnUpdate();
        }
    }
}



