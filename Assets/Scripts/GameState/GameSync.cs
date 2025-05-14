using System.Collections.Generic;
using GameState;
using Mirror;
using UnityEngine;

public class GameSync : NetworkBehaviour
{
    public static GameSync Instance;
    
    private readonly HashSet<NetworkConnectionToClient> readyPlayers = new();

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
    
    public void PlayerReady(NetworkConnection conn)
    {
        if (!isServer) return;

        if (conn is NetworkConnectionToClient connToClient)
        {
            if (readyPlayers.Contains(connToClient)) return;

            readyPlayers.Add(connToClient);

            Debug.Log($"Player {connToClient.connectionId} is ready. ({readyPlayers.Count}/{NetworkServer.connections.Count})");

            if (readyPlayers.Count == NetworkServer.connections.Count)
            {
                Debug.Log("All players are ready. Starting game...");
                StartGame();
            }
        }
        else
        {
            Debug.LogWarning("Connection is not a NetworkConnectionToClient. Ignoring.");
        }
    }

    private void StartGame()
    {
        Debug.Log("Game started!");
        
        GameManager.Instance.StartStateMachine();
    }
}