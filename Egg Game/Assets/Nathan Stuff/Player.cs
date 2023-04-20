using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Transform groundCheckCollider;
    private Rigidbody2D rigid;

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

    private double weaponTimer = 1.5f;
    private double internalTimer;
    private bool isAttacking = false;

    void OnEnable() {
        internalTimer = weaponTimer;
    }

    // Start is called before the first frame update
    void Start()
    {
        weaponCollider.enabled = false;
        rigid = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();

        if(isGrounded){
            if(Input.GetKeyDown(KeyCode.UpArrow)){
                rigid.AddForce(Vector3.up * jumpSpeed, ForceMode2D.Impulse);
                Debug.Log("Jumped");
            }
        }

        if(Input.GetKey(KeyCode.LeftArrow)){
            rigid.AddForce(Vector3.left * m_RunSpeed, ForceMode2D.Impulse);
            if(!lastDirRight){
                Flip();
            }
        }

        if(Input.GetKey(KeyCode.RightArrow)){
            rigid.AddForce(Vector3.right * m_RunSpeed, ForceMode2D.Impulse);
            if(lastDirRight){
                Flip();
            }
        }

        if(Input.GetMouseButtonDown(0)){
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
            }
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
}
