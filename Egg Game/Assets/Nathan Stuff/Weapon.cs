using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float knockbackStrength;

    private void OnTriggerEnter(Collider collision) {

        Rigidbody2D rigid = collision.GetComponent<Collider2D>().GetComponent<Rigidbody2D>();

        if((transform.parent != null && transform.parent.tag == "Player1")){
            if(collision.gameObject.tag == "Player2"){
                Debug.Log("Launched Player2");
                rigid.AddForce(Vector3.right * knockbackStrength, ForceMode2D.Impulse);
            }
        }
    }
}
