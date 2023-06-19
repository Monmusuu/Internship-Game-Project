using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundControl : MonoBehaviour
{
    public Player[] player;
    public PlayerSaveData playerSaveData;
    public float RoundTime = 360f;
    public int Round = 0;
    public bool timerOn = false;
    public bool Respawn = false;
    public bool itemsPlaced = false;
    public bool placingItems = false;
    public int playersPlacedBlocks = 0;
    public bool playerRemovingItem = false;

// Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PopulatePlayerArray());
        timerOn = true;
    }

    IEnumerator PopulatePlayerArray()
    {
        yield return new WaitForEndOfFrame(); // Wait until the end of the frame

        player = new Player[6];
        for (int i = 0; i < 6; i++)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player" + (i + 1));
            if (playerObj != null)
            {
                Player playerComponent = playerObj.GetComponent<Player>();
                if (playerComponent != null)
                {
                    player[i] = playerComponent;
                }
                else
                {
                    Debug.LogError("Player component not found on game object with tag 'Player" + (i + 1) + "'!");
                }
            }
            else
            {
                //Debug.LogError("Player game object with tag 'Player" + (i + 1) + "' not found!");
            }
        }

        // Call a method or perform any logic that requires the 'player' array here
        // ...
    }

// Update is called once per frame
    void Update()
    {
        if(playersPlacedBlocks >= PlayerSaveData.playerNumber + 2){
            placingItems = false;
            itemsPlaced = true;
        }

        if (!placingItems)
        {
            for (int i = 0; i < player.Length; i++)
            {
                if (player[i] != null && player[i].becameKing)
                {
                    Respawn = true;
                    Round += 1;
                    RoundTime = 360f;
                    itemsPlaced = false;
                    timerOn = false;
                    placingItems = true;
                    break; // Exit the loop since the king is found
                }
            }

            if (itemsPlaced && Round >= 1)
            {

                timerOn = true;
                    
                if (timerOn)
                {
                    RoundTime -= Time.deltaTime;
                }

                if (RoundTime <= 0)
                {
                    Respawn = true;
                    Debug.Log("Round Over");
                    playersPlacedBlocks = 0;
                    itemsPlaced = false;
                    timerOn = false;
                    RoundTime = 360f;
                    placingItems = true;
                }

                if (RoundTime <= 360f && itemsPlaced)
                {
                    Respawn = false;
                }
            }
        }
        
    }
}
