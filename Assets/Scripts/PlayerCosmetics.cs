using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCosmetics : MonoBehaviour
{
    public SpriteRenderer playerMesh;
    public Material[] playerColors;

    public void PlayerCosmeticsSetup()
    {
        playerMesh.material = playerColors[GetComponent<PlayerObjectController>().PlayerColor];
    }
}
