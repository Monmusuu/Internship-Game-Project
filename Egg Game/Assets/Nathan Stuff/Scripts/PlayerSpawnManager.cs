using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnManager : MonoBehaviour
{
    public Transform[] startSpawnLocations;
    private Player player;

    void OnPlayerJoined(PlayerInput playerInput) {
        Debug.Log("PlayerInput ID: " + playerInput.playerIndex);
        
        playerInput.gameObject.GetComponent<PlayerDetails>().playerID = playerInput.playerIndex + 1;
        playerInput.gameObject.GetComponent<PlayerDetails>().startPos = startSpawnLocations[playerInput.playerIndex].position;
        
    }

}
