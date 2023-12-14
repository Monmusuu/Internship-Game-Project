using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro.Examples;

public class Player : NetworkBehaviour
{
// Rigidbody and movement parameters
    public Rigidbody2D rigid;
    [SerializeField] private float jumpSpeed = 5;
    [SerializeField] private float m_RunSpeed = 1;
    private float m_horizontal;
    private const float groundCheckRadius = 0.2f;

    // Ground and layer masks
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask kingLayer;

    // Ground check and movement flags
    [SerializeField] private bool isGrounded = false;
    private bool wasGrounded = false;
    private bool left = false;
    private bool right = false;
    [SerializeField] private bool jumped = false;
    private bool attack = false;

    // Attack parameters and timers
    [SerializeField] private GameObject Weapon;
    [SerializeField] private Collider2D weaponCollider;
    [SerializeField] private Animator m_WeaponAnimator;
    [SyncVar(hook = nameof(OnFlipChanged))] private bool isFacingRight = true;
    private bool lastDirRight = false;
    private bool attackFinished = false;
    private double weaponTimer = 1.5f;
    [SerializeField] private double internalTimer;
    private bool isAttacking = false;
    [SerializeField] private float weaponCooldown = 0.8f;

    // Player status and synchronization
    [SyncVar] public bool isKing = false;
    [SyncVar(hook = nameof(OnIsDeadChanged))] public bool isDead = false;
    public bool isAlreadyDead = false;
    public bool becameKing = false;
    public bool isPlayer = false;
    [SyncVar(hook = nameof(OnIsFlashingChanged))] private bool isflashing = false;

    // Health and scoring
    private float maxSpeed = 15.0f;
    private float maxHealth = 6;
    [SerializeField] [SyncVar(hook = nameof(OnCurrentHealthChanged))] private float currentHealth = 6;
    [SyncVar(hook = nameof(OnCurrentScoreChanged))] public int currentScore = 0;
    [SerializeField] private Healthbar healthbar;
    public float GetMaxHealth() { return maxHealth; }
    public void SetMaxHealth(int value) { maxHealth = value; }
    public float GetCurrentHealth() { return currentHealth; }
    public void SetCurrentHealth(int value) { currentHealth = value; }

    // Game components and objects
    public PlayerSaveData playerSaveData;
    public RoundControl roundControl;
    public MenuScript menuScript;
    public MultiTargetCamera multiTargetCamera;
    [SerializeField] public GameObject playerBlockPlacement;
    [SerializeField] public GameObject trapInteraction;
    [SerializeField] public GameObject victoryInteraction;
    [SerializeField] private Transform groundCheckCollider;
    [SerializeField] private Transform groundCheckCollider2;

    // Player number and running state
    private const int MaxPlayers = 6;
    public int PlayerNumber; // This will store the player number extracted from the tag
    private bool isRunningLocal = false; // Local variable to handle isRunning
    [SerializeField][SyncVar(hook = nameof(OnRunningChanged))] private bool isRunning = false; // Synced variable to handle isRunning

    // Hat sprites
    [SerializeField][SyncVar(hook = nameof(OnHatSpriteChange))] private int hatSpriteIndex = 0;
    [SerializeField] private Sprite[] hatSpriteVariations;
    [SerializeField] private SpriteRenderer hatSpriteRenderer;

    // Body sprites
    [SerializeField][SyncVar(hook = nameof(OnBodySpriteChange))] public int bodySpriteIndex = 0;
    [SerializeField] private Sprite[] bodySpriteVariations;
    [SerializeField] private SpriteRenderer bodySpriteRenderer;

    // Weapon sprites
    [SerializeField][SyncVar(hook = nameof(OnWeaponSpriteChange))] private int weaponSpriteIndex = 0;
    [SerializeField] private Sprite[] weaponSpriteVariations;
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;

    // Animator parameters
    [SerializeField][SyncVar(hook = nameof(OnAnimatorChange))] private int animatorIndex = 0;
    [SerializeField] private RuntimeAnimatorController[] animatorVariations;
    [SerializeField] private Animator animator;

