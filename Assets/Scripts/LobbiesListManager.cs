using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class LobbiesListManager : MonoBehaviour
{
    public static LobbiesListManager instance;

    // Lobbies List Variables
    public GameObject lobbiesMenu;
    public GameObject lobbyDataItemPrefab;
    public GameObject lobbyListContent;

    public GameObject lobbiesButton, hostFriendsButton, hostPublicButton;
    public GameObject refreshButton;

    public List<GameObject> listOfLobbies = new List<GameObject>();


    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void GetListOfLobbies()
    {
        lobbiesButton.SetActive(false);
        hostFriendsButton.SetActive(false);
        hostPublicButton.SetActive(false);

        lobbiesMenu.SetActive(true);

        SteamLobby.Instance.GetLobbiesList();
    }

    public void BackToMenu()
    {
        lobbiesButton.SetActive(true);
        hostFriendsButton.SetActive(true);
        hostPublicButton.SetActive(true);

        lobbiesMenu.SetActive(false);
    }

    public void RefreshLobbies()
    {
        DestroyLobbies();
        SteamLobby.Instance.GetLobbiesList();
        StartCoroutine(RotateSmooth(refreshButton, 270f, 5f));
    }

    public void DisplayLobbies(List<CSteamID> lobbyIDs, LobbyDataUpdate_t result)
    {
        for (int i=0; i < lobbyIDs.Count; i++)
        {
            if (lobbyIDs[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                GameObject createdItem = Instantiate(lobbyDataItemPrefab);

                createdItem.GetComponent<LobbyDataEntry>().lobbyID = (CSteamID)lobbyIDs[i].m_SteamID;
                createdItem.GetComponent<LobbyDataEntry>().lobbyName = SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDs[i].m_SteamID, "name");
                createdItem.GetComponent<LobbyDataEntry>().SetLobbyData();

                createdItem.transform.SetParent(lobbyListContent.transform);
                createdItem.transform.localScale = Vector3.one;

                listOfLobbies.Add(createdItem);
            }
        }
    }

    public void DestroyLobbies()
    {
        foreach(GameObject lobbyItem in listOfLobbies)
        {
            Destroy(lobbyItem);
        }
        listOfLobbies.Clear();
    }

    IEnumerator RotateSmooth(GameObject objectToRotate, float rotationAngle, float speed)
    {
        float totalRotation = 0f;
        while (totalRotation < rotationAngle)
        {
            float rotationStep = 180f * Time.deltaTime * speed;
            objectToRotate.transform.Rotate(0f, 0f, rotationStep);
            totalRotation += rotationStep;
            yield return null;
        }
    }
}
