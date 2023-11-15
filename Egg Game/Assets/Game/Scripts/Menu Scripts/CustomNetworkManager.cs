using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;
public class CustomNetworkManager : NetworkManager
{
    [System.Serializable]
    public struct ScenePlayerPrefabs
    {
        public string sceneName;
        public GameObject playerPrefab;
    }
    
    public ScenePlayerPrefabs[] scenePlayerPrefabs;
    private int currentPlayerPrefabIndex = 0;

    public GameObject GameManager;
    public GameObject VotingSystem;
    public GameObject RoundControl;
    private PlayerCounter playerCounter; // Reference to PlayerCounter script
    public GameObject playerCounterPrefab;

    private Dictionary<NetworkConnectionToClient, Transform> playerSpawnPositions = new Dictionary<NetworkConnectionToClient, Transform>();

    [SerializeField]
    private SteamLobby steamLobby; // Reference to the SteamLobby script.

    public override void OnStartServer()
    {
        // Instantiate the PlayerCounter script on the server
        playerCounter = InstantiatePlayerCounter();

        base.OnStartServer();
    }
    

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        string sceneName = SceneManager.GetActiveScene().name;

        foreach (ScenePlayerPrefabs scenePrefabs in scenePlayerPrefabs)
        {
            if (scenePrefabs.sceneName == sceneName)
            {
                GameObject playerPrefab = scenePrefabs.playerPrefab;

                // Find the first available spawn position
                Transform startPosition = GetAvailableStartPosition();

                if (startPosition != null)
                {
                    // Mark this spawn position as occupied
                    playerSpawnPositions[conn] = startPosition;

                    // Instantiate the player at the chosen spawn position
                    GameObject player = Instantiate(playerPrefab, startPosition.position, startPosition.rotation);

                    int playerId = conn.connectionId + 1;
                    player.tag = "Player" + playerId;

                    NetworkServer.AddPlayerForConnection(conn, player);

                    // Increment the player prefab index for the next player
                    currentPlayerPrefabIndex = (currentPlayerPrefabIndex + 1) % scenePlayerPrefabs.Length;

                    // Increment the player count when a new player joins.
                    playerCounter.playerCount++;

                    Debug.Log("Players: " + playerCounter.playerCount);

                    // Initialize player-specific data here

                    return;
                }
                else
                {
                    Debug.LogError("No available spawn positions found.");
                }
            }
        }

        Debug.LogError("No player prefab found for scene: " + sceneName);
    }

    public override void OnServerChangeScene(string newSceneName)
    {
        base.OnServerChangeScene(newSceneName);

        currentPlayerPrefabIndex = 0; // Reset the player prefab index when changing scenes

        if(playerCounter !=null){
            playerCounter.playerCount = 0; // Reset the player count
        }

        Debug.Log("Changing scene to: " + newSceneName);

        // Extract the scene name without the path
        string sceneNameWithoutPath = System.IO.Path.GetFileNameWithoutExtension(newSceneName);

        if (sceneNameWithoutPath == "CharacterSelection")
        {
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the sceneLoaded event
        }

        if (sceneNameWithoutPath != "CharacterSelection")
        {
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the sceneLoaded event
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "CharacterSelection")
        {
            Debug.Log("Instantiating GameManager");
            GameObject GameManagerObject = Instantiate(GameManager);
            if (GameManagerObject != null)
            {
                NetworkServer.Spawn(GameManagerObject);
                Debug.Log("GameManager spawned on the server.");
            }
            else
            {
                Debug.LogError("GameManagerObject is null");
            }

            Debug.Log("Instantiating VotingSystem");
            GameObject VotingSystemObject = Instantiate(VotingSystem);
            if (VotingSystemObject != null)
            {
                NetworkServer.Spawn(VotingSystemObject);
                Debug.Log("VotingSystem spawned on the server.");
            }
            else
            {
                Debug.LogError("VotingSystemObject is null");
            }

            SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from the event
        }

        if (scene.name == "Nathan")
        {
            Debug.Log("Instantiating RoundControl");
            GameObject RoundControlObject = Instantiate(RoundControl);
            if (RoundControlObject != null)
            {
                NetworkServer.Spawn(RoundControlObject);
                Debug.Log("RoundControl spawned on the server.");
            }
            else
            {
                Debug.LogError("RoundControlObject is null");
            }

            SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from the event
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        // Increment the player count when a new player joins.
        playerCounter.playerCount--;

        // Free up the spawn position of the disconnected player
        if (playerSpawnPositions.ContainsKey(conn))
        {
            playerSpawnPositions.Remove(conn);
        }

        // Reset player data based on connection ID
        PlayerSaveData playerSaveData = PlayerSaveData.Instance;
        if (playerSaveData != null)
        {
            playerSaveData.ResetPlayerData(conn.connectionId);
        }else{
            Debug.Log("Reference not found");
        }

        base.OnServerDisconnect(conn);
    }

     public override void OnStopClient()
    {
        steamLobby.RequestLeaveLobby();

        base.OnStopClient();
    }

    public void ClientChangeScene(string sceneName)
    {
        if (NetworkClient.isConnected)
        {
            if (clientLoadedScene)
            {
                Debug.LogWarning("ClientChangeScene: Already in a scene, cannot change scene while in a scene.");
                return;
            }

            if (NetworkClient.ready)
            {
                Debug.LogWarning("ClientChangeScene: Scene operation already in progress, cannot change scene.");
                return;
            }

            if (NetworkServer.active)
            {
                Debug.LogWarning("ClientChangeScene: Cannot change scene while acting as a server.");
                return;
            }

            Debug.Log($"ClientChangeScene: Requesting scene change to '{sceneName}'.");
            NetworkClient.connection.Send(new ChangeSceneMessage { sceneName = sceneName });
        }
        else
        {
            Debug.LogWarning("ClientChangeScene: Client is not connected.");
        }
    }

    // Helper method to find an available spawn position
    private Transform GetAvailableStartPosition()
    {
        foreach (Transform startPosition in startPositions)
        {
            bool positionOccupied = playerSpawnPositions.ContainsValue(startPosition);
            if (!positionOccupied)
            {
                return startPosition;
            }
        }
        return null;
    }

    private PlayerCounter InstantiatePlayerCounter()
    {
        // Instantiate the PlayerCounter prefab
        GameObject playerCounterObj = Instantiate(playerCounterPrefab);

        // Spawn the object on the network
        NetworkServer.Spawn(playerCounterObj);

        // Prevent the object from being destroyed on scene change
        DontDestroyOnLoad(playerCounterObj);

        // Retrieve the PlayerCounter script from the instantiated object
        var counterScript = playerCounterObj.GetComponent<PlayerCounter>();

        return counterScript;
    }
}