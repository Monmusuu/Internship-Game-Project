using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public Rigidbody2D rigid;
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
    public GameObject playerBlockPlacement;
    public GameObject trapInteraction;
    public Transform kingSpawnLocation;
    [SerializeField] private Transform groundCheckCollider;
    [SerializeField] private Transform groundCheckCollider2;

<<<<<<< Updated upstream
=======
    [SerializeField]
    private CustomNetworkManager customNetworkManager;

    public int PlayerNumber; // This will store the player number extracted from the tag

    private bool isRunningLocal = false; // Local variable to handle isRunning
    [SyncVar(hook = nameof(OnRunningChanged))]
    private bool isRunning = false; // Synced variable to handle isRunning

>>>>>>> Stashed changes
    public int GetMaxHealth() { return maxHealth; }
    public void SetMaxHealth(int value) { maxHealth = value; }
    public int GetCurrentHealth() { return currentHealth; }
    public void SetCurrentHealth(int value) { currentHealth = value; }

    [SerializeField][SyncVar(hook = nameof(OnHatSpriteChange))]
    private int hatSpriteIndex = 0;

    [SerializeField]
    private Sprite[] hatSpriteVariations;

    [SerializeField]
    private SpriteRenderer hatSpriteRenderer;

    public void hatChangeSprite(int newIndex)
    {
        //if (isServer)
        //{
            hatSpriteIndex = newIndex;
        //}
    }

    private void OnHatSpriteChange(int oldIndex, int newIndex)
    {
        hatSpriteRenderer.sprite = hatSpriteVariations[newIndex];
    }

    [SerializeField][SyncVar(hook = nameof(OnBodySpriteChange))]
    private int bodySpriteIndex = 0;

    [SerializeField]
    private Sprite[] bodySpriteVariations;

    [SerializeField]
    private SpriteRenderer bodySpriteRenderer;

    public void bodyChangeSprite(int newIndex)
    {
        //if (isServer)
        //{
            bodySpriteIndex = newIndex;
        //}
    }

    private void OnBodySpriteChange(int oldIndex, int newIndex)
    {
        bodySpriteRenderer.sprite = bodySpriteVariations[newIndex];
    }

    
    [SerializeField][SyncVar(hook = nameof(OnWeaponSpriteChange))]
    private int weaponSpriteIndex = 0;

    [SerializeField]
    private Sprite[] weaponSpriteVariations;

    [SerializeField]
    private SpriteRenderer weaponSpriteRenderer;

    public void weaponChangeSprite(int newIndex)
    {
        //if (isServer)
        //{
            weaponSpriteIndex = newIndex;
        //}
    }

    private void OnWeaponSpriteChange(int oldIndex, int newIndex)
    {
        weaponSpriteRenderer.sprite = weaponSpriteVariations[newIndex];
    }

    [SerializeField][SyncVar(hook = nameof(OnAnimatorChange))]
    private int animatorIndex = 0;

    [SerializeField]
    private RuntimeAnimatorController[] animatorVariations;

    [SerializeField]
    private Animator animator;

    public void animatorChange(int newIndex)
    {
        //if (isServer)
        //{
            animatorIndex = newIndex;
        //}
    }

    private void OnAnimatorChange(int oldIndex, int newIndex)
    {
        animator.runtimeAnimatorController = animatorVariations[newIndex];
    }
    

    private void Awake()
    {
        Transform hatChild = transform.GetChild(5);
        hatSpriteRenderer = hatChild.GetComponent<SpriteRenderer>();
        bodySpriteRenderer = GetComponent<SpriteRenderer>();
        Transform weaponChild = transform.GetChild(4);
        weaponSpriteRenderer = weaponChild.GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        internalTimer = weaponTimer;
    }



    void Start()
    {
        // Get the tag of the GameObject this script is attached to
        string objectTag = gameObject.tag;

        // Check if the tag starts with "player" and has at least 7 characters (e.g., "player1" to "player6")
        if (objectTag.StartsWith("Player") && objectTag.Length >= 7)
        {
            // Extract the number part of the tag and parse it to an int
            string numberPart = objectTag.Substring(6); // Get characters after "player"
            int.TryParse(numberPart, out PlayerNumber);
        }
        else
        {
            // If the tag doesn't match the expected format, set PlayerNumber to a default value (e.g., -1)
            PlayerNumber = -1;
        }

        // Now you can use PlayerNumber as the number extracted from the tag
        Debug.Log("PlayerNumber: " + PlayerNumber);

        customNetworkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
        gameManager = GameObject.Find("GameState").GetComponent<GameManager>();
        roundControl = GameObject.Find("RoundControl").GetComponent<RoundControl>();
<<<<<<< Updated upstream
        playerSaveData = GameObject.Find("GameState").GetComponent<PlayerSaveData>();
=======
        multiTargetCamera = GameObject.Find("Main Camera").GetComponent<MultiTargetCamera>();
        playerSaveData = GameObject.Find("GameManager").GetComponent<PlayerSaveData>();
>>>>>>> Stashed changes
        kingSpawnLocation = GameObject.Find("KingPoint").transform;
        isPlayer = true;
        healthbar.SetMaxHealth(maxHealth);
        BuildManager.SetActive(false);
        trapInteraction.SetActive(false);
        weaponCollider.enabled = false;
        rigid = gameObject.GetComponent<Rigidbody2D>();
        controller = gameObject.GetComponent<CharacterController>();
    }

<<<<<<< Updated upstream
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
=======
        ApplyPlayerSprites(gameObject, PlayerNumber-1);
        

        if (isServer)
>>>>>>> Stashed changes
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

        if(roundControl.placingItems && isPlayer && roundControl.Round >= 1 ){
            playerBlockPlacement.SetActive(true);
        }else{
            playerBlockPlacement.SetActive(false);
        }

        if(roundControl.placingItems && isKing && roundControl.Round >= 1 ){
            BuildManager.SetActive(true);
        }else{
            BuildManager.SetActive(false);
        }

        if(roundControl.timerOn && isKing && roundControl.Round >= 1){
            trapInteraction.SetActive(true);
        }else{
            trapInteraction.SetActive(false);
        }

        if(roundControl.timerOn){
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
                //Debug.Log("Kingship has been claimed");
                rigid.velocity = Vector2.zero;
                transform.position = kingSpawnLocation.position;
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
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
        lastDirRight = !lastDirRight;
    }

    //[Command]
    public void ApplyPlayerSprites(GameObject player, int i)
    {
        hatSpriteIndex = playerSaveData.playerHatSpriteNumbers[i];
        weaponSpriteIndex = playerSaveData.playerWeaponSpriteNumbers[i];
        bodySpriteIndex = playerSaveData.playerBodySpriteNumbers[i];
        animatorIndex = playerSaveData.playerAnimatorNumbers[i];
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

    // void OnTriggerStay2D(Collider2D other)
    // {
    //     if (other.gameObject.CompareTag("KingPoint"))
    //     {
    //         isKing = true;
    //         becameKing = false;
    //     }
    // }

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
