using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro.Examples;

public class Player : NetworkBehaviour
{
    public Rigidbody2D rigid;
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
    [SyncVar]
    public bool isDead = false;
    public bool isAlreadyDead = false;
    //[SyncVar]
    public bool becameKing = false;
    //[SyncVar]
    public bool isPlayer = false;
    [SyncVar(hook = nameof(OnIsFlashingChanged))]
    private bool isflashing = false;
    private float maxSpeed = 15.0f;
    private int maxHealth = 6;
    private int currentHealth = 6;
    [SyncVar]
    public int currentScore = 0;
    [SerializeField] private Healthbar healthbar;

    public PlayerSaveData playerSaveData;
    public RoundControl roundControl;
    public MultiTargetCamera multiTargetCamera;
    [SerializeField] public GameObject playerBlockPlacement;
    [SerializeField] public GameObject trapInteraction;
    public Transform kingSpawnLocation;
    [SerializeField] private Transform groundCheckCollider;
    [SerializeField] private Transform groundCheckCollider2;

    public int PlayerNumber; // This will store the player number extracted from the tag

    private bool isRunningLocal = false; // Local variable to handle isRunning
    [SyncVar(hook = nameof(OnRunningChanged))]
    private bool isRunning = false; // Synced variable to handle isRunning

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

    private bool gameManagerFound = false;

    private bool roundControlFound = false;

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
        Transform hatChild = transform.GetChild(4);
        hatSpriteRenderer = hatChild.GetComponent<SpriteRenderer>();
        bodySpriteRenderer = GetComponent<SpriteRenderer>();
        Transform weaponChild = transform.GetChild(3);
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
        multiTargetCamera = GameObject.Find("Main Camera").GetComponent<MultiTargetCamera>();
        kingSpawnLocation = GameObject.Find("KingPoint").transform;
        isPlayer = true;
        healthbar.SetMaxHealth(maxHealth);
        playerBlockPlacement.SetActive(false);
        trapInteraction.SetActive(false);
        weaponCollider.enabled = false;
        rigid = gameObject.GetComponent<Rigidbody2D>();

