using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class OliveOil : NetworkBehaviour
{
    public float speed = 10f; // Adjust the speed as desired
    private Vector2 direction;


    [SyncVar]
    private Vector3 syncPosition;


    public void SetDirection(Vector2 dir)
    {
        direction = dir;
    }


    private void Start()
    {
        if (isServer)
        {
            syncPosition = transform.position;
        }

        StartCoroutine(Destroy());
    }

    private void Update()
    {
        if (isServer)
        {
            // Move the projectile in the specified direction
            syncPosition += (Vector3)(direction.normalized * speed * Time.deltaTime);

        }

        if (isClient)
        {
            transform.position = syncPosition;
        }
    }

    private IEnumerator Destroy()
    {
        float elapsedTime = 0f;
        float duration = 6f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        NetworkServer.Destroy(gameObject);
    }
}