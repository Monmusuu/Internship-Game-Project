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

    private Dictionary<int, bool> hasVoted;
    private Dictionary<int, int> selectedMapIndex;
    private Dictionary<int, int[]> votes;
    private Vector2 movementInput;

    private void OnEnable()
    {
        InitializeVotingSystem();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        var inputDevices = InputSystem.devices;
        Debug.Log($"Input devices count: {inputDevices.Count}");
        // Process movement input
        Vector2 movement = movementInput.normalized;
        transform.Translate(movement * speed * Time.deltaTime);
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            int playerID = context.control.device.deviceId;

            if (!hasVoted.ContainsKey(playerID) || !hasVoted[playerID])
            {
                // Get the map objects with the "Map" tag
                GameObject[] mapObjects = GameObject.FindGameObjectsWithTag("Map");

                // Pass the mapObjects array as an argument to GetClosestMapIndex method
                selectedMapIndex[playerID] = GetClosestMapIndex(mapObjects);
                Debug.Log("Player " + playerID + " selected map index: " + selectedMapIndex[playerID]);

                hasVoted[playerID] = true;
                Debug.Log("Player " + playerID + " vote action triggered");

                // Increment the vote count for the selected map for the specific player
                votes[playerID][selectedMapIndex[playerID]]++;
                Debug.Log("Player " + playerID + " voted for map " + selectedMapIndex[playerID]);

                // Update the vote count display
                UpdateVoteCounts();
            }
            else
            {
                UndoVote(playerID);
            }
        }
    }

    private int GetClosestMapIndex(GameObject[] mapObjects)
    {
        int mapIndex = -1;
        float closestDistance = Mathf.Infinity;
        Vector3 playerPosition = transform.position;

        for (int i = 0; i < mapObjects.Length; i++)
        {
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

    public void UndoVote(int playerID)
    {
        if (hasVoted.ContainsKey(playerID) && hasVoted[playerID])
        {
            hasVoted[playerID] = false;

            // Decrement the vote count for the corresponding map index, but ensure it doesn't go below zero
            if (votes[playerID][selectedMapIndex[playerID]] > 0)
            {
                votes[playerID][selectedMapIndex[playerID]]--;
                Debug.Log("Player " + playerID + " vote for map " + selectedMapIndex[playerID] + " undone.");
            }

            // Update the vote count display
            UpdateVoteCounts();
        }
    }

    private void UpdateVoteCounts()
    {
        for (int i = 0; i < voteCounts.Length; i++)
        {
            int totalVotes = 0;

            foreach (var voteArray in votes.Values)
            {
                totalVotes += voteArray[i];
            }

            voteCounts[i].text = totalVotes.ToString();
        }
    }

    private void InitializeVotingSystem()
    {
        // Initialize other necessary variables
        hasVoted = new Dictionary<int, bool>();
        selectedMapIndex = new Dictionary<int, int>();
        votes = new Dictionary<int, int[]>();

        // Get the map objects with the "Map" tag
        GameObject[] mapObjects = GameObject.FindGameObjectsWithTag("Map");
        int numMaps = mapObjects.Length;

        // Initialize the dictionaries for each player
        foreach (var device in InputSystem.devices)
        {
            int playerID = device.deviceId;

            hasVoted.Add(playerID, false);
            selectedMapIndex.Add(playerID, -1);
            votes.Add(playerID, new int[numMaps]);
        }

        // Update the vote count display
        UpdateVoteCounts();
    }
}