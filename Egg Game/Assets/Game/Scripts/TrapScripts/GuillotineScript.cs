using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GuillotineScript : NetworkBehaviour
{
    public Transform pointA; // Starting point
    public Transform pointB; // Target point
    public float moveSpeedDown = 5f; // Speed of movement down to point B
    public float moveSpeedUp = 2f; // Speed of movement back up to point A

    [SyncVar]
    private bool isMovingDown = false; // Flag indicating if the object is moving down

    [SyncVar]
    private bool isMovingUp = false; // Flag indicating if the object is moving up

    [SyncVar]
    private Vector3 syncedPosition; // Synchronized position across the network

    private bool playedAudio = false;

    public AudioSource audioSource; // Reference to the AudioSource component
    public AudioClip audioClip; // The audio clip to be played

    private void Start()
    {
        if (isServer)
        {
            syncedPosition = transform.position;
        }
    }

    private void Update()
    {
        if (isServer)
        {
            if (isMovingDown)
            {
                MoveDown();
            }
            else if (isMovingUp)
            {
                MoveUp();
            }
        }
        else
        {
            // Interpolate position for clients
            transform.position = Vector3.Lerp(transform.position, syncedPosition, Time.deltaTime * 10f);
        }
    }

    public void ActivateFunction()
    {
        Debug.Log("ActivateFunction called");

        // Check if the object is already at pointA
        if (transform.position == pointA.position)
        {
            StartMovingDown();
        }
        else
        {
            Debug.Log("Object is not in position A");
            // Handle the case when the object is not in position A
        }
    }

    private void StartMovingDown()
    {
        if (!isServer) return;

        isMovingDown = true;
        isMovingUp = false;
    }

    private void MoveDown()
    {
        if (!isServer) return;

        if(playedAudio == false){
            audioSource.PlayOneShot(audioClip);
            playedAudio = true;
        }

        transform.position = Vector3.MoveTowards(transform.position, pointB.position, moveSpeedDown * Time.deltaTime);

        if (transform.position == pointB.position)
        {
            isMovingDown = false;
            isMovingUp = true;
        }

        syncedPosition = transform.position;
    }

    private void MoveUp()
    {
        if (!isServer) return;

        playedAudio = false;
        transform.position = Vector3.MoveTowards(transform.position, pointA.position, moveSpeedUp * Time.deltaTime);

        if (transform.position == pointA.position)
        {
            isMovingUp = false;
        }

        syncedPosition = transform.position;
    }
}
