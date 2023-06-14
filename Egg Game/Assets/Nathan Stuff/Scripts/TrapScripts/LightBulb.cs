using UnityEngine;

public class LightBulb : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    private Color originalColor;

    private void Start()
    {
        originalColor = spriteRenderer.color;
    }

    public void ActivateFunction(Sprite sprite)
    {
        Debug.Log("ActivateFunction called");
        SetSpriteWithOpacity(sprite);
    }

    public void SetSpriteWithOpacity(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.90f);

        animator.SetTrigger("Darkness");

        StartCoroutine(ResetOpacityAfterDelay(20f));
    }

    private System.Collections.IEnumerator ResetOpacityAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        spriteRenderer.color = originalColor;

        animator.SetTrigger("Light has Come");
    }
}
