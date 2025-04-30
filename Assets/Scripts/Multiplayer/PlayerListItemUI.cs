using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class PlayerListItemUI : MonoBehaviour
{
    private LobbyPlayerData linkedPlayer;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI readyStatusText;
    public RawImage avatarImage;

    public void Initialize(LobbyPlayerData data)
    {
        linkedPlayer = data;
        UpdateDisplay();
        LobbyPlayerData.OnLobbyDataChanged += OnAnyLobbyDataChanged;
    }

    private void OnDestroy()
    {
        LobbyPlayerData.OnLobbyDataChanged -= OnAnyLobbyDataChanged;
    }

    private void OnAnyLobbyDataChanged(LobbyPlayerData changedPlayer)
    {
        if (linkedPlayer == changedPlayer)
            UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (linkedPlayer != null)
        {
            nameText.text = linkedPlayer.playerName;
            readyStatusText.text = linkedPlayer.isReady ? "Ready" : "Not Ready";
            readyStatusText.color = linkedPlayer.isReady ? Color.green : Color.red;
            if (linkedPlayer.playerAvatar != null)
                avatarImage.texture = linkedPlayer.playerAvatar;
        }
    }
}
