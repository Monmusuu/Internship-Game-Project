using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class RoundControl : NetworkBehaviour
{
    // SyncVars for network synchronization
    [SyncVar] public int Round = 0;
    [SyncVar] public bool timerOn = false;
    [SyncVar] public bool Respawn = false;
    [SyncVar] public bool itemsPlaced = false;
    [SyncVar] public bool placingItems = false;
    [SyncVar] public int playersPlacedBlocks = 0;
    [SyncVar] public float timerElapsed;
    [SyncVar] public float RoundTime = 360f;

    [SyncVar(hook = nameof(OnTimerImageFillAmountChanged))]
    public float timerImageFillAmount = 1.0f;

    [SyncVar(hook = nameof(OnRoundTimerImageFillAmountChanged))]
    public float roundTimerImageFillAmount = 1.0f;

    // Public variables
    public List<Player> players = new List<Player>();
    public Transform playerSpawnLocation;
    public Transform kingSpawnLocation;
    public Image timerImage;
    public Image roundTimerImage;

    // Private variables
    private PlayerSaveData playerSaveData;

    private void Start()
    {

        StartCoroutine(WaitForPlayerSaveData());
        playerSpawnLocation = GameObject.Find("SpawnPoints").transform;
        kingSpawnLocation = GameObject.Find("KingPoint").transform;
        timerImage = GameObject.Find("Timer").GetComponent<Image>();
        roundTimerImage = GameObject.Find("RoundTimer").GetComponent<Image>();
    }

    void Update()
    {
        //if(isServer){
            if (placingItems)
            {
                timerElapsed = 0f;
                Respawn = false;
                timerOn = false;

                if(NoKingAmongPlayers()){
                    if(playersPlacedBlocks >= playerSaveData.playerCount){
                        itemsPlaced = true;
                        placingItems = false;
                    }
                }else{
                    if(playersPlacedBlocks >= playerSaveData.playerCount +2){
                        itemsPlaced = true;
                        placingItems = false;
                    }
                }
                
            }else
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

                if(Round == 0 && timerOn == false)
                {
                    Debug.Log("Round 0");
                    placingItems = true;
                    // Start the coroutine to gradually decrease the fill amount
                    StartCoroutine(DecreaseTimerFillOverTime(3f));
                }

                if (itemsPlaced && Round >= 1 && timerOn == false)
                {
                    // Start the coroutine to gradually decrease the fill amount
                    StartCoroutine(DecreaseTimerFillOverTime(3f));
                    //Debug.Log("Inbetween Iimer");

                }else if(itemsPlaced && Round >= 1 && timerOn == true)
                {
                    RoundTime -= Time.deltaTime;

                    // Update the round timer image fill amount based on RoundTime
                    float fillAmountRound = RoundTime / 360f; // Assuming 360 is the max RoundTime
                    if (roundTimerImage != null)
                    {
                        roundTimerImageFillAmount = RoundTime / 360f; // Assuming 360 is the max RoundTime
                    }

                    if (AllPlayersExceptKingAreDead())
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
        //}
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

    private void OnTimerImageFillAmountChanged(float oldFillAmount, float newFillAmount)
    {
        // Update the fill amount of the timerImage on all clients
        if (timerImage != null)
        {
            timerImage.fillAmount = newFillAmount;
        }
    }

    private void OnRoundTimerImageFillAmountChanged(float oldFillAmount, float newFillAmount)
    {
        // Update the fill amount of the roundTimerImage on all clients
        if (roundTimerImage != null)
        {
            roundTimerImage.fillAmount = newFillAmount;
        }
    }

    private IEnumerator DecreaseTimerFillOverTime(float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;

        while (Time.time < endTime)
        {
            timerElapsed = Time.time - startTime;
            timerImageFillAmount = Mathf.Lerp(1f, 0f, timerElapsed / duration);

            yield return null;
        }

        timerImageFillAmount = 0f;
        placingItems = false;
        timerOn = true;

        yield break;
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

    // Check if there is no king among players
    private bool NoKingAmongPlayers()
    {
        foreach (Player player in players)
        {
            if (player != null && player.isKing)
            {
                return false; // There is a king among players
            }
        }
        return true; // No king among players
    }

    private bool AllPlayersExceptKingAreDead()
    {
        // Find the current king
        Player king = players.Find(player => player != null && player.isKing);

        // If there is no king or the king is null, return false
        if (king == null)
        {
            return false;
        }

        // Check if all players except the king are dead
        foreach (Player player in players)
        {
            if (player != null && player != king && !player.isDead)
            {
                return false; // At least one player (not the king) is still alive
            }
        }

        return true; // All players except the king are dead
    }
}
