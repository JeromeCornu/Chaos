using UnityEngine;
using FishNet;
using FishNet.Managing;
using FishNet.Managing.Server;
using FishNet.Transporting;
using FishNet.Connection;
using Steamworks;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;

    private void OnEnable()
    {
        InstanceFinder.ServerManager.OnRemoteConnectionState += OnRemoteConnectionState;
        InstanceFinder.ServerManager.OnServerConnectionState += OnServerStarted;
    }

    private void OnDisable()
    {
        InstanceFinder.ServerManager.OnRemoteConnectionState -= OnRemoteConnectionState;
        InstanceFinder.ServerManager.OnServerConnectionState -= OnServerStarted;
    }

    private void OnServerStarted(ServerConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            SpawnHostPlayer();
        }
    }

    private void OnRemoteConnectionState(NetworkConnection conn, RemoteConnectionStateArgs args)
    {
        if (args.ConnectionState == RemoteConnectionState.Started)
        {
            SpawnPlayer(conn);
        }
    }

    private void SpawnHostPlayer()
    {
        NetworkConnection hostConnection = InstanceFinder.ClientManager.Connection;
        if (hostConnection == null)
        {
            Debug.LogError("[DEBUG] Host connection not found!");
            return;
        }
        SpawnPlayer(hostConnection);
    }

    private void SpawnPlayer(NetworkConnection conn)
    {
        if (playerPrefab == null)
        {
            Debug.LogError("[DEBUG] No playerPrefab assigned!");
            return;
        }

        GameObject playerInstance = Instantiate(playerPrefab);
        InstanceFinder.ServerManager.Spawn(playerInstance, conn);

        if (conn.ClientId == -1)
        {
            LobbyPlayerData lobbyData = playerInstance.GetComponent<LobbyPlayerData>();
            if (lobbyData != null)
            {
                if (SteamManager.Initialized)
                {
                    lobbyData.playerName = SteamFriends.GetPersonaName();
                    int avatarInt = SteamFriends.GetLargeFriendAvatar(SteamUser.GetSteamID());
                    lobbyData.playerAvatar = lobbyData.GetSteamImageAsTexture2D(avatarInt);

                    Debug.Log($"[DEBUG] Host='{lobbyData.playerName}'");
                }
                else
                {
                    lobbyData.playerName = "Player_" + Random.Range(1000, 9999);
                }

                LobbyUIManager uiManager = FindObjectOfType<LobbyUIManager>();
                if (uiManager != null)
                {
                    uiManager.ForceAddPlayer(lobbyData);
                }
                else
                {
                    Debug.LogError("[DEBUG] No LobbyUIManager found in scene!");
                }
            }
        }
    }
}
