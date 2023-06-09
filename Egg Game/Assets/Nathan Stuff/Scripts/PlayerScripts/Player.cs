using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rigid;
    private CharacterController controller;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float jumpSpeed = 5;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask kingLayer;
    [SerializeField] private bool isGrounded = false;
    private bool wasGrounded = false;
    private const float groundCheckRadius = 0.2f;
    [SerializeField] private float m_RunSpeed = 1;
    private float m_horizontal;
    [SerializeField] private GameObject Weapon;
    [SerializeField] private Collider2D weaponCollider;
    [SerializeField] private Animator m_WeaponAnimator;
    private bool lastDirRight = false;
    private bool attackFinished = false;
    private double weaponTimer = 1.5f;
    [SerializeField] private double internalTimer;
    private bool isAttacking = false;
    [SerializeField] private float weaponCooldown = 0.8f;
    private bool left = false;
    private bool right = false;
    private bool jumped = false;
    private bool attack = false;
    public bool isKing = false;
    public bool isPause = false;
    public bool becameKing = false;
    public bool isPlayer = false;
    public bool isflashing = false;
    private float maxSpeed = 15.0f;
    private int maxHealth = 6;
    private int currentHealth = 6;
    [SerializeField] private Healthbar healthbar;

    public Player[] player;
    public PlayerSaveData playerSaveData;
    public RoundControl roundControl;
    public GameObject BuildManager;
    public GameObject KingGrid;
    public Transform playerSpawnLocation;
    public Transform kingSpawnLocation;
    [SerializeField] private Transform groundCheckCollider;
    [SerializeField] private Transform groundCheckCollider2;

    public int GetMaxHealth() { return maxHealth; }
    public void SetMaxHealth(int value) { maxHealth = value; }
    public int GetCurrentHealth() { return currentHealth; }
    public void SetCurrentHealth(int value) { currentHealth = value; }

    void OnEnable()
    {
        internalTimer = weaponTimer;
    }

    void Start()
    {
        gameManager = GameObject.Find("GameState").GetComponent<GameManager>();
        roundControl = GameObject.Find("RoundControl").GetComponent<RoundControl>();
        kingSpawnLocation = GameObject.Find("KingPoint").transform;
        playerSpawnLocation = GameObject.Find("SpawnPoint").transform;
        animator = GetComponent<Animator>();
        isPlayer = true;
        healthbar.SetMaxHealth(maxHealth);
        BuildManager.SetActive(false);
        KingGrid.SetActive(false);
        weaponCollider.enabled = false;
        rigid = gameObject.GetComponent<Rigidbody2D>();
        controller = gameObject.GetComponent<CharacterController>();
    }

    public void OnMoveLeft(InputAction.CallbackContext context)
    {
        left = context.action.triggered;
    }

    public void OnMoveRight(InputAction.CallbackContext context)
    {
        right = context.action.triggered;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        jumped = context.action.triggered;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        attack = context.action.triggered;
    }

