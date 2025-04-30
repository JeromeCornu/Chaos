using FishNet.Managing;
using UnityEngine;

public class FishNetStartTest : MonoBehaviour
{
    private void Start()
    {
        NetworkManager nm = FindObjectOfType<NetworkManager>();
        if (nm != null)
        {
            Debug.Log("FishNet est actif !");
            nm.ServerManager.StartConnection(); // Start Host
            nm.ClientManager.StartConnection();
        }
        else
        {
            Debug.LogError("NetworkManager introuvable");
        }
    }
}
