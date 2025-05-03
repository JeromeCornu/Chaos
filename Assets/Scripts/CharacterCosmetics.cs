using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using Mirror;
using TMPro;

public class CharacterCosmetics : MonoBehaviour
{
    public int currentColorIndex = 0;
    public Material[] playerColors;
    public Image currentColorImage;
    public TextMeshProUGUI currentColorText;

    private PlayerObjectController playerObjectController;

    private void Start()
    {
        currentColorIndex = PlayerPrefs.GetInt("currentColorIndex", 0);
        currentColorImage.color = playerColors[currentColorIndex].color;
        currentColorText.text = playerColors[currentColorIndex].name;

        StartCoroutine(FindPlayerObject());
    }

    private IEnumerator FindPlayerObject()
    {
        while (playerObjectController == null)
        {
            foreach (var player in FindObjectsOfType<PlayerObjectController>())
            {
                if (player.hasAuthority)
                {
                    playerObjectController = player;
                    break;
                }
            }
            yield return null;
        }
    }

    public void NextColor()
    {
        currentColorIndex = (currentColorIndex + 1) % playerColors.Length;
        UpdateColor();
    }

    public void PreviousColor()
    {
        currentColorIndex = (currentColorIndex - 1 + playerColors.Length) % playerColors.Length;
        UpdateColor();
    }

    private void UpdateColor()
    {
        PlayerPrefs.SetInt("currentColorIndex", currentColorIndex);
        currentColorImage.color = playerColors[currentColorIndex].color;
        currentColorText.text = playerColors[currentColorIndex].name;

        if (playerObjectController != null)
        {
            playerObjectController.CmdUpdatePlayerColor(currentColorIndex);
        }
    }

}
