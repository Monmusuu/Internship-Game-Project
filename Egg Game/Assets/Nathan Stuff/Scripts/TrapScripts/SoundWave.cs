using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWave : MonoBehaviour
{
    public float speed = 15f; // Adjust the speed as desired
    private Vector2 direction;

    private Collider2D soundWaveCollider;

    public void SetDirection(Vector2 dir)
    {
        direction = dir;
    }

    void Start()
    {
        soundWaveCollider = GetComponentInChildren<Collider2D>(); // Use the child collider of the SoundWave object
        StartCoroutine(IncreaseSizeAndDestroy());
    }

    void Update()
    {
        // Move the projectile in the specified direction
        transform.Translate(direction.normalized * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (soundWaveCollider != null && (other.CompareTag("Player1") ||
            other.CompareTag("Player2") || other.CompareTag("Player3") ||
            other.CompareTag("Player4") || other.CompareTag("Player5") ||
            other.CompareTag("Player6")))
        {
            Rigidbody2D otherRigidbody = other.gameObject.GetComponent<Rigidbody2D>();
            if (otherRigidbody != null)
            {
                Vector2 pushDirection = direction.normalized;
                otherRigidbody.AddForce(pushDirection * speed, ForceMode2D.Impulse);
            }

            Debug.Log("SoundWave collided with: " + other.gameObject.name);
        }
    }

    IEnumerator IncreaseSizeAndDestroy()
    {
        float elapsedTime = 0f;
        Vector3 initialScale = soundWaveCollider.transform.localScale;
        Vector3 targetScale = new Vector3(8f, 8f, 8f);
        float duration = 2f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            soundWaveCollider.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
