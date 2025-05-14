using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;

public class PlayerObjectController : NetworkBehaviour
{
    // Player data
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIdNumber;
    [SyncVar] public ulong PlayerSteamID;
    // "hook" = when the var changes, the function "PlayerNameUpdate" is called
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool Ready;

    // Cosmetics
    [SyncVar(hook = nameof(SendPlayerColor))] public int PlayerColor;


    private CustomNetworkManager manager;
    private CustomNetworkManager Manager
    {
        get
        {
            if(manager != null)
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }


    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }


    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        CmdSetSteamID(SteamUser.GetSteamID().m_SteamID);

        gameObject.name = "LocalGamePlayer";
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();

        int savedColor = PlayerPrefs.GetInt("currentColorIndex", 0);
        CmdUpdatePlayerColor(savedColor);
    }

    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);

        if (LobbyController.Instance != null && SceneManager.GetActiveScene().name == "Lobby")
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    [Command]
    private void CmdSetPlayerName(string PlayerName)
    {
        this.PlayerNameUpdate(this.PlayerName, PlayerName);
    }

    [Command]
    private void CmdSetSteamID(ulong steamID)
    {
        PlayerSteamID = steamID;
    }

    public void PlayerNameUpdate(string OldValue, string NewValue)
    {
        if (isServer) // host
        {
            this.PlayerName = NewValue;
        }
        if (isClient) // client
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }


    private void PlayerReadyUpdate(bool OldValue, bool NewValue)
    {
        if (isServer)
        {
            this.Ready = NewValue;
        }
        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    [Command]
    private void CmdSetPlayerReady()
    {
        this.PlayerReadyUpdate(this.Ready, !this.Ready);
    }

    public void ChangeReady()
    {
        if (hasAuthority)
        {
            CmdSetPlayerReady();
        }
    }

    // ----- Start Game -----
    
    // host call this, then call command
    public void CanStartGame(string SceneName)
    {
        if(hasAuthority)
        {
            CmdCanStartGame(SceneName);
        }
    }

    [Command] // run on every client
    public void CmdCanStartGame(string SceneName)
    {
        manager.StartGame(SceneName);
    }


    // ----- Cosmetics -----

    [Command]
    public void CmdUpdatePlayerColor(int newValue)
    {
        SendPlayerColor(PlayerColor, newValue);
    }

    public void SendPlayerColor(int oldValue, int newValue)
    {
        if(isServer)
        {
            PlayerColor = newValue;
        }
        if (isClient && (oldValue != newValue))
        {
            UpdateColor(newValue);
        }
    }

    private void UpdateColor(int message)
    {
        PlayerColor = message;
    }

}
