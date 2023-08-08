using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    private Animator animator;
    public Rigidbody2D rigid;
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
    [SyncVar(hook = nameof(OnFlipChanged))]
    private bool isFacingRight = true;
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
    [SyncVar]
    public bool isKing = false;
    public bool isPause = false;
    [SyncVar]
    public bool becameKing = false;
    [SyncVar]
    public bool isPlayer = false;
    [SyncVar(hook = nameof(OnIsFlashingChanged))]
    private bool isflashing = false;
    private float maxSpeed = 15.0f;
    private int maxHealth = 6;
    private int currentHealth = 6;
    [SerializeField] private Healthbar healthbar;

    public Player[] player;
    public PlayerSaveData playerSaveData;
    public RoundControl roundControl;
    public MultiTargetCamera multiTargetCamera;
    public GameObject BuildManager;
    public GameObject playerBlockPlacement;
    public GameObject trapInteraction;
    public Transform kingSpawnLocation;
    [SerializeField] private Transform groundCheckCollider;
    [SerializeField] private Transform groundCheckCollider2;

    private bool isRunningLocal = false; // Local variable to handle isRunning
    [SyncVar(hook = nameof(OnRunningChanged))]
    private bool isRunning = false; // Synced variable to handle isRunning

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
        multiTargetCamera = GameObject.Find("Main Camera").GetComponent<MultiTargetCamera>();
        playerSaveData = GameObject.Find("GameState").GetComponent<PlayerSaveData>();
        kingSpawnLocation = GameObject.Find("KingPoint").transform;
        animator = GetComponent<Animator>();
        isPlayer = true;
        healthbar.SetMaxHealth(maxHealth);
        playerBlockPlacement.SetActive(false);
        trapInteraction.SetActive(false);
        weaponCollider.enabled = false;
        rigid = gameObject.GetComponent<Rigidbody2D>();

        if (isServer)
        {
            roundControl = GameObject.FindObjectOfType<RoundControl>();
            roundControl.AddPlayer(this); // Add this player to the players list
            multiTargetCamera = GameObject.FindObjectOfType<MultiTargetCamera>();
            multiTargetCamera.AddPlayer(this);
        }
    }

    [ClientRpc]
    private void RpcActivateBuildManager(bool activate)
    {   
        BuildManager.SetActive(activate);
    }

 
    [ClientRpc]
    private void RpcActivatePlayerPlacement(bool activate)
    {
        playerBlockPlacement.SetActive(activate);
    }

    [ClientRpc]
    private void RpcActivateTrapInteraction(bool activate)
    {
        trapInteraction.SetActive(activate);
    }

    [Client]
    private void ActivateBuildManager()
    {
        bool activate = roundControl.placingItems && isKing && roundControl.Round >= 1;
        RpcActivateBuildManager(activate);
    }

    [Client]
    private void ActivatePlayerPlacement()
    {
        bool activate = roundControl.placingItems && isPlayer && roundControl.Round >= 1;
        RpcActivatePlayerPlacement(activate);
    }

    [Client]
    private void ActivateTrapInteraction()
    {
        bool activate = roundControl.timerOn && isKing && roundControl.Round >= 1;
        RpcActivateTrapInteraction(activate);
    }

    // OnDestroy is called when the player GameObject is destroyed
    void OnDestroy()
    {
        // Ensure that the roundControl reference is valid and this is the server (host)
        if (roundControl != null && isServer)
        {
            roundControl.RemovePlayer(this); // Remove this player from the players list
        }

        if (multiTargetCamera != null && isServer){
            multiTargetCamera.RemovePlayer(this);
        }
    }

    void Update()
    {
        left = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        right = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        jumped = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W);
        attack = Input.GetKeyDown(KeyCode.Mouse0);

        if (player == null || player.Length == 0)
        {
            InitializePlayerArray();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPause = !isPause;
            if (isPause)
            {
                gameManager.PauseGame();
            }
            else
            {
                gameManager.UnpauseGame();
            }
        }

        ActivateBuildManager();
        ActivatePlayerPlacement();
        ActivateTrapInteraction();

        if(!roundControl.placingItems){
            if (roundControl.timerOn)
            {
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
                            // Set the local isRunning variable based on the rigidbody velocity
                            isRunningLocal = Mathf.Abs(rigid.velocity.x) > 0.1f;
                        }
                    }

                    wasGrounded = isGrounded;

                    rigid.velocity = new Vector2(Mathf.Clamp(rigid.velocity.x, -maxSpeed, maxSpeed), rigid.velocity.y);

                    float targetVelocityX = 0f;
                    if (left)
                    {
                        targetVelocityX = -m_RunSpeed;
                    }
                    else if (right)
                    {
                        targetVelocityX = m_RunSpeed;
                    }

                    float acceleration = left || right ? 0.1f : 0.008f;
                    rigid.velocity = new Vector2(Mathf.Lerp(rigid.velocity.x, targetVelocityX, acceleration), rigid.velocity.y);

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
            }

            if (isLocalPlayer)
            {
                // If the player changes direction, update the lastDirRight and isFacingRight variables
                if (left && !lastDirRight)
                {
                    lastDirRight = true;
                    isFacingRight = true;
                    Flip();
                }
                else if (right && lastDirRight)
                {
                    lastDirRight = false;
                    isFacingRight = false;
                    Flip();
                }

                // Call the CmdPlayerJump command on the server if the player jumped
                if (jumped)
                {
                    CmdPlayerJump();
                }

                // Call the CmdPlayerAttack command on the server if the player attacked
                if (attack)
                {
                    CmdPlayerAttack();
                }

                // Set the local isRunning variable based on the rigidbody velocity
                isRunningLocal = Mathf.Abs(rigid.velocity.x) > 0.1f;

                // Call CmdSetRunning method directly on the server (host)
                CmdSetRunning(isRunningLocal);
            }
        }
        

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
        // Reverse the isFacingRight value for the host to ensure correct flip for all players
        if (isLocalPlayer)
        {
            isFacingRight = !isFacingRight;
            CmdSetFacingRight(isFacingRight);
        }

        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x = isFacingRight ? 1 : -1;
        gameObject.transform.localScale = currentScale;
    }

    private void OnFlipChanged(bool oldValue, bool newValue)
    {
        // Update the player's scale based on the isFacingRight variable
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x = newValue ? 1 : -1;
        gameObject.transform.localScale = currentScale;
    }

    [Command]
    private void CmdSetFacingRight(bool facingRight)
    {
        isFacingRight = facingRight;
    }

    [Command]
    private void CmdSetRunning(bool running)
    {
        isRunning = running;
    }

    private void OnRunningChanged(bool oldValue, bool newValue)
    {
        animator.SetBool("Running", newValue);
    }

    [Command]
    private void CmdPlayerJump()
    {
        // Perform jump logic here, like applying jump force to the rigidbody
        if (isGrounded)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, jumpSpeed);
            animator.SetTrigger("Jumped");
        }
    }

    [Command]
    private void CmdPlayerAttack()
    {
        // Perform attack logic here on the server
        if (!isAttacking && weaponCooldown == 0.8f)
        {
            isAttacking = true;
            weaponCollider.enabled = true;
            RpcPlayAttackAnimation(); // Call an RPC to play the attack animation on all clients
        }
    }

    // RPC to play the attack animation on all clients (including the host)
    [ClientRpc]
    private void RpcPlayAttackAnimation()
    {
        // Perform any visual/sound effects related to the attack on the clients here, if needed.
        m_WeaponAnimator.SetTrigger("Swing"); // Assuming you have a common trigger "Swing" for both host and client
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("KingPoint"))
        {
            becameKing = true;
        }

        if (other.gameObject.CompareTag("Trap") && !isflashing)
        {
            isflashing = true;
            currentHealth -= 1;
            StartCoroutine(InvincibleFlash());
        }
    }

    private void OnIsFlashingChanged(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            StartCoroutine(InvincibleFlash());
        }
    }

    private IEnumerator InvincibleFlash()
    {
        for (int i = 0; i <= 2; i++) // Changed 2.8 to 2 (assuming it's for visual flashing effect)
        {
            GetComponent<Renderer>().material.color = new Color(1f, 0.30196078f, 0.30196078f);
            yield return new WaitForSeconds(0.5f);
            GetComponent<Renderer>().material.color = new Color(255, 255, 255);
            yield return new WaitForSeconds(0.5f);
        }
        isflashing = false;
    }
}