public void OnPause(InputAction.CallbackContext context)
{
    if (context.performed)
    {
        isPause = !isPause;
    }
}

    void Update()
    {
        if (player == null || player.Length == 0)
        {
            InitializePlayerArray();
        }


        if (!gameManager.GetIsPaused()) {
            // Look for the Esc keypress and pause the game if Esc is pressed.
            if (isPause && !gameManager.GetIsPaused()) {
                gameManager.PauseGame();
            }
        } else {
            // If the game is paused and the Esc key is pressed, unpause the game.
            if (!isPause && gameManager.GetIsPaused()) {
                gameManager.UnpauseGame();
            }
        }

        //if(roundControl.timerOn){
            if (!isKing)
            {
                GroundCheck();

                if (isGrounded)
                {
                    animator.SetBool("Landed", true);
                    if (jumped)
                    {
                        animator.SetBool("Landed", false);
                        rigid.velocity = new Vector2(rigid.velocity.x, jumpSpeed);
                        animator.SetTrigger("Jumped");
                    }
                    else
                    {
                        if (Mathf.Abs(rigid.velocity.x) > 0.1f)
                        {
                            animator.SetBool("Running", true);
                        }
                        else if (Mathf.Abs(rigid.velocity.x) <= 0f)
                        {
                            animator.SetBool("Running", false);
                        }
                    }
                }

                wasGrounded = isGrounded;
                
                // Limit maximum speed
                rigid.velocity = new Vector2(Mathf.Clamp(rigid.velocity.x, -maxSpeed, maxSpeed), rigid.velocity.y);

                float targetVelocityX = 0f;
                if (left)
                {
                    targetVelocityX = -m_RunSpeed;
                    if (!lastDirRight)
                    {
                        Flip();
                    }
                }
                else if (right)
                {
                    targetVelocityX = m_RunSpeed;
                    if (lastDirRight)
                    {
                        Flip();
                    }
                }

                // Apply smooth acceleration and deceleration
                float acceleration = left || right ? 0.1f : 0.008f;
                rigid.velocity = new Vector2(Mathf.Lerp(rigid.velocity.x, targetVelocityX, acceleration), rigid.velocity.y);


                if (attack && weaponCooldown == 0.8f && !isAttacking)
                {
                    isAttacking = true;
                    weaponCollider.enabled = true;
                    m_WeaponAnimator.SetTrigger("Swing");
                }
                if (isAttacking)
                {
                    internalTimer -= Time.deltaTime;
                    if (internalTimer <= 0)
                    {
                        weaponCollider.enabled = false;
                        isAttacking = false;
                        internalTimer = weaponTimer;
                        attackFinished = true;
                    }
                }
                if (attackFinished)
                {
                    weaponCooldown -= Time.deltaTime;
                }
                if (weaponCooldown <= 0)
                {
                    weaponCooldown = 0.8f;
                    attackFinished = false;
                }
            }

            if (becameKing)
            {
                rigid.velocity = Vector2.zero;
                transform.position = kingSpawnLocation.position;
            }
            if (isPlayer && roundControl.Respawn)
            {
                rigid.velocity = Vector2.zero;
                transform.position = playerSpawnLocation.position;
            }
        //}
    }

    void InitializePlayerArray()
    {
        List<Player> playerList = new List<Player>();

        for (int i = 1; i <= 6; i++)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player" + i);
            if (playerObject != null)
            {
                Player playerComponent = playerObject.GetComponent<Player>();
                if (playerComponent != null)
                {
                    playerList.Add(playerComponent);
                }
            }
        }

        player = playerList.ToArray();
    }

    void GroundCheck()
    {
        isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, groundLayer);
        Collider2D[] colliders2 = Physics2D.OverlapCircleAll(groundCheckCollider2.position, groundCheckRadius, kingLayer);
        if (colliders.Length > 0 || colliders2.Length > 0)
        {
            isGrounded = true;
        }
    }

    void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
        lastDirRight = !lastDirRight;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("KingPoint"))
        {
            becameKing = true;
            isPlayer = false;
            BuildManager.SetActive(true);
            KingGrid.SetActive(true);

            if (PlayerSaveData.playerNumber == 1 && player[0].isKing)
            {
                player[0].isKing = false;
                player[0].isPlayer = true;
            }

            if (PlayerSaveData.playerNumber == 2 && (player[0].isKing || player[1].isKing))
            {
                player[0].isKing = false;
                player[0].isPlayer = true;

                player[1].isKing = false;
                player[1].isPlayer = true;
            }

            if (PlayerSaveData.playerNumber == 3 && (player[0].isKing || player[1].isKing || player[2].isKing))
            {
                player[0].isKing = false;
                player[0].isPlayer = true;

                player[1].isKing = false;
                player[1].isPlayer = true;

                player[2].isKing = false;
                player[2].isPlayer = true;
            }

            if (PlayerSaveData.playerNumber == 4 && (player[0].isKing || player[1].isKing || player[2].isKing || player[3].isKing))
            {
                player[0].isKing = false;
                player[0].isPlayer = true;

                player[1].isKing = false;
                player[1].isPlayer = true;

                player[2].isKing = false;
                player[2].isPlayer = true;

                player[3].isKing = false;
                player[3].isPlayer = true;
            }

            if (PlayerSaveData.playerNumber == 5 && (player[0].isKing || player[1].isKing || player[2].isKing || player[3].isKing || player[4].isKing))
            {
                player[0].isKing = false;
                player[0].isPlayer = true;

                player[1].isKing = false;
                player[1].isPlayer = true;

                player[2].isKing = false;
                player[2].isPlayer = true;

                player[3].isKing = false;
                player[3].isPlayer = true;

                player[4].isKing = false;
                player[4].isPlayer = true;
            }

            if (PlayerSaveData.playerNumber == 6 && (player[0].isKing || player[1].isKing || player[2].isKing || player[3].isKing || player[4].isKing || player[5].isKing))
            {
                player[0].isKing = false;
                player[0].isPlayer = true;

                player[1].isKing = false;
                player[1].isPlayer = true;

                player[2].isKing = false;
                player[2].isPlayer = true;

                player[3].isKing = false;
                player[3].isPlayer = true;

                player[4].isKing = false;
                player[4].isPlayer = true;

                player[5].isKing = false;
                player[5].isPlayer = true;
            }
        }

        if (other.gameObject.CompareTag("Trap") && !isflashing)
        {
            isflashing = true;
            currentHealth -= 1;
            StartCoroutine(InvincibleFlash());
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("KingPoint"))
        {
            isKing = true;
            becameKing = false;
        }
    }

    IEnumerator InvincibleFlash()
    {
        for (int i = 0; i <= 2.8; i++)
        {
            GetComponent<Renderer>().material.color = new Color(1f, 0.30196078f, 0.30196078f);
            yield return new WaitForSeconds(0.5f);
            GetComponent<Renderer>().material.color = new Color(255, 255, 255);
            yield return new WaitForSeconds(0.5f);
        }
        isflashing = false;
    }
}
