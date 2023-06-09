using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAutoJoins : MonoBehaviour
{
    public PlayerSaveData playerSaveData; // Reference to the PlayerSaveData script
    public GameObject playerPrefab;
    public Canvas canvas; // Reference to the Canvas where the player should be spawned

    void Awake()
    {
        // Get the current player count from PlayerSaveData
        int playerCount = PlayerSaveData.playerNumber;

        // Calculate the middle position of the canvas
        Vector3 spawnPosition = canvas.transform.position;

        // Offset the spawn position based on the number of players
        float offsetX = (playerCount - 1) * 0.5f;
        spawnPosition.x -= offsetX;

        // Spawn players based on the player count
        for (int i = 0; i < playerCount; i++)
        {
            // Instantiate a player character prefab at the spawn position
            GameObject newPlayerPrefab = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            newPlayerPrefab.transform.SetParent(canvas.transform);
            spawnPosition.x += 1f;
            newPlayerPrefab.tag = "Player" + (i + 1);
        }
    }
}