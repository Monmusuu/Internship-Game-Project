using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class MapSelection : MonoBehaviour
{
    public TMP_Text[] voteCounts;
    public float speed = 5f;

    private bool hasVoted;
    private int selectedMapIndex;
    private int[] votes;
    private Vector2 movementInput;

    private HashSet<int> votedMapIndices;

    private void OnEnable()
    {
        InitializeVotingSystem();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

public void OnClick(InputAction.CallbackContext context)
{
    if (!hasVoted)
    {
        hasVoted = true;
        Debug.Log("Vote action triggered");

        // Get the map objects with the "Map" tag
        GameObject[] mapObjects = GameObject.FindGameObjectsWithTag("Map");

        // Pass the mapObjects array as an argument to GetMapIndex method
        selectedMapIndex = GetMapIndex(mapObjects);
        Debug.Log("Selected map index: " + selectedMapIndex);

        if (!votedMapIndices.Contains(selectedMapIndex))
        {
            // Increment the vote count for the selected map
            votes[selectedMapIndex]++;
            Debug.Log("Voted for map " + selectedMapIndex);

            // Add the selected map index to the votedMapIndices set
            votedMapIndices.Add(selectedMapIndex);

            // Update the vote count display
            UpdateVoteCounts();
        }
    }
    else
    {
        UndoVote();
    }
}

    private int GetMapIndex(GameObject[] mapObjects)
    {
        int mapIndex = -1;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < mapObjects.Length; i++)
        {
            Vector3 playerPosition = transform.position;
            Vector3 mapObjectPosition = mapObjects[i].transform.position;
            float distance = Vector3.Distance(playerPosition, mapObjectPosition);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                mapIndex = i;
            }
        }

        return mapIndex;
    }

private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.CompareTag("Map") && hasVoted)
    {
        // Get the map objects with the "Map" tag
        GameObject[] mapObjects = GameObject.FindGameObjectsWithTag("Map");

        // Get the map index from the collided map object
        Map map = collision.GetComponent<Map>();
        int mapIndex = GetMapIndex(mapObjects);

        // Increment the vote count for the corresponding map index
        votes[mapIndex]++;
        Debug.Log("Voted for map " + mapIndex);

        // Update the vote count display
        UpdateVoteCounts();
    }
}

public void UndoVote()
{
    if (hasVoted)
    {
        hasVoted = false;

        // Get the map index based on the current position or other logic
        GameObject[] mapObjects = GameObject.FindGameObjectsWithTag("Map");
        int mapIndex = GetMapIndex(mapObjects);

        // Decrement the vote count for the corresponding map index
        votes[mapIndex]--;
        Debug.Log("Vote for map " + mapIndex + " undone.");

        // Update the vote count display
        UpdateVoteCounts();
    }
}

    private void UpdateVoteCounts()
    {
        for (int i = 0; i < voteCounts.Length; i++)
        {
            voteCounts[i].text = votes[i].ToString();
        }
    }

    private void Update()
    {
        if (!hasVoted)
        {
            // Process movement input
            Vector2 movement = movementInput.normalized;
            transform.Translate(movement * speed * Time.deltaTime);
        }
    }

    private void InitializeVotingSystem()
    {
        // Initialize other necessary variables
        hasVoted = false;

        // Initialize the votedMapIndices set
        votedMapIndices = new HashSet<int>();

        // Initialize the votes array
        GameObject[] mapObjects = GameObject.FindGameObjectsWithTag("Map");
        votes = new int[mapObjects.Length]; // Assuming mapObjects is an array that holds the map objects

        // Update the vote count display
        UpdateVoteCounts();
    }

    // Example code for resetting the voting system
    private void ResetVotingSystem()
    {
        // Reset vote counts and other variables
        votes = new int[votes.Length];
        hasVoted = false;

        // Update the vote count display
        UpdateVoteCounts();
    }

    // Example code for starting the voting process
    private void StartVoting()
    {
        InitializeVotingSystem();

        // Enable voting button and other necessary UI elements
        // voteButton.interactable = true;
    }

    // Example code for ending the voting process
    private void EndVoting()
    {
        // Disable voting button and other necessary UI elements
        // voteButton.interactable = false;

        // Perform any actions based on the final vote count, such as determining the winning map
    }

    // Example code for triggering the voting process
    private void TriggerVoting()
    {
        StartVoting();
    }

    // Example code for resetting and triggering the voting process again
    private void RestartVoting()
    {
        ResetVotingSystem();
        TriggerVoting();
    }

    // Example code for other methods and functionality as needed
}