using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class MapSelection : MonoBehaviour
{
    public VotingSystem votingSystem;
    public float speed = 5f;

    private PlayerInput playerInput;
    private Vector2 movementInput;

    private void Start()
    {
        votingSystem = GameObject.Find("VotingSystem").GetComponent<VotingSystem>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (votingSystem.HasVoted(playerInput.playerIndex))
        {
            // Player has voted, stop movement
            return;
        }
        
        // Process movement input
        Vector2 movement = movementInput.normalized;
        transform.Translate(movement * speed * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            int playerID = playerInput.playerIndex;

            if (!votingSystem.HasVoted(playerID))
            {
                // Get the map objects with the "Map" tag
                GameObject[] mapObjects = GameObject.FindGameObjectsWithTag("Map");

                // Find the closest map index
                int closestMapIndex = GetClosestMapIndex(mapObjects);

                // Vote for the selected map
                votingSystem.Vote(playerID, closestMapIndex);
                Debug.Log("Player " + playerID + " voted for map index: " + closestMapIndex);
            }
            else
            {
                // Undo the player's vote
                votingSystem.UndoVote(playerID);
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
}