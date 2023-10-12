using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SawScript : NetworkBehaviour
{
    public Transform pointA; // First point
    public Transform pointB; // Second point
    public float rotationSpeed = 360f; // Rotation speed in degrees per second
    public float movementSpeed = 2f; // Movement speed in units per second
    public RoundControl roundControl;

    [SyncVar(hook = nameof(SyncPositionValues))]
    private Vector3 syncPosition; // Synchronized position across the network

    [SyncVar(hook = nameof(SyncRotationValues))]
    private Quaternion syncRotation; // Synchronized rotation across the network

    private Vector3 targetPosition; // Current target position

    private void Start()
    {
        roundControl = GameObject.Find("RoundControl").GetComponent<RoundControl>();
        targetPosition = pointB.position; // Start at point B

        if (isServer)
        {
            syncPosition = targetPosition;
            syncRotation = transform.rotation;
        }
    }

    private void Update()
    {
        if (!isServer)
        {
            // Interpolate the position and rotation on clients
            transform.position = Vector3.Lerp(transform.position, syncPosition, Time.deltaTime * movementSpeed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, syncRotation, rotationSpeed * Time.deltaTime);
            return;
        }

        if (roundControl.timerOn)
        {
            RotateObject();
            MoveObject();
        }
    }

    private void RotateObject()
    {
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        syncRotation = transform.rotation; // Update the synchronized rotation
    }

    private void MoveObject()
    {
        if (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
            syncPosition = transform.position; // Update the synchronized position
        }
        else
        {
            // Switch target position
            if (targetPosition == pointA.position)
                targetPosition = pointB.position;
            else
                targetPosition = pointA.position;

            // Update the synchronized position on the server
            syncPosition = targetPosition;
        }
    }

    // Hook method for syncing position
    public void SyncPositionValues(Vector3 oldValue, Vector3 newValue)
    {
        syncPosition = newValue;
    }

    // Hook method for syncing rotation
    public void SyncRotationValues(Quaternion oldValue, Quaternion newValue)
    {
        syncRotation = newValue;
    }
}
