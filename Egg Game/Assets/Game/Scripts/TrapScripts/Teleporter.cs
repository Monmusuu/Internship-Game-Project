using System.Collections;
using UnityEngine;
using Mirror;

public class Teleporter : NetworkBehaviour
{
    [SyncVar]
    public int teleporterNumber;
    private Transform destination;
    private TeleporterReceiver receiver;  // Store the receiver reference directly
    public Animator animator;
    public RoundControl roundControl;

    private bool hasCheckedReceiver = false;

    private void Start()
    {
        StartCoroutine(WaitForRoundControl());
        StartCoroutine(FindAndLinkReceiver());
    }

    private void Update()
    {
        if (roundControl.timerOn && !hasCheckedReceiver)
        {
            StartCoroutine(CheckReceiverValidity());
            hasCheckedReceiver = true;
        }
        else if (!roundControl.timerOn)
        {
            hasCheckedReceiver = false; // Reset the flag when the timer is turned off
        }
    }

    IEnumerator FindAndLinkReceiver()
    {
        while (true)
        {
            TeleporterReceiver[] foundReceivers = FindObjectsOfType<TeleporterReceiver>();
            foreach (TeleporterReceiver foundReceiver in foundReceivers)
            {
                if (foundReceiver.receiverNumber == teleporterNumber)
                {
                    receiver = foundReceiver;
                    destination = receiver.transform;
                    break;
                }
            }

            if (receiver != null)
            {
                //Debug.Log("Receiver found for teleporter number " + teleporterNumber);
                break; // Exit the coroutine when the receiver is found
            }

            yield return null; // Optionally yield before restarting the loop
        }
    }

    IEnumerator WaitForRoundControl()
    {
        while (true)
        {
            GameObject roundControlObject = GameObject.Find("RoundControl(Clone)");

            if (roundControlObject != null)
            {
                roundControl = roundControlObject.GetComponent<RoundControl>();
                break;
            }

            yield return null; // Wait for a frame before checking again
        }
    }

    IEnumerator CheckReceiverValidity()
    {
        while (true)
        {
            // Check if the receiver is still valid, destroy the teleporter if it's not
            if (receiver != null && receiver.gameObject == null)
            {
                Debug.LogWarning("Destroying teleporter " + teleporterNumber + " because its receiver was destroyed.");
                Destroy(gameObject);
                break; // Exit the coroutine after destroying the teleporter
            }

            yield return new WaitForSeconds(1f); // Adjust the interval as needed
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Teleporter OnTriggerEnter2D triggered");
        // Check if the other collider has a tag ranging from "1" to "6" and if the current object is the server
        if (IsPlayerWithTag(other.tag))
        {
            animator.SetTrigger("Activate");
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

    void TeleportPlayer(Transform playerTransform)
    {
        if (destination != null)
        {
            Debug.Log("Teleporting");
            playerTransform.position = destination.position;
        }
        else
        {
            Debug.LogError("Destination not set for teleporter number " + teleporterNumber);
        }
    }
}
