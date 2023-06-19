using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OliveOil : MonoBehaviour
{
    public float speed = 10f; // Adjust the speed as desired
    private Vector3 direction;

    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }

    void Update()
    {
        // Move the projectile in the specified direction
        transform.Translate(direction.normalized * speed * Time.deltaTime);
    }
}
