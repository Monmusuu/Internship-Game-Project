using System.Collections;
using UnityEngine;
using Mirror;

public class TeleporterReceiver : NetworkBehaviour
{
    [SyncVar]
    public int receiverNumber;
    public float destroyTimer = 2f; // Set a timer for destruction if not linked
    public RoundControl roundControl;
    private Teleporter teleporter;
    private bool isChecking = false;

    private void Start()
    {
        StartCoroutine(WaitForRoundControl());
    }

    private void Update()
    {
        if (roundControl.timerOn && !isChecking)
        {
            StartCoroutine(CheckTeleporterExistence());

        }else if (!roundControl.timerOn)
        {
            isChecking = false; // Reset the flag when the timer is turned off
        }
    }

    IEnumerator CheckTeleporterExistence()
    {
        isChecking = true;
        yield return new WaitForSeconds(destroyTimer);

        Teleporter[] teleporters = FindObjectsOfType<Teleporter>();
        bool foundMatchingTeleporter = false;

        foreach (Teleporter teleporter in teleporters)
        {
            if (teleporter.teleporterNumber == receiverNumber)
            {
                foundMatchingTeleporter = true;
                break;
            }
        }

        if (!foundMatchingTeleporter)
        {
            Debug.LogWarning("Destroying TeleporterReceiver " + receiverNumber + " because its corresponding teleporter was not found.");
            Destroy(gameObject);
        }

        isChecking = false;
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
}

