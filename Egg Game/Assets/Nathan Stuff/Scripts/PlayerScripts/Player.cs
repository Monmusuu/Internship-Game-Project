using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public PlayerDetails playerDetails;

    public Player[] player;

    [SerializeField] Animator animator; 

    public RoundControl roundControl;

    public GameObject BuildManager;

    public GameObject KingGrid;

    public Transform playerSpawnLocation;
    public Transform kingSpawnLocation;

    [SerializeField] Transform groundCheckCollider;
    [SerializeField] Transform groundCheckCollider2;
    private Rigidbody2D rigid;

    private CharacterController controller;

    [SerializeField] private float jumpSpeed = 5;

    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask kingLayer;

    [SerializeField] bool isGrounded = false;

    [SerializeField] bool wasGrounded = false;
    const float groundCheckRadius = 0.2f;

    [SerializeField] private float m_RunSpeed = 1;

    private float m_horizontal;

    [SerializeField] private GameObject Weapon;

    [SerializeField] private Collider2D weaponCollider;

    [SerializeField] Animator m_WeaponAnimator;

    private bool lastDirRight = false;
    private bool attackFinished = false;

    private double weaponTimer = 1.5f;
    [SerializeField] private double internalTimer;
    private bool isAttacking = false;

    [SerializeField] private float weaponCooldown = 0.8f;

    [SerializeField] private float knockbackStrength;

    private bool left = false;
    private bool right = false;

    private bool jumped = false;

    private bool attack = false;

    public bool isKing = false;
    public bool becameKing = false;
    public bool isPlayer = false;
    public bool isflashing = false;

    private float maxSpeed = 15.0f;

    private int maxHealth = 6;
    private int currentHealth = 6;
    [SerializeField] private Healthbar healthbar;

    public int GetMaxHealth(){return maxHealth;}
    public void SetMaxHealth(int value){maxHealth = value;}
    
    public int  GetCurrentHealth(){return currentHealth;}
    public void SetCurrentHealth(int value){currentHealth = value;}

    void OnEnable() {
        internalTimer = weaponTimer;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isPlayer = true;
        playerSpawnLocation = GameObject.Find("SpawnPoint").transform;
        kingSpawnLocation = GameObject.Find("KingPoint").transform;
        healthbar.SetMaxHealth(maxHealth);
        BuildManager.SetActive(false);
        KingGrid.SetActive(false);
        weaponCollider.enabled = false;
        rigid = gameObject.GetComponent<Rigidbody2D>();
        controller = gameObject.GetComponent<CharacterController>();
    }

    public void OnMoveLeft(InputAction.CallbackContext context){
        left = context.action.triggered;
    }

    public void OnMoveRight(InputAction.CallbackContext context){
        right = context.action.triggered;
    }

    public void OnJump(InputAction.CallbackContext context){
        jumped = context.action.triggered;

    }

    public void OnAttack(InputAction.CallbackContext context){
        attack = context.action.triggered;

    }

    // Update is called once per frame
    void Update()
    {
        if(!isKing){
            GroundCheck();

            if(isGrounded){
                animator.SetBool("Landed", true);
                if(jumped){
                    animator.SetBool("Landed", false);
                    rigid.AddForce(Vector3.up * jumpSpeed, ForceMode2D.Impulse);
                    animator.SetTrigger("Jumped");
                    //Debug.Log("Jumped");
                }else{
                    // Player is running while grounded
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

            //Debug.Log(rigid.velocity.magnitude);
            if(rigid.velocity.magnitude > maxSpeed){
                rigid.velocity = Vector2.ClampMagnitude(rigid.velocity, maxSpeed);
            }

            if(left){
                rigid.AddForce(Vector2.left * m_RunSpeed, ForceMode2D.Impulse);
                if(!lastDirRight){
                    Flip();
                }
            }

            if(right){
                rigid.AddForce(Vector2.right * m_RunSpeed, ForceMode2D.Impulse);
                if(lastDirRight){
                    Flip();
                }
            }

            if(attack && weaponCooldown == 0.8f && !isAttacking){
                isAttacking = true;
                weaponCollider.enabled = true;
                m_WeaponAnimator.SetTrigger("Swing");
                Debug.Log("Attack");
            }
            if(isAttacking){
                internalTimer -= Time.deltaTime;
                if (internalTimer <= 0){
                    weaponCollider.enabled = false;
                    isAttacking = false;
                    internalTimer = weaponTimer;
                    attackFinished = true;
                }
            }
            if(attackFinished){
                weaponCooldown -= Time.deltaTime;
            }
            if(weaponCooldown <= 0){
                weaponCooldown = 0.8f;
                attackFinished = false;
            }
        }

        if(becameKing){
            rigid.velocity = Vector2.zero;
            transform.position = kingSpawnLocation.position;
        }
        if(isPlayer && roundControl.Respawn){
            rigid.velocity = Vector2.zero;
            transform.position = playerSpawnLocation.position;
            //isPlayer = false;
        }
    }

    void FixedUpdate() {

    }

    void GroundCheck(){
        isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, groundLayer);
        Collider2D[] colliders2 = Physics2D.OverlapCircleAll(groundCheckCollider2.position, groundCheckRadius, kingLayer);
        if(colliders.Length > 0){
            isGrounded = true;
            //Debug.Log("Grounded");
        }

        if(colliders2.Length > 0){
            isGrounded = true;
            //Debug.Log("Grounded");
        }
    }

    void Flip(){
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
        lastDirRight = !lastDirRight;
    }

    void OnTriggerEnter2D(Collider2D other) {

        if(other.gameObject.CompareTag("KingPoint")){
            becameKing = true;
            isPlayer = false;
            BuildManager.SetActive(true);
            KingGrid.SetActive(true);
            Debug.Log(playerDetails.playerID.ToString() + " is King");

            if(player[0].isKing){
                player[0].isKing = false;
                player[0].isPlayer = true;
            }

            if(player[1].isKing){
                player[1].isKing = false;
                player[1].isPlayer = true;
            }

            if(player[2].isKing){
                player[2].isKing = false;
                player[2].isPlayer = true;
            }

            if(player[3].isKing){
                player[3].isKing = false;
                player[3].isPlayer = true;
            }

            if(player[4].isKing){
                player[4].isKing = false;
                player[4].isPlayer = true;
            }

            if(player[5].isKing){
                player[5].isKing = false;
                player[5].isPlayer = true;
            }
        }

        if(other.gameObject.CompareTag("Trap") && !isflashing){
            isflashing = true;
            currentHealth -= 1;
            Debug.Log(playerDetails.playerID.ToString() + "is Hit by Trap");
            StartCoroutine(InvincibleFlash());
        }

    }
    void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.CompareTag("KingPoint")){
            isKing = true;
            becameKing = false;
        }
    }

    IEnumerator InvincibleFlash() {
        for (int i = 0; i <= 2.8; i++)
        {
            GetComponent<Renderer>().material.color = new Color(1f, 0.30196078f, 0.30196078f);;
            yield return new WaitForSeconds(0.5f);
            GetComponent<Renderer>().material.color = new Color (255, 255, 255);
            yield return new WaitForSeconds(0.5f);
        }
        isflashing = false;
        
    }
}
