using UnityEngine;
using System.Collections;

public class Eraser : MonoBehaviour
{
    public LayerMask eraseLayer;
    private bool canDestroy = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (eraseLayer == (eraseLayer | (1 << other.gameObject.layer)) && other.gameObject != transform.parent.gameObject)
        {
            Debug.Log("Trigger: Destroying " + other.gameObject.name);
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (eraseLayer == (eraseLayer | (1 << collision.gameObject.layer)) && collision.gameObject != transform.parent.gameObject)
        {
            Debug.Log("Collision: Destroying " + collision.gameObject.name);
            Destroy(collision.gameObject);
        }
    }

    private void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.parent.position, transform.parent.localScale / 2f, transform.parent.rotation.eulerAngles.z);
        foreach (Collider2D collider in colliders)
        {
            if (eraseLayer == (eraseLayer | (1 << collider.gameObject.layer)) && collider.gameObject != transform.parent.gameObject)
            {
                Debug.Log("Overlap: Destroying " + collider.gameObject.name);
                StartCoroutine(DelayedDestroy(2f, collider.gameObject));
            }
        }

        if (canDestroy)
        {
            Debug.Log("Self-Destruct: Destroying " + transform.parent.gameObject.name);
            Destroy(transform.parent.gameObject);
        }
        else
        {
            StartCoroutine(DelayedDestroy(2f, transform.parent.gameObject)); // Delayed destroy for the eraser's parent object
        }
    }

    private IEnumerator DelayedDestroy(float delay, GameObject objectToDestroy)
    {
        yield return new WaitForSeconds(delay);
        if (objectToDestroy == transform.parent.gameObject)
        {
            canDestroy = true;
        }
        else
        {
            Destroy(objectToDestroy); // Destroy the colliding game object
        }
    }
}