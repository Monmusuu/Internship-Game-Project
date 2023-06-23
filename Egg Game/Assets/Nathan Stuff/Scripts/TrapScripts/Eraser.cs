using UnityEngine;
using System.Collections;

public class Eraser : MonoBehaviour
{
    public LayerMask eraseLayer;
    public float eraseRadius = 5f; // Customize the erase radius if needed

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ShouldDestroyObject(other.gameObject))
        {
            Debug.Log("Trigger: Destroying " + other.gameObject.name);
            StartCoroutine(DestroyHierarchy(other.gameObject, 1f));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (ShouldDestroyObject(collision.gameObject))
        {
            Debug.Log("Collision: Destroying " + collision.gameObject.name);
            StartCoroutine(DestroyHierarchy(collision.gameObject, 1f));
        }
    }

    private void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.parent.position, eraseRadius, eraseLayer);
        bool isEraserOnEraserLayer = eraseLayer == (eraseLayer | (1 << transform.parent.gameObject.layer));

        foreach (Collider2D collider in colliders)
        {
            if (ShouldDestroyObject(collider.gameObject))
            {
                Debug.Log("Overlap: Destroying " + collider.gameObject.name);
                StartCoroutine(DestroyHierarchy(collider.gameObject, 1f));
            }
        }

        if (isEraserOnEraserLayer)
        {
            StartCoroutine(DelayedSelfDestruct(2f));
        }
    }

    private bool ShouldDestroyObject(GameObject obj)
    {
        return eraseLayer == (eraseLayer | (1 << obj.layer)) && obj != transform.parent.gameObject;
    }

    private IEnumerator DestroyHierarchy(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null)
        {
            Transform topParent = obj.transform;
            while (topParent.parent != null)
            {
                topParent = topParent.parent;
            }

            Destroy(topParent.gameObject);
        }
    }

    private IEnumerator DelayedSelfDestruct(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Self-Destruct: Destroying " + transform.parent.gameObject.name);
        Destroy(transform.parent.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (transform.parent == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.parent.position, eraseRadius);
    }
}