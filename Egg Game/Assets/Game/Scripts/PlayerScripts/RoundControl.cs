using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

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
    public Transform kingSpawnLocation;

    [SerializeField]
    private PlayerSaveData playerSaveData;

    public Image timerImage;

    [SyncVar]
    public float timerElapsed;

    public Image roundTimerImage;


    // Start is called before the first frame update

    public override void OnStartServer()
    {
        base.OnStartServer();
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
    
    private IEnumerator WaitForPlayerSaveData()
    {
        while (playerSaveData == null)
        {
            playerSaveData = FindObjectOfType<PlayerSaveData>();
            yield return null;
        }
    }

    private void Start()
    {
        // Wait for the PlayerCounter to be spawned by the network manager
        StartCoroutine(WaitForPlayerSaveData());
        
        playerSpawnLocation = GameObject.Find("SpawnPoints").transform;
        kingSpawnLocation = GameObject.Find("KingPoint").transform;
        timerImage = GameObject.Find("Timer").GetComponent<Image>();
        roundTimerImage = GameObject.Find("RoundTimer").GetComponent<Image>();
    }

    void Update()
    {

        if (placingItems)
        {
            Respawn = false;
            timerOn = false;

            if(playersPlacedBlocks >= playerSaveData.playerCount +2){
                itemsPlaced = true;
                placingItems = false;

            }
            
        }

        if(Round == 0 && timerOn == false){
            placingItems = true;
            // Start the coroutine to gradually decrease the fill amount
            StartCoroutine(DecreaseTimerFillOverTime(3f));
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

                        if (j == i)
                        {
                            players[j].currentScore += 1;
                        }
                    }

                    // Set respawn and round variables
                    Respawn = true;
                    if (Respawn && isServer)
                    {
                        RespawnPlayers();
                    }
                    playersPlacedBlocks = 0;
                    itemsPlaced = false;
                    Round += 1;
                    players[i].becameKing = false;
                    placingItems = true;
                    // Exit the loop since the king is found
                    break;
                }
            }

            if (itemsPlaced && Round >= 1)
            {
                // Start the coroutine to gradually decrease the fill amount
                StartCoroutine(DecreaseTimerFillOverTime(3f));

                if (timerOn)
                {
                    RoundTime -= Time.deltaTime;
                }

                // Update the round timer image fill amount based on RoundTime
                float fillAmount = RoundTime / 360f; // Assuming 360 is the max RoundTime
                if (roundTimerImage != null)
                {
                    roundTimerImage.fillAmount = fillAmount;
                }
            }

            if (RoundTime <= 0)
                {
                    Respawn = true;
                    Debug.Log("Round Over");
                    if (Respawn && isServer)
                    {
                        RespawnPlayers();
                    }
                    playersPlacedBlocks = 0;
                    itemsPlaced = false;
                    Round += 1;
                    timerOn = false;
                    RoundTime = 360f;
                    placingItems = true;

                    // Check if there's a current king
                    Player currentKing = players.Find(player => player != null && player.isKing);

                    if (currentKing != null)
                    {
                        // Increase the currentScore of the current king
                        currentKing.currentScore += 1;
                    }
                }
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

    [ClientRpc]
    private void RpcRespawnKing(NetworkIdentity playerNetIdentity)
    {
        // Reset player's velocity and position on all clients
        Player player = playerNetIdentity.GetComponent<Player>();
        if (player != null)
        {
            player.rigid.velocity = Vector2.zero;
            player.transform.position = kingSpawnLocation.position;
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

            if (player != null && player.isKing)
            {
                // Respawn the player on all clients
                RpcRespawnKing(player.GetComponent<NetworkIdentity>());
            }
        }

        Respawn = false;
    }

    // Coroutine to decrease the fill amount over a specified duration
    private IEnumerator DecreaseTimerFillOverTime(float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;

        while (Time.time < endTime)
        {
            timerElapsed = Time.time - startTime;
            float fillAmount = Mathf.Lerp(1f, 0f, timerElapsed / duration);

            // Set the fill amount of the timerFillImage
            if (timerImage != null)
            {
                timerImage.fillAmount = fillAmount;
            }

            yield return null;
        }

        // Ensure the fill amount is exactly 0 at the end
        if (timerImage != null)
        {
            timerImage.fillAmount = 0f;
            timerElapsed = 0f;

            placingItems = false;
            timerOn = true;
        }
    }
}
