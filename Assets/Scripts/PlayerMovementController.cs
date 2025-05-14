using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Wall Check")]
    public Transform wallCheckLeft;
    public Transform wallCheckRight;
    public float wallCheckRadius = 0.1f;
    private bool isTouchingWall;
    public float wallJumpForceMultiplier = 0.7f;
    public float maxWallSlideSpeed = -3f;

    [Header("Cosmetics")]
    public GameObject PlayerModel;
    [SerializeField] private PlayerCosmetics playerCosmetics;

    private Rigidbody2D rb;
    private bool isGrounded;
    public Inputs input; 
    private CircleCollider2D playerCollider;
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    private void OnEnable()
    {
        input = new Inputs();
        input.Enable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PlayerModel.SetActive(false);

        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CircleCollider2D>();
        PlayerModel.SetActive(false);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (!PlayerModel.activeSelf)
            {
                SetPosition();
                PlayerModel.SetActive(true);
                playerCosmetics.PlayerCosmeticsSetup();
            }

            if (hasAuthority)
            {
                HandleGroundCheck();
                HandleWallCheck();
                Movement();
            }
        }
    }

    private void HandleGroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;
    }

    private void HandleWallCheck()
    {
        bool left = Physics2D.OverlapCircle(wallCheckLeft.position, wallCheckRadius, groundLayer);
        bool right = Physics2D.OverlapCircle(wallCheckRight.position, wallCheckRadius, groundLayer);
        isTouchingWall = left || right;
    }


    public void SetPosition()
    {
        int index = GetComponent<PlayerObjectController>().PlayerIdNumber;

        Vector2 spawnPos = SpawnManager.instance.GetSpawnPosition(index);
        transform.position = spawnPos;
    }

    private void Movement()
    {
        float moveInput = input.PlayerInputs.LeftRight.ReadValue<float>();
        float velocityX = moveInput * moveSpeed;

        float boxHeight = playerCollider.bounds.size.y * 0.7f;
        Vector2 boxSize = new Vector2(0.1f, boxHeight);

        // prohibit to push yourself to a wall if stuck
        bool pushingAgainstLeftWall = (moveInput < 0 && Physics2D.OverlapBox(wallCheckLeft.position, boxSize, 0f, groundLayer));
        bool pushingAgainstRightWall = (moveInput > 0 && Physics2D.OverlapBox(wallCheckRight.position, boxSize, 0f, groundLayer));


        if (pushingAgainstLeftWall || pushingAgainstRightWall)
        {
            velocityX = 0f;
        }
        

        rb.velocity = new Vector2(velocityX, rb.velocity.y);

        // Jump on ground OR on wall
        if (input.PlayerInputs.Jump.triggered && (coyoteTimeCounter > 0f || isTouchingWall))
        {
            float appliedJumpForce = jumpForce;

            // on wall but not touching ground : jump less effective
            if (!isGrounded && isTouchingWall)
            {
                appliedJumpForce *= wallJumpForceMultiplier;
            }

            rb.velocity = new Vector2(rb.velocity.x, appliedJumpForce);
            coyoteTimeCounter = 0f;
        }


        // limited slide on wall (slide instead of fall)
        if (isTouchingWall && rb.velocity.y < maxWallSlideSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, maxWallSlideSpeed);
        }
    }

    private void OnDisable()
    {
        input.Disable();
    }


    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (wallCheckLeft != null)
        {
            Gizmos.color = Color.blue;

            float boxHeight = 2f; // default in case
#if UNITY_EDITOR
            CircleCollider2D col = GetComponent<CircleCollider2D>();
            if (col != null)
            {
                boxHeight = col.bounds.size.y * 0.7f;
            }
#endif
            Vector2 boxSize = new Vector2(0.1f, boxHeight);
            Gizmos.DrawWireCube(wallCheckLeft.position, boxSize);
        }

        if (wallCheckRight != null)
        {
            Gizmos.color = Color.blue;

            float boxHeight = 2f; // default in case
#if UNITY_EDITOR
            CircleCollider2D col = GetComponent<CircleCollider2D>();
            if (col != null)
            {
                boxHeight = col.bounds.size.y * 0.7f;
            }
#endif
            Vector2 boxSize = new Vector2(0.1f, boxHeight);
            Gizmos.DrawWireCube(wallCheckRight.position, boxSize);
        }
    }


}
