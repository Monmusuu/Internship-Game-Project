using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RoundControl : NetworkBehaviour
{
    public List<Player> players = new List<Player>();
    public float RoundTime = 360f;
    [SyncVar]
    public int Round = 0;

    [SyncVar]
    public bool timerOn = false;

    [SyncVar]
    public bool Respawn = false;

    [SyncVar]
    public bool itemsPlaced = false;

    [SyncVar]
    public bool placingItems = false;

    [SerializeField][SyncVar]
    public int playersPlacedBlocks = 0;

    public bool playerRemovingItem = false;
    public Transform playerSpawnLocation;
    [SerializeField]
    private CustomNetworkManager customNetworkManager;


    // Start is called before the first frame update

    public override void OnStartServer()
    {
        base.OnStartServer();
        // Start the round timer
        timerOn = true;
    }

    // Method to add a player to the players list
    public void AddPlayer(Player newPlayer)
    {
        players.Add(newPlayer);
    }

    // Method to remove a player from the players list
    public void RemovePlayer(Player playerToRemove)
    {
        players.Remove(playerToRemove);
    }

    private void Start()
    {
        customNetworkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
        playerSpawnLocation = GameObject.Find("SpawnPoint").transform;
    }

    void Update()
    {
        if (placingItems)
        {
            Respawn = false;
            timerOn = false;

            if(playersPlacedBlocks >= customNetworkManager.playerCount +2){
                itemsPlaced = true;
            }
            
        }

        if (!placingItems)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] != null && players[i].becameKing)
                {
                    // Update player flags for the current king
                    for (int j = 0; j < players.Count; j++)
                    {
                        if (players[j] != null)
                        {
                            players[j].isKing = (j == i);
                            players[j].isPlayer = (j != i);
                        }
                    }

                    // Set respawn and round variables
                    Respawn = true;
                    Round += 1;
                    players[i].becameKing = false;
                    placingItems = true;
                    // Exit the loop since the king is found
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

        // Only the server should handle respawning and sync it across clients
        if (Respawn && isServer)
        {
            RespawnPlayers();
        }
    }

    [ClientRpc]
    private void RpcRespawnPlayer(NetworkIdentity playerNetIdentity)
    {
        // Reset player's velocity and position on all clients
        Player player = playerNetIdentity.GetComponent<Player>();
        if (player != null)
        {
            player.rigid.velocity = Vector2.zero;
            player.transform.position = playerSpawnLocation.position;
        }
    }

    private void RespawnPlayers()
    {
        foreach (Player player in players)
        {
            if (player != null && player.isPlayer)
            {
                // Respawn the player on all clients
                RpcRespawnPlayer(player.GetComponent<NetworkIdentity>());
            }
        }

        Respawn = false;
    }
}
