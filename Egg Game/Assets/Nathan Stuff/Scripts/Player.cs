using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public PlayerDetails playerDetails;

    public GameObject BuildManager;

    public GameObject KingGrid;

    [SerializeField] Transform groundCheckCollider;
    private Rigidbody2D rigid;

    private CharacterController controller;

    [SerializeField] private float jumpSpeed = 5;

    [SerializeField] LayerMask groundLayer;

    [SerializeField] bool isGrounded = false;
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

    [SerializeField] private float weaponCooldown = 1.0f;

    [SerializeField] private float knockbackStrength;

    private bool left = false;
    private bool right = false;

    private bool jumped = false;

    private bool attack = false;

    public bool isKing = false;

    private float maxSpeed = 15.0f;

    void OnEnable() {
        internalTimer = weaponTimer;
    }

    // Start is called before the first frame update
    void Start()
    {
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
        GroundCheck();

        if(isGrounded){
            if(jumped){
                rigid.AddForce(Vector3.up * jumpSpeed, ForceMode2D.Impulse);
                //Debug.Log("Jumped");
            }
        }

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

        if(attack && weaponCooldown == 1.0f && !isAttacking){
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
            weaponCooldown = 1.0f;
            attackFinished = false;
        }
    }

    void FixedUpdate() {

    }

    void GroundCheck(){
        isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, groundLayer);
        if(colliders.Length > 0){
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

        if (other.gameObject.CompareTag("KingPoint")){
           isKing = true;
           BuildManager.SetActive(true);
           KingGrid.SetActive(true);
           Debug.Log(playerDetails.playerID.ToString() + " is King");

        }
    }
}