    [SerializeField] private SpriteRenderer placeCursor;

    [SerializeField] private SpriteRenderer interactionCursor;

    [SerializeField] private SpriteRenderer victoryCursor;

    // Audio components
    public AudioSource audioSource; // Reference to the AudioSource component
    public AudioClip hitAudioClip; // The audio clip to be played
    public AudioClip deathAudioClip; // The audio clip to be played
    public AudioClip swingAudioClip; // The audio clip to be played
    public AudioClip jumpAudioClip; // The audio clip to be played
    public AudioClip oilAudioClip; // The audio clip to be played
    
    private void Awake()
    {
        Transform hatChild = transform.GetChild(4);
        hatSpriteRenderer = hatChild.GetComponent<SpriteRenderer>();
        bodySpriteRenderer = GetComponent<SpriteRenderer>();
        Transform weaponChild = transform.GetChild(3);
        weaponSpriteRenderer = weaponChild.GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        internalTimer = weaponTimer;
    }

    void Start()
    {
        // Check if the current player is the local player
        if (isLocalPlayer)
        {
            // Add an AudioListener component to the local player
            AddAudioListener();
        }
        AssignPlayerTag();

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

        StartCoroutine(WaitForGameManager());
        StartCoroutine(WaitForRoundControl());

        // Now you can use PlayerNumber as the number extracted from the tag
        Debug.Log("PlayerNumber: " + PlayerNumber);
        multiTargetCamera = GameObject.Find("Main Camera").GetComponent<MultiTargetCamera>();
        isPlayer = true;
        healthbar.SetMaxHealth(maxHealth);
        playerBlockPlacement.SetActive(false);
        trapInteraction.SetActive(false);
        victoryInteraction.SetActive(false);
        weaponCollider.enabled = false;
        rigid = gameObject.GetComponent<Rigidbody2D>();
        menuScript = FindObjectOfType<MenuScript>();

        if (isServer)
        {
            multiTargetCamera = GameObject.FindObjectOfType<MultiTargetCamera>();
            multiTargetCamera.AddPlayer(this);
        }
    }

    void Update()
    {
        left = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        right = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        jumped = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W);
        attack = Input.GetKeyDown(KeyCode.Mouse0);
        
        isDeadPlayer();

        if(isServer){
            ActivatePlayerPlacement();
            ActivateTrapInteraction();
            ActivateVictoryInteraction();
        }

        if(isLocalPlayer){
            CmdGroundCheck(left || right);
        }