        if (isServer)
        {
            multiTargetCamera = GameObject.FindObjectOfType<MultiTargetCamera>();
            multiTargetCamera.AddPlayer(this);
        }
    }

    IEnumerator WaitForGameManager() {
        while (true) {
            GameObject gameManager = GameObject.Find("GameManager(Clone)");

            if (gameManager != null) {
                playerSaveData = gameManager.GetComponent<PlayerSaveData>();
                //Debug.Log("GameManager found!");
                gameManagerFound = true;

                ApplyPlayerSprites(gameObject, PlayerNumber-1);

                break;
            }

            yield return null; // Wait for a frame before checking again
        }
    }

    IEnumerator WaitForRoundControl() {
        while (true) {
            GameObject roundControlObject = GameObject.Find("RoundControl(Clone)");

            if (roundControlObject != null) {
                roundControl = roundControlObject.GetComponent<RoundControl>();
                //Debug.Log("RoundControl found!");
                roundControlFound = true;

                if (isServer)
                {
                    roundControl.AddPlayer(this);
                }

                break;
            }

            yield return null; // Wait for a frame before checking again
        }
    }

    [ClientRpc]
    private void RpcActivatePlayerPlacement(bool activate)
    {
        if (playerBlockPlacement != null)
        {
            playerBlockPlacement.SetActive(activate);
        }
    }

    [ClientRpc]
    private void RpcActivateTrapInteraction(bool activate)
    {
        if (trapInteraction != null)
        {
            trapInteraction.SetActive(activate);
        }
    }

    [Client]
    private void ActivatePlayerPlacement()
    {
        bool activate = roundControl.placingItems && roundControl.Round >= 1;
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
        if (!gameManagerFound) {
            Debug.Log("Looking for GameManager");
            StartCoroutine(WaitForGameManager());
        }

        if(!roundControlFound){
            Debug.Log("Looking for RoundControl");
            StartCoroutine(WaitForRoundControl());
        }
        
        left = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        right = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        jumped = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W);
        attack = Input.GetKeyDown(KeyCode.Mouse0);
        
        if(isServer){
            ActivatePlayerPlacement();
            ActivateTrapInteraction();
        }

        if(currentHealth <= 0 && !isDead && !isAlreadyDead)
        {
            rigid.velocity = Vector2.zero;
            isDead = true;
            animator.SetTrigger("Dead");
            isAlreadyDead = true;
            if (weaponSpriteRenderer != null)
            {
                Color spriteColor = weaponSpriteRenderer.color;
                spriteColor.a = 0.0f; // You can set the alpha value to any value between 0 (fully transparent) and 1 (fully opaque)
                weaponSpriteRenderer.color = spriteColor;
            }

            if(healthbar != null)
            {
                // Iterate through all child objects
                foreach (Transform child in healthbar.transform)
                {
                    // Check if the child has a SpriteRenderer component
                    Image childImage = child.GetComponent<Image>();
                    if (childImage != null)
                    {
                        // Adjust the alpha value of the sprite's color
                        Color spriteColor = childImage.color;
                        spriteColor.a = 0.0f;
                        childImage.color = spriteColor;
                    }else{
                        Debug.Log("Is null");
                    }
                }
            }else{
                Debug.Log("Is null");
            }
        }

        if (isKing)
        {
            rigid.velocity = Vector2.zero;
            if(roundControl.placingItems){
                transform.position = kingSpawnLocation.position;
            }
        }

        if(roundControl.placingItems)
        {
            rigid.velocity = Vector2.zero;
            isDead = false;
            animator.SetTrigger("Respawn");
            currentHealth = 6;
            isAlreadyDead = false;

            //Debug.Log("Respawned");

            if (weaponSpriteRenderer != null)
            {
                Color spriteColor = weaponSpriteRenderer.color;
                spriteColor.a = 1.0f; // You can set the alpha value to any value between 0 (fully transparent) and 1 (fully opaque)
                weaponSpriteRenderer.color = spriteColor;
            }

            if(healthbar != null)
            {
                // Iterate through all child objects
                foreach (Transform child in healthbar.transform)
                {
                    // Check if the child has a SpriteRenderer component
                    Image childImage = child.GetComponent<Image>();
                    if (childImage != null)
                    {
                        // Adjust the alpha value of the sprite's color
                        Color spriteColor = childImage.color;
                        spriteColor.a = 1.0f;
                        childImage.color = spriteColor;
                    }else{
                        Debug.Log("Is null");
                    }
                }
            }
        }

        if(!roundControl.placingItems)
        {
            if (roundControl.timerOn)
            {
                if(!isDead && !isAlreadyDead){
                    if (!isKing)
                    {
                        GroundCheck();

                        if (isGrounded)
                        {
                            animator.SetBool("Landed", true);
                            if (jumped)
                            {
                                animator.SetBool("Running", false);
                                animator.SetBool("Landed", false);
                                rigid.velocity = new Vector2(rigid.velocity.x, jumpSpeed);
                                animator.SetTrigger("Jumped");
                            }
                            else
                            {
                                // Set the local isRunning variable based on the rigidbody velocity
                                isRunningLocal = Mathf.Abs(rigid.velocity.x) >= 0.1f;
                            }
                        }else{
                            animator.SetBool("Landed", false);
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
                }
            }
            
            if (isLocalPlayer && !isDead && !isKing && NetworkClient.ready)
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
        if (other.gameObject.CompareTag("KingPoint") && isServer && !isKing)
        {
            becameKing = true;
        }

        if (other.gameObject.CompareTag("Trap") && !isflashing)
        {
            isflashing = true;
            currentHealth -= 3;
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
