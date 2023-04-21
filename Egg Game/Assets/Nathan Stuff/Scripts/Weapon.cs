using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float knockbackStrength = 20.0f;


    private void OnTriggerEnter2D(Collider2D other) {

        Rigidbody2D m_rigidbody = other.GetComponent<Collider2D>().GetComponent<Rigidbody2D>();
        Vector2 direction = other.transform.position - transform.position;
        direction.y = 4;
        direction.x = 5;

        if (transform.parent.CompareTag("Player1")){
            Debug.Log("I'm Player One");
            if(other.gameObject.tag == "Player2"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player2");
            }
            if(other.gameObject.tag == "Player3"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player3");
            }
            if(other.gameObject.tag == "Player4"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player4");
            }
            if(other.gameObject.tag == "Player5"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player5");
            }
            if(other.gameObject.tag == "Player6"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player6");
            }
        }

        if (transform.parent.CompareTag("Player2")){
            Debug.Log("I'm Player Two");
            if(other.gameObject.tag == "Player1"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player1");
            }
            if(other.gameObject.tag == "Player3"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player3");
            }
            if(other.gameObject.tag == "Player4"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player4");
            }
            if(other.gameObject.tag == "Player5"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player5");
            }
            if(other.gameObject.tag == "Player6"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player6");
            }
        }

        if (transform.parent.CompareTag("Player3")){
            Debug.Log("I'm Player Three");
            if(other.gameObject.tag == "Player2"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player2");
            }
            if(other.gameObject.tag == "Player1"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player1");
            }
            if(other.gameObject.tag == "Player4"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player4");
            }
            if(other.gameObject.tag == "Player5"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player5");
            }
            if(other.gameObject.tag == "Player6"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player6");
            }
        }

        if (transform.parent.CompareTag("Player4")){
            Debug.Log("I'm Player Four");
            if(other.gameObject.tag == "Player2"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player2");
            }
            if(other.gameObject.tag == "Player3"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player3");
            }
            if(other.gameObject.tag == "Player1"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player1");
            }
            if(other.gameObject.tag == "Player5"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player5");
            }
            if(other.gameObject.tag == "Player6"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player6");
            }
        }

        if (transform.parent.CompareTag("Player5")){
            Debug.Log("I'm Player Five");
            if(other.gameObject.tag == "Player2"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player2");
            }
            if(other.gameObject.tag == "Player3"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player3");
            }
            if(other.gameObject.tag == "Player4"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player4");
            }
            if(other.gameObject.tag == "Player1"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player1");
            }
            if(other.gameObject.tag == "Player6"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player6");
            }
        }

        if (transform.parent.CompareTag("Player6")){
            Debug.Log("I'm Player Six");
            if(other.gameObject.tag == "Player2"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player2");
            }
            if(other.gameObject.tag == "Player3"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player3");
            }
            if(other.gameObject.tag == "Player4"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player4");
            }
            if(other.gameObject.tag == "Player5"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player5");
            }
            if(other.gameObject.tag == "Player1"){
                m_rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode2D.Impulse);
                Debug.Log("Launched Player1");
            }
        }
    }
}