        if (isKing && isLocalPlayer)
        {
            rigid.velocity = Vector2.zero;
        }else if(roundControl.placingItems && !menuScript.isPause)
        {
            gameIntermission();
        }else if(!isKing && !roundControl.placingItems && roundControl.timerOn && isLocalPlayer && !menuScript.isPause)
        {
            gameInPlay();
            CmdSetRunning(isRunningLocal);
        }
    }

    public void gameIntermission()
    {
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

    public void gameInPlay()
    {
        if(!isDead && !isAlreadyDead){

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
        }
    }

    public void isDeadPlayer(){
        if(currentHealth <= 0 && !isDead && !isAlreadyDead && !roundControl.placingItems)
        {
            rigid.velocity = Vector2.zero;
            isDead = true;
            animator.SetTrigger("Dead");
            isAlreadyDead = true;
            audioSource.PlayOneShot(deathAudioClip);
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
        }else if(roundControl.placingItems){
            currentHealth = 6;
            isDead = false;
            animator.SetTrigger("Alive");
            isAlreadyDead = false;
        }
    }

    IEnumerator WaitForGameManager() {
        while (playerSaveData == null) {
            GameObject gameManager = GameObject.Find("GameManager(Clone)");

            if (gameManager != null) {
                playerSaveData = gameManager.GetComponent<PlayerSaveData>();

                ApplyPlayerSprites(gameObject, PlayerNumber-1);

                break;
            }

            yield return null; // Wait for a frame before checking again
        }
    }

    IEnumerator WaitForRoundControl() {
        while (roundControl == null) {
            GameObject roundControlObject = GameObject.Find("RoundControl(Clone)");

            if (roundControlObject != null) {
                roundControl = roundControlObject.GetComponent<RoundControl>();

                if (isServer)
                {
                    roundControl.AddPlayer(this);
                }

                break;
            }

            yield return null; // Wait for a frame before checking again
        }
    }

    void AddAudioListener()
    {
        // Check if the player already has an AudioListener component
        AudioListener audioListener = GetComponent<AudioListener>();
        if (audioListener == null)
        {
            // Add the AudioListener component to the player
            gameObject.AddComponent<AudioListener>();
        }
    }

    void AssignPlayerTag()
    {
        string[] playerColors = { "Red", "Blue", "Yellow", "Green", "Purple", "Brown" };

        for (int i = 1; i <= MaxPlayers; i++)
        {
            string playerTag = "Player" + i;

            // Check if an object with this tag already exists
            GameObject existingPlayer = GameObject.FindGameObjectWithTag(playerTag);

            if (existingPlayer == null)
            {
                // No existing player with this tag, it's available
                gameObject.tag = playerTag;
                // Assign color based on the index in playerColors array
                Color playerColor = GetPlayerColor(i - 1); // Subtract 1 from the index
                Debug.Log("Assigned tag: " + playerTag);
                interactionCursor.color = playerColor;
                placeCursor.color = playerColor;
                victoryCursor.color = playerColor;
                break;
            }
        }
    }

    Color GetPlayerColor(int index)
    {
        Color[] colors = { Color.red, Color.blue, Color.yellow, Color.green, new Color(0.5f, 0, 0.5f), new Color(0.6f, 0.4f, 0.2f) };
        
        if (index < colors.Length)
        {
            return colors[index];
        }
        else
        {
            // Return a default color if the index is out of bounds
            return Color.white;
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

    [ClientRpc]
    private void RpcActivateVictoryInteraction(bool activate)
    {
        if (victoryInteraction != null)
        {
            victoryInteraction.SetActive(activate);
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

    [Client]
    private void ActivateVictoryInteraction()
    {
        bool activate = roundControl.victoryScreen;
        RpcActivateVictoryInteraction(activate);
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

    [Command]
    void CmdGroundCheck(bool isRunning){
        RpcGroundCheck(isRunning);
    }

    [ClientRpc]
    void RpcGroundCheck(bool isRunning)
    {
        isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, groundLayer);
        Collider2D[] colliders2 = Physics2D.OverlapCircleAll(groundCheckCollider2.position, groundCheckRadius, kingLayer);
        if (colliders.Length > 0 || colliders2.Length > 0)
        {
            isGrounded = true;
        }

        if (isGrounded)
        {
            animator.SetBool("Landed", true);

            if(!isKing){

                if(roundControl != null && roundControl.timerOn && !menuScript.isPause){
                    // Set the local isRunning variable based on the rigidbody velocity
                    isRunningLocal = isRunning; // Update the local isRunning variable
                    if (isRunning)
                    {
                        animator.SetBool("Running", true);
                    }
                }else{
                    animator.SetBool("Running", false);
                }

            }else{
                animator.SetBool("Running", false);
            }
        }else{
            animator.SetBool("Landed", false);
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
        // Notify clients about the jump
        RpcPlayerJump();
    }

    [ClientRpc]
    private void RpcPlayerJump()
    {
        // Perform jump logic here, like applying jump force to the rigidbody
        if (isGrounded)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, jumpSpeed);
            animator.SetBool("Running", false);
            animator.SetBool("Landed", false);
            animator.SetTrigger("Jumped");
            audioSource.PlayOneShot(jumpAudioClip);
        }
    }

    [Command]
    private void CmdPlayerAttack()
    {
        RpcPlayAttackAnimation(); // Call an RPC to play the attack animation on all clients
    }

    // RPC to play the attack animation on all clients (including the host)
    [ClientRpc]
    private void RpcPlayAttackAnimation()
    {
        if (!isAttacking && weaponCooldown == 0.8f)
        {
            isAttacking = true;
            weaponCollider.enabled = true;
            audioSource.PlayOneShot(swingAudioClip);
            m_WeaponAnimator.SetTrigger("Swing"); // Assuming you have a common trigger "Swing" for both host and client
        }
    }

    private void OnIsDeadChanged(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            // Trigger death animation on the server
            RpcPlayDeathAnimation();
        }
    }

    [ClientRpc]
    private void RpcPlayDeathAnimation()
    {
        // Play death animation on all clients
        animator.SetTrigger("Dead");
        // Additional logic related to death animation on clients
    }

    public void ApplyPlayerSprites(GameObject player, int i)
    {
        hatSpriteIndex = playerSaveData.playerHatSpriteNumbers[i];
        weaponSpriteIndex = playerSaveData.playerWeaponSpriteNumbers[i];
        bodySpriteIndex = playerSaveData.playerBodySpriteNumbers[i];
        animatorIndex = playerSaveData.playerAnimatorNumbers[i];
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("KingPoint") && isServer && !isKing && !becameKing)
        {
            becameKing = true;
        }

        if (other.gameObject.CompareTag("Trap") && !isflashing)
        {
            audioSource.PlayOneShot(hitAudioClip);
            isflashing = true;
            currentHealth -= 3;
            StartCoroutine(InvincibleFlash());
        }

        if (other.gameObject.CompareTag("Weapon")){
            audioSource.PlayOneShot(hitAudioClip);
        }

        if (other.gameObject.CompareTag("Oil") && !isflashing){
            audioSource.PlayOneShot(oilAudioClip);
            isflashing = true;
            currentHealth -= 1;
            StartCoroutine(InvincibleFlash());
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("SawBlade") && !isflashing)
        {
            audioSource.PlayOneShot(hitAudioClip);
            isflashing = true;
            currentHealth -= 1;
            StartCoroutine(InvincibleFlash());
        }

        if (other.gameObject.CompareTag("Spike") && !isflashing)
        {
            audioSource.PlayOneShot(hitAudioClip);
            isflashing = true;
            currentHealth -= 1;
            StartCoroutine(InvincibleFlash());
        }

        if (other.gameObject.CompareTag("GuillotineBlade") && !isflashing)
        {
            audioSource.PlayOneShot(hitAudioClip);
            isflashing = true;
            currentHealth -= 1;
            StartCoroutine(InvincibleFlash());
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.CompareTag("Flame")) {
            currentHealth -= 0.05f;
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

    public void hatChangeSprite(int newIndex)
    {
        hatSpriteIndex = newIndex;
    }

    private void OnHatSpriteChange(int oldIndex, int newIndex)
    {
        hatSpriteRenderer.sprite = hatSpriteVariations[newIndex];
    }

    private void OnCurrentHealthChanged(float oldValue, float newValue)
    {
        // Update UI or perform actions based on the new health value
        healthbar.SetHealth(newValue);
    }

    public void bodyChangeSprite(int newIndex)
    {
        bodySpriteIndex = newIndex;
    }

    private void OnBodySpriteChange(int oldIndex, int newIndex)
    {
        bodySpriteRenderer.sprite = bodySpriteVariations[newIndex];
    }

    public void animatorChange(int newIndex)
    {
        animatorIndex = newIndex;
    }

    private void OnAnimatorChange(int oldIndex, int newIndex)
    {
        animator.runtimeAnimatorController = animatorVariations[newIndex];
    }
    
    public void weaponChangeSprite(int newIndex)
    {
        weaponSpriteIndex = newIndex;
    }

    private void OnWeaponSpriteChange(int oldIndex, int newIndex)
    {
        weaponSpriteRenderer.sprite = weaponSpriteVariations[newIndex];
    }

    private void OnCurrentScoreChanged(int oldScore, int newScore)
    {
        // Handle the change in currentScore here if needed
    }
}
