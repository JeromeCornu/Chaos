using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerMovementController : NetworkBehaviour
{
    public float Speed = 0.1f;
    public GameObject PlayerModel;

    public SpriteRenderer playerMesh;
    public Material[] playerColors;

    private void Start()
    {
        PlayerModel.SetActive(false);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if(PlayerModel.activeSelf == false)
            {
                SetPosition();
                PlayerModel.SetActive(true);
                PlayerCosmeticsSetup();
            }

            if(hasAuthority) // check if its ur player
            {
                Movement();
            }

        }
    }

    public void SetPosition() // spawn player
    {
        transform.position = new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
    }


    public void Movement()
    {
        float xDirection = Input.GetAxis("Horizontal");
        float yDirection = Input.GetAxis("Vertical");

        Vector2 moveDirection = new Vector2(xDirection, yDirection);

        transform.position += (Vector3)(moveDirection * Speed * Time.deltaTime);
    }

    public void PlayerCosmeticsSetup()
    {
        playerMesh.material = playerColors[GetComponent<PlayerObjectController>().PlayerColor];
    }
}

