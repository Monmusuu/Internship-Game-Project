using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AutoJoinPlayerPlayScene : MonoBehaviour
{
    public PlayerSaveData playerSaveData; // Reference to the PlayerSaveData script
    public GameObject playerPrefab;
    public Transform startLocation;
    public Transform playerHolder; // Reference to the player holder object

    void Awake()
    {
        // Get the current player count from PlayerSaveData
        int playerCount = playerSaveData.playerNumber;

        // Spawn players based on the player count
        for (int i = 0; i < playerCount; i++)
        {
            // Instantiate a player character prefab at the spawn position
            GameObject newPlayerPrefab = Instantiate(playerPrefab, startLocation.position, Quaternion.identity, playerHolder);
            // Set the tag for the spawned player prefab
            newPlayerPrefab.tag = "Player" + (i + 1);
        }
        
    }

}
