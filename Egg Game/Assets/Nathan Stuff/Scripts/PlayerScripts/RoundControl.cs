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
    public Transform playerSpawnLocation;

// Start is called before the first frame update
    void Start()
    {
        playerSpawnLocation = GameObject.Find("SpawnPoint").transform;
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

    void Update()
    {
        if (placingItems)
        {
            Respawn = false;

            if (playersPlacedBlocks >= PlayerSaveData.playerNumber + 2)
            {
                itemsPlaced = true;
                placingItems = false;
            }
        }
        

        if (!placingItems)
        {
            for (int i = 0; i < player.Length; i++)
            {
                if (player[i] != null && player[i].becameKing)
                {
                    // Update player flags for the current king
                    for (int j = 0; j < player.Length; j++)
                    {
                        if (player[j] != null)
                        {
                            player[j].isKing = (j == i);
                            player[j].isPlayer = (j != i);
                        }
                    }

                    // Set respawn and round variables
                    Respawn = true;
                    Round += 1;
                    player[i].becameKing = false;
                    // Exit the loop since the king is found
                    break;
                }

                if (player[i] != null && player[i].isPlayer && Respawn)
                {
                    player[i].rigid.velocity = Vector2.zero;
                    player[i].transform.position = playerSpawnLocation.position;
                    itemsPlaced = false;
                    timerOn = false;
                    RoundTime = 360f;
                    playersPlacedBlocks = 0;
                    placingItems = true;

                    break;
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
            }
        }
    }
}