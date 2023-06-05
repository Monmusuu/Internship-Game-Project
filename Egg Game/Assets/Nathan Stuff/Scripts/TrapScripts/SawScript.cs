using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawScript : MonoBehaviour
{
    public Transform pointA; // First point
    public Transform pointB; // Second point
    public float rotationSpeed = 360f; // Rotation speed in degrees per second
    public float movementSpeed = 2f; // Movement speed in units per second

    private Vector3 targetPosition; // Current target position

    private void Start()
    {
        targetPosition = pointB.position; // Start at point B
    }

    private void Update()
    {
        RotateObject();
        MoveObject();
    }

    private void RotateObject()
    {
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }

    private void MoveObject()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

        if (transform.position == targetPosition)
        {
            // Switch target position
            if (targetPosition == pointA.position)
                targetPosition = pointB.position;
            else
                targetPosition = pointA.position;
        }
    }
}
