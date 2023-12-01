using UnityEngine;
using System.Collections;

public class Eraser : MonoBehaviour
{
    public LayerMask eraseLayer;
    public float eraseRadius = 4f;
    public Animator animator;
    public Collider2D eraserCollider;

    private void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(eraserCollider.transform.position, eraseRadius, eraseLayer);
        bool isEraserOnEraserLayer = eraseLayer == (eraseLayer | (1 << eraserCollider.gameObject.layer));

        foreach (Collider2D collider in colliders)
        {
            if (ShouldDestroyObject(collider.gameObject))
            {
                //Debug.Log("Overlap: Destroying " + collider.gameObject.name);
                StartCoroutine(DestroyObject(collider.gameObject, 1f));
            }
        }

        if (isEraserOnEraserLayer)
        {
            StartCoroutine(DelayedSelfDestruct(2f));
        }

        // Check if there is any overlap and trigger animation
        if (colliders.Length > 0)
        {
            animator.SetTrigger("Activate");
        }
    }

    private bool ShouldDestroyObject(GameObject obj)
    {
        return obj.layer == eraserCollider.gameObject.layer && obj != eraserCollider.gameObject;
    }

    private IEnumerator DestroyObject(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null)
        {
            Destroy(obj);
        }
    }

    private IEnumerator DelayedSelfDestruct(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Self-Destruct: Destroying " + eraserCollider.gameObject.name);
        Destroy(eraserCollider.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (eraserCollider == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(eraserCollider.transform.position, eraseRadius);
    }
}
