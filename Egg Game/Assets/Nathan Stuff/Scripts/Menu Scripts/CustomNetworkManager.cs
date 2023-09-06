using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

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

    [SerializeField]
    public int playerCount = 0; // This variable will store the current player count.

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
{
    string sceneName = SceneManager.GetActiveScene().name;

    foreach (ScenePlayerPrefabs scenePrefabs in scenePlayerPrefabs)
    {
        if (scenePrefabs.sceneName == sceneName)
        {
            GameObject playerPrefab = scenePrefabs.playerPrefab;
            GameObject player = Instantiate(playerPrefab);

            int playerId = conn.connectionId + 1;
            player.tag = "Player" + playerId;

            NetworkServer.AddPlayerForConnection(conn, player);

            // Set the player's position based on the round-robin logic
            Transform startPosition = GetStartPosition();
            if (startPosition != null)
            {
                player.transform.position = startPosition.position;
                player.transform.rotation = startPosition.rotation;
            }
            else
            {
                player.transform.position = Vector3.zero;
            }

            // Increment the player prefab index for the next player
            currentPlayerPrefabIndex = (currentPlayerPrefabIndex + 1) % scenePlayerPrefabs.Length;

            // Increment the player count when a new player joins.
            playerCount++;

            Debug.Log("Players: " + playerCount);

            PlayerSaveData.Instance.playerSpriteHats = new Sprite[playerCount];
            PlayerSaveData.Instance.playerSpriteWeapons = new Sprite[playerCount];
            PlayerSaveData.Instance.playerSpriteBodies = new Sprite[playerCount];

            return;
        }
    }

    Debug.LogError("No player prefab found for scene: " + sceneName);
}


    public override void OnServerChangeScene(string newSceneName)
    {
        base.OnServerChangeScene(newSceneName);

        playerCount = 0;
        currentPlayerPrefabIndex = 0; // Reset the player prefab index when changing scenes
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);

        // Decrease the player count when a player disconnects.
        playerCount--;
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
}