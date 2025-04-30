using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FishNet;
using FishNet.Object;

public class LobbyUIManager : MonoBehaviour
{
    
    public GameObject playerListItemPrefab;
    public Transform playerListContainer;
    public Button startButton;
    public Button readyButton;
    public TextMeshProUGUI readyButtonText;

    private Dictionary<LobbyPlayerData, PlayerListItemUI> playerUIItems = new Dictionary<LobbyPlayerData, PlayerListItemUI>();
    private LobbyPlayerData myPlayerData;

    private void OnEnable()
    {
        LobbyPlayerData.OnPlayerSpawned += AddPlayerToList;
        LobbyPlayerData.OnPlayerDespawned += RemovePlayerFromList;
        LobbyPlayerData.OnLobbyDataChanged += UpdatePlayerList;
    }

    private void OnDisable()
    {
        LobbyPlayerData.OnPlayerSpawned -= AddPlayerToList;
        LobbyPlayerData.OnPlayerDespawned -= RemovePlayerFromList;
        LobbyPlayerData.OnLobbyDataChanged -= UpdatePlayerList;
    }


    private void Start()
    {
        if (!InstanceFinder.NetworkManager.IsServerStarted)
            startButton.gameObject.SetActive(false);
    }

    public void AddPlayerToList(LobbyPlayerData playerData)
    {
        if (playerUIItems.ContainsKey(playerData))
            return;

        GameObject itemGO = Instantiate(playerListItemPrefab, playerListContainer);
        PlayerListItemUI itemUI = itemGO.GetComponent<PlayerListItemUI>();
        if (itemUI != null)
        {
            itemUI.Initialize(playerData);
            playerUIItems.Add(playerData, itemUI);
        }

        // ICI CA NE RENTRE PAS
        if (playerData.IsOwner)
        {
            myPlayerData = playerData;
            Debug.Log("assign player");
        }
    }


    public void UpdatePlayerList(LobbyPlayerData changedPlayer)
    {
        if (playerUIItems.TryGetValue(changedPlayer, out PlayerListItemUI itemUI))
        {
            if (itemUI != null)
            {
                itemUI.Initialize(changedPlayer);
            }
        }
    }



    public void ToggleReady()
    {
        Debug.Log("ToggleReady clicked !");

        if (myPlayerData != null)
        {
            bool newReadyState = !myPlayerData.isReady;
            myPlayerData.SetReady(newReadyState);

            Debug.Log($"SetReady called with {newReadyState}");

            readyButtonText.text = newReadyState ? "Unready" : "Ready ?";
        }
        else
        {
            Debug.LogError("Aucun joueur LobbyPlayerData trouvé pour ce client !");
        }
    }

    public void TryStartGame()
    {
        LobbyPlayerData[] players = FindObjectsOfType<LobbyPlayerData>();
        foreach (LobbyPlayerData player in players)
        {
            if (!player.isReady)
            {
                Debug.LogWarning("Not all players are ready!");
                return;
            }
        }

        Debug.Log("All players ready! Starting game...");
        
        // TODO : Scene loading
    }


    public void ForceAddPlayer(LobbyPlayerData playerData)
    {
        AddPlayerToList(playerData);
        UpdatePlayerList(playerData);
    }


    private void RemovePlayerFromList(LobbyPlayerData playerData)
    {
        if (playerUIItems.TryGetValue(playerData, out PlayerListItemUI itemUI))
        {
            if (itemUI != null)
                Destroy(itemUI.gameObject);

            playerUIItems.Remove(playerData);
        }
    }
}
