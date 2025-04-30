using UnityEngine;
using FishNet.Object;
using Steamworks;
using System.Collections;

public class LobbyPlayerData : NetworkBehaviour
{
    public static event System.Action<LobbyPlayerData> OnPlayerSpawned;
    public static event System.Action<LobbyPlayerData> OnLobbyDataChanged;


    public string playerName;
    public bool isReady;
    public Texture2D playerAvatar;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner)
        {
            playerName = SteamManager.Initialized ? SteamFriends.GetPersonaName() : "Player " + Random.Range(1, 1000);
            int avatarInt = SteamFriends.GetLargeFriendAvatar(SteamUser.GetSteamID());
            playerAvatar = GetSteamImageAsTexture2D(avatarInt);
        }

        OnLobbyDataChanged?.Invoke(this);

        // player just spawned
        OnPlayerSpawned?.Invoke(this);
    }


    public void SetReady(bool readyState)
    {
        Debug.Log("SetReady envoyé au serveur !");

        if (!IsSpawned)
        {
            Debug.LogWarning("Player not yet spawned, delaying ready call...");
            StartCoroutine(DelayedSetReady(readyState));
            return;
        }

        ServerSetReady(readyState);
    }

    private IEnumerator DelayedSetReady(bool readyState)
    {
        // wait until the object is spawned
        yield return new WaitUntil(() => IsSpawned);

        Debug.Log("Now spawned, sending ServerSetReady");
        ServerSetReady(readyState);
    }

    [ServerRpc]
    private void ServerSetReady(bool readyState)
    {
        isReady = readyState;
        Debug.Log("SetReady reçu sur serveur. Maintenant notify clients.");

        NotifyReadyChange(isReady);
    }

    [ObserversRpc]
    private void NotifyReadyChange(bool readyState)
    {
        isReady = readyState;
        Debug.Log("NotifyReadyChange reçu sur clients. Ready=" + readyState);

        OnLobbyDataChanged?.Invoke(this);
    }


    public Texture2D GetSteamImageAsTexture2D(int iImage)
    {
        if (iImage == -1) return null;

        uint Width, Height;
        if (!SteamUtils.GetImageSize(iImage, out Width, out Height))
            return null;

        byte[] Image = new byte[Width * Height * 4];
        if (!SteamUtils.GetImageRGBA(iImage, Image, (int)(Width * Height * 4)))
            return null;

        Texture2D texture = new Texture2D((int)Width, (int)Height, TextureFormat.RGBA32, false, true);
        texture.LoadRawTextureData(Image);
        texture.Apply();
        return texture;
    }

    public static event System.Action<LobbyPlayerData> OnPlayerDespawned;

    public override void OnStopClient()
    {
        base.OnStopClient();
        OnPlayerDespawned?.Invoke(this);
    }



}
