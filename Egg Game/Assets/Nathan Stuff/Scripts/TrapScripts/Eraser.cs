using UnityEngine;
using System.Collections;

public class Eraser : MonoBehaviour
{
    public LayerMask eraseLayer;

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
        bool isEraserOnEraserLayer = eraseLayer == (eraseLayer | (1 << transform.parent.gameObject.layer));
        //bool shouldSelfDestruct = false;

        foreach (Collider2D collider in colliders)
        {
            if (isEraserOnEraserLayer && eraseLayer == (eraseLayer | (1 << collider.gameObject.layer)) && collider.gameObject != transform.parent.gameObject)
            {
                Debug.Log("Overlap: Destroying " + collider.gameObject.name);
                Destroy(collider.gameObject);
            }
        }

        if (isEraserOnEraserLayer)
        {
            StartCoroutine(DelayedSelfDestruct(2f));
        }
    }

    private IEnumerator DelayedSelfDestruct(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Self-Destruct: Destroying " + transform.parent.gameObject.name);
        Destroy(transform.parent.gameObject);
    }
}