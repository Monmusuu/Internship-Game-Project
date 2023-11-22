using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Teleporter : NetworkBehaviour
{
    [SyncVar]
    public int teleporterNumber;
    private Transform destination;
    public TeleporterReceiver[] receivers;
    public Teleporter[] teleporters;


    private void Update() {
        FindAndLinkReceiver();
    }

    private void FindAndLinkReceiver()
    {
        receivers = FindObjectsOfType<TeleporterReceiver>();
        foreach (TeleporterReceiver receiver in receivers)
        {
            if (receiver.receiverNumber == teleporterNumber)
            {
                destination = receiver.transform;
                break; // Stop searching once a matching receiver is found
            }
        }

        if (destination == null)
        {
            //Debug.LogError("No matching receiver found for teleporter number " + teleporterNumber);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Teleporter OnTriggerEnter2D triggered");
        // Check if the other collider has a tag ranging from "1" to "6" and if the current object is the server
        if (IsPlayerWithTag(other.tag) && isServer)
        {
            Debug.Log("Player with tag " + other.tag + " entered the teleporter trigger zone on the server.");
            TeleportPlayer(other.transform);
        }
    }

    // Helper method to check if a tag is in the range of "1" to "6"
    private bool IsPlayerWithTag(string tag)
    {
        // Assuming your player tags are "Player1", "Player2", ..., "Player6"
        for (int i = 1; i <= 6; i++)
        {
            if (tag == "Player" + i)
            {
                return true;
            }
        }
        return false;
    }


    [Server]
    void TeleportPlayer(Transform playerTransform)
    {
        if (destination != null)
        {
            Debug.LogError("Teleporting");
            playerTransform.position = destination.position;
        }
        else
        {
            Debug.LogError("Destination not set for teleporter number " + teleporterNumber);
        }
    }
}
