using UnityEngine;
using Steamworks;
using FishNet.Transporting;
using FishNet.Managing;

public class SteamLobbyManager : MonoBehaviour
{

    public static SteamLobbyManager Instance;

    private const int MaxPlayers = 2; // 1v1 for the moment
    private Callback<LobbyCreated_t> lobbyCreated;
    private Callback<GameLobbyJoinRequested_t> lobbyJoinRequest;
    private Callback<LobbyEnter_t> lobbyEntered;
    private Callback<LobbyChatUpdate_t> lobbyChatUpdate;
    private CSteamID currentLobbyID;

    private NetworkManager networkManager;

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

    private void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();

        if (SteamManager.Initialized)
        {
            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            lobbyJoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
            lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
        }
        else
        {
            Debug.LogError("SteamManager not initialized !");
        }
    }

    public void OpenSteamFriends()
    {
        if (SteamManager.Initialized)
        {
            SteamFriends.ActivateGameOverlay("Friends");
        }
    }

    private bool isLobbyCreating = false;

    public void HostLobby()
    {
        if (isLobbyCreating)
        {
            Debug.LogWarning("[SteamLobbyManager] Already creating a lobby!");
            return;
        }

        isLobbyCreating = true;
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, MaxPlayers);
        SteamMatchmaking.SetLobbyData(currentLobbyID, "MaxPlayers", MaxPlayers.ToString());
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Failed to create lobby");
            isLobbyCreating = false;
            return;
        }

        Debug.Log("Lobby created successfully");

        currentLobbyID = new CSteamID(callback.m_ulSteamIDLobby);

        // Set connection info for clients
        SteamMatchmaking.SetLobbyData(currentLobbyID, "HostAddress", SteamUser.GetSteamID().ToString());

        // Start hosting on FishNet
        networkManager.ServerManager.StartConnection();
    }

    private void OnLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Joining lobby...");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        currentLobbyID = new CSteamID(callback.m_ulSteamIDLobby);

        // If not host, connect as client
        if (!SteamMatchmaking.GetLobbyOwner(currentLobbyID).Equals(SteamUser.GetSteamID()))
        {
            string hostAddress = SteamMatchmaking.GetLobbyData(currentLobbyID, "HostAddress");
            Debug.Log("Connecting to host: " + hostAddress);

            // Connect to the host through FishNet (local connect because Steam will handle P2P)
            networkManager.ClientManager.StartConnection();
        }
    }

    private void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
    {
        CSteamID userChanged = new CSteamID(callback.m_ulSteamIDUserChanged);
        CSteamID userActedOn = new CSteamID(callback.m_ulSteamIDMakingChange);

        EChatMemberStateChange stateChange = (EChatMemberStateChange)callback.m_rgfChatMemberStateChange;

        Debug.Log($"[SteamLobbyManager] LobbyChatUpdate: {userChanged} | StateChange: {stateChange}");

        // if quit is on purpose
        if (stateChange == EChatMemberStateChange.k_EChatMemberStateChangeLeft ||
            stateChange == EChatMemberStateChange.k_EChatMemberStateChangeDisconnected ||
            stateChange == EChatMemberStateChange.k_EChatMemberStateChangeKicked ||
            stateChange == EChatMemberStateChange.k_EChatMemberStateChangeBanned)
        {
            Debug.Log($"[SteamLobbyManager] Player {userChanged} left or was removed from lobby.");

            // is it the host?
            if (userChanged == SteamMatchmaking.GetLobbyOwner(currentLobbyID))
            {
                Debug.LogWarning("[SteamLobbyManager] Host has left the lobby. Leaving lobby...");

                // Le host est parti, on force le leave
                ForceLeaveLobby();
            }

            // TODO force refresh UI
        }

        // player arrived
        if (stateChange == EChatMemberStateChange.k_EChatMemberStateChangeEntered)
        {
            Debug.Log($"[SteamLobbyManager] Player {userChanged} entered the lobby.");

            // TODO force refresh ui
        }
    }

    private void ForceLeaveLobby()
    {
        Debug.Log("[SteamLobbyManager] Forcing leave of lobby.");

        if (SteamManager.Initialized)
        {
            SteamMatchmaking.LeaveLobby(currentLobbyID);
        }

        if (networkManager != null)
        {
            if (networkManager.IsServerStarted)
                networkManager.ServerManager.StopConnection(true);

            if (networkManager.IsClientStarted)
                networkManager.ClientManager.StopConnection();
        }

        // get back to first scene
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

    }


}
