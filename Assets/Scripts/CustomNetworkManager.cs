using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;
using UnityEngine.Serialization;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController GamePlayerPrefab;
    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();
    [SerializeField] private NetworkIdentity gameManager;

    [Header("Custom Prefabs")]
    public GameObject gameManagerPrefab;

    private GameObject gameManagerInstance;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            PlayerObjectController GamePlayerInstance = Instantiate(GamePlayerPrefab);
            GamePlayerInstance.ConnectionID = conn.connectionId;
            GamePlayerInstance.PlayerIdNumber = GamePlayers.Count + 1;
            GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.CurrentLobbyID, GamePlayers.Count);

            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
        }
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        
        // Check if this is the gameplay scene
        if (sceneName == "Game")
        {
            // Spawn GameManager only once
            if (gameManagerInstance == null)
            {
                gameManagerInstance = Instantiate(gameManagerPrefab);
                NetworkServer.Spawn(gameManagerInstance);
            }
        }
    }
    
    public void StartGame(string SceneName)
    {
        ServerChangeScene(SceneName);
    }
    
    public override void OnStopServer()
    {
        base.OnStopServer();
        gameManagerInstance = null;
    }

}
