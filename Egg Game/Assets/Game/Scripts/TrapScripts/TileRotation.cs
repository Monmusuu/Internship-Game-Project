using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TileRotation : NetworkBehaviour
{
    public RoundControl roundControl;

    [SerializeField]
    private float parentRotationSpeed = 40f;

    [SerializeField]
    private GameObject childPrefab; // Reference to the child prefab

    [SyncVar(hook = nameof(OnRotationStateChanged))]
    private float currentRotation = 0f;

    [SerializeField]
    private Transform[] childSpawnTransforms;
    private bool roundControlFound = false;

    IEnumerator WaitForRoundControl() {
        while (true) {
            GameObject roundControlObject = GameObject.Find("RoundControl(Clone)");

            if (roundControlObject != null) {
                roundControl = roundControlObject.GetComponent<RoundControl>();
                Debug.Log("RoundControl found!");
                roundControlFound = true;
                break;
            }

            yield return null; // Wait for a frame before checking again
        }
    }

    private void Start()
    {
        if (isServer)
        {
            SpawnChildObjects();
        }
    }

    [ServerCallback]
    private void Update()
    {

        if(!roundControlFound){
            Debug.Log("Looking for RoundControl");
            StartCoroutine(WaitForRoundControl());
        }

        if (roundControl.timerOn)
        {
            currentRotation += parentRotationSpeed * Time.deltaTime;
        }
    }

    private void OnRotationStateChanged(float oldRotation, float newRotation)
    {
        currentRotation = newRotation;

        transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);
    }

    private void FixedUpdate()
    {

        // Update the rotation of the parent
        transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);

        // Calculate child rotation speed to remain upright
        float childRotationSpeed = -parentRotationSpeed * Mathf.Deg2Rad;

        // Rotate the child objects to remain upright
        foreach (Transform childTransform in transform)
        {
            childTransform.rotation = Quaternion.identity; // Reset rotation
            childTransform.Rotate(Vector3.forward, childRotationSpeed * Time.deltaTime, Space.Self); // Rotate around local Z-axis
        }
    }

    [Server]
    private void SpawnChildObjects()
    {
        foreach (Transform spawnTransform in childSpawnTransforms)
        {
            Vector3 spawnPosition = spawnTransform.position;
            Quaternion spawnRotation = spawnTransform.rotation;

            GameObject spawnedChild = Instantiate(childPrefab, spawnPosition, spawnRotation);

            // Attach the ChildObject script to the spawned child
            ChildPlatform childScript = spawnedChild.GetComponent<ChildPlatform>();
            if (childScript != null)
            {
                childScript.ParentIdentity = this.netIdentity; // Assign the parent identity
            }
            else
            {
                Debug.LogWarning("ChildPlatform script not found on spawned child!");
            }

            NetworkServer.Spawn(spawnedChild);
        }
    }
}
