using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Transform groundCheckCollider;

    [SerializeField] private float jumpSpeed = 5;

    [SerializeField] private Rigidbody2D rigid;

    bool isGrounded = false;


    // Start is called before the first frame update
    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow)){
            rigid.AddForce(Vector3.up * jumpSpeed, ForceMode2D.Impulse);
        }
    }

    // void GroundCheck(){
    //     Collider2d[] colliders = Physics2D.OverlapCircleAll()
    // }
}
