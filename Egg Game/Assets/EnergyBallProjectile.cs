using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnergyBallProjectile : NetworkBehaviour
{
    // public float speed = 15f; // Adjust the speed as desired
    // private Vector2 direction;

    // [SyncVar]
    // private Vector3 syncScale;

    // [SyncVar]
    // private Vector3 syncPosition;

    // private Collider2D energyBallCollider;

    // [SyncVar]
    // private Vector2 pushDirection;

    // public void SetDirection(Vector2 dir)
    // {
    //     direction = dir;
    // }

    // private void Awake()
    // {
    //     energyBallCollider = GetComponentInChildren<Collider2D>(); // Use the child collider of the SoundWave object
    // }

    // private void Start()
    // {
    //     // if (isServer)
    //     // {
    //     //     syncScale = transform.localScale;
    //     //     syncPosition = transform.position;
    //     // }

    //     StartCoroutine(IncreaseSizeAndDestroy());
    // }

    // [ServerCallback] // Only runs on the server
    // private void OnTriggerEnter2D(Collider2D other)
    // {

    // }

    // private IEnumerator IncreaseSizeAndDestroy()
    // {
    //     float elapsedTime = 0f;
    //     Vector3 initialScale = energyBallCollider.transform.localScale;
    //     Vector3 targetScale = new Vector3(8f, 8f, 8f);
    //     float duration = 2f;

    //     while (elapsedTime < duration)
    //     {
    //         float t = elapsedTime / duration;
    //         syncScale = Vector3.Lerp(initialScale, targetScale, t);
    //         elapsedTime += Time.deltaTime;
    //         yield return null;
    //     }

    //     NetworkServer.Destroy(gameObject);
    // }
}
