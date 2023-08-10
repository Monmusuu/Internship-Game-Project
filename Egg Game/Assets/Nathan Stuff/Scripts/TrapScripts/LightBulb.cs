using UnityEngine;
using Mirror;

public class LightBulb : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnColorChange))]
    private Color syncedColor;

    public SpriteRenderer spriteRenderer;
    public Animator animator;

    private Color originalColor;

    private void Start()
    {
        originalColor = spriteRenderer.color;
        syncedColor = originalColor;
    }

    [Server]
    public void ActivateFunction(Sprite sprite)
    {
        SetSpriteWithOpacity(sprite);
    }

    [Server]
    public void SetSpriteWithOpacity(Sprite sprite)
    {
        syncedColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.90f);
        animator.SetTrigger("Darkness");
        StartCoroutine(ResetOpacityAfterDelay(20f));
    }

    [Server]
    private System.Collections.IEnumerator ResetOpacityAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        syncedColor = originalColor;
        animator.SetTrigger("Light has Come");
    }

    private void OnColorChange(Color oldColor, Color newColor)
    {
        spriteRenderer.color = newColor;
    }
}
