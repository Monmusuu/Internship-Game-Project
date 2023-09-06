using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SoundWave : NetworkBehaviour
{
    public float speed = 15f; // Adjust the speed as desired
    private Vector2 direction;

    [SyncVar]
    private Vector3 syncScale;

    [SyncVar]
    private Vector3 syncPosition;

    private Collider2D soundWaveCollider;

    [SyncVar]
    private Vector2 pushDirection;

    public void SetDirection(Vector2 dir)
    {
        direction = dir;
    }

    private void Awake()
    {
        soundWaveCollider = GetComponentInChildren<Collider2D>(); // Use the child collider of the SoundWave object
    }

    private void Start()
    {
        if (isServer)
        {
            syncScale = transform.localScale;
            syncPosition = transform.position;
        }

        StartCoroutine(IncreaseSizeAndDestroy());
    }

    private void Update()
    {
        if (isServer)
        {
            // Move the projectile in the specified direction
            syncPosition += (Vector3)(direction.normalized * speed * Time.deltaTime);
            pushDirection = direction.normalized * speed; // Set push direction for synchronization

        }

        if (isClient)
        {
            transform.localScale = syncScale;
            transform.position = syncPosition;
        }
    }

    [ServerCallback] // Only runs on the server
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (soundWaveCollider != null && (other.CompareTag("Player1") ||
            other.CompareTag("Player2") || other.CompareTag("Player3") ||
            other.CompareTag("Player4") || other.CompareTag("Player5") ||
            other.CompareTag("Player6")))
        {
            RpcApplyPushEffect(other.gameObject, pushDirection);
        }
    }

    [ClientRpc] // Synchronized across all clients
    private void RpcApplyPushEffect(GameObject playerObject, Vector2 pushDir)
    {
        Rigidbody2D otherRigidbody = playerObject.GetComponent<Rigidbody2D>();
        if (otherRigidbody != null)
        {
            otherRigidbody.AddForce(pushDir, ForceMode2D.Impulse);
        }

        Debug.Log("SoundWave collided with: " + playerObject.name);
    }

    private IEnumerator IncreaseSizeAndDestroy()
    {
        float elapsedTime = 0f;
        Vector3 initialScale = soundWaveCollider.transform.localScale;
        Vector3 targetScale = new Vector3(8f, 8f, 8f);
        float duration = 2f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            syncScale = Vector3.Lerp(initialScale, targetScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        NetworkServer.Destroy(gameObject);
    }
}
