using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Mirror;

public struct ChangeSceneMessage : NetworkMessage
{
    public string sceneName;
}

public class PlayerSaveData : NetworkBehaviour
{
    private Scene scene;
    public CharacterSelection[] characterSelections;
    public Sprite[] allHats;
    public Sprite[] allBodies;
    public Sprite[] allWeapons;
    public RuntimeAnimatorController[] allAnimators; 

    [SyncVar(hook = nameof(OnPlayerReadyNumberChanged))]
    public int playerReadyNumber = 0;

    public int[] playerHatSpriteNumbers;
    public int[] playerBodySpriteNumbers;
    public int[] playerWeaponSpriteNumbers;
    public int[] playerAnimatorNumbers;

    public Sprite[] playerSpriteHats;
    public Sprite[] playerSpriteBodies;
    public Sprite[] playerSpriteWeapons;

    public GameObject[] players;
    public GameObject[] playerTexts;
    public GameObject[] playerCursors;

    [SerializeField]
    private CustomNetworkManager customNetworkManager;

    private bool isSceneChanging = false;

    private static PlayerSaveData instance;
    public static PlayerSaveData Instance { get { return instance; } }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        customNetworkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
        Debug.Log("Players: " + customNetworkManager.playerCount);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnPlayerReadyNumberChanged(int oldValue, int newValue)
    {
        // Handle the playerReadyNumber change, if needed
    }
    private void Update()
    {
        scene = SceneManager.GetActiveScene();

        if (scene.name == "CharacterSelection")
        {
            for (int i = 0; i < customNetworkManager.playerCount; i++)
            {
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player" + (i + 1));
                if (playerObject != null)
                {
                    CharacterSelection characterSelection = playerObject.GetComponent<CharacterSelection>();
                    if (characterSelection != null)
                    {

                       if (characterSelection.isReady)
                        {
                            // Check if the arrays need resizing before accessing the indices
                            if (playerHatSpriteNumbers.Length <= i)
                                Array.Resize(ref playerHatSpriteNumbers, i + 1);
                            if (playerBodySpriteNumbers.Length <= i)
                                Array.Resize(ref playerBodySpriteNumbers, i + 1);
                            if (playerWeaponSpriteNumbers.Length <= i)
                                Array.Resize(ref playerWeaponSpriteNumbers, i + 1);
                            if (playerAnimatorNumbers.Length <= i)
                                Array.Resize(ref playerAnimatorNumbers, i + 1);

                            playerHatSpriteNumbers[i] = characterSelection._hatValue;
                            playerBodySpriteNumbers[i] = characterSelection._bodyValue;
                            playerWeaponSpriteNumbers[i] = characterSelection._weaponValue;
                            playerAnimatorNumbers[i] = characterSelection._animatorValue;
                        }
                    }
                }
            }

            if (playerReadyNumber == customNetworkManager.playerCount && playerReadyNumber >= 1 && !isSceneChanging)
            {
                isSceneChanging = true; // Set the flag to indicate a scene change is in progress
                StartCoroutine(ChangeSceneCoroutine());
            }
        }

        if (scene.name != "CharacterSelection" && scene.name != "MapSelection" && scene.name != "Menu")
        {
            players = new GameObject[]
            {
                GameObject.FindGameObjectWithTag("Player1"),
                GameObject.FindGameObjectWithTag("Player2"),
                GameObject.FindGameObjectWithTag("Player3"),
                GameObject.FindGameObjectWithTag("Player4"),
                GameObject.FindGameObjectWithTag("Player5"),
                GameObject.FindGameObjectWithTag("Player6")
            };

            playerSpriteHats = new Sprite[customNetworkManager.playerCount];
            playerSpriteWeapons = new Sprite[customNetworkManager.playerCount];
            playerSpriteBodies = new Sprite[customNetworkManager.playerCount];

            // playerTexts = new GameObject[]
            // {
            //     GameObject.Find("Player One Text"),
            //     GameObject.Find("Player Two Text"),
            //     GameObject.Find("Player Three Text"),
            //     GameObject.Find("Player Four Text"),
            //     GameObject.Find("Player Five Text"),
            //     GameObject.Find("Player Six Text")
            // };

            // for (int i = customNetworkManager.playerCount; i < players.Length; i++)
            // {
            //     if (playerTexts[i] != null)
            //         playerTexts[i].SetActive(true);
            // }

            // for (int i = 0; i < customNetworkManager.playerCount; i++)
            // {

            //      // Assign the sprite to a child object of the corresponding player text
            //     SpriteRenderer childSpriteRenderer = playerTexts[i].GetComponentInChildren<SpriteRenderer>();
            //     if (childSpriteRenderer != null)
            //         childSpriteRenderer.sprite = playerSpriteBodies[i];
            // }

        }
    }

    private IEnumerator ChangeSceneCoroutine()
    {
        yield return new WaitForSeconds(1f); // Delay the scene change to allow time for other network operations

        // Perform the scene change
        if (NetworkServer.active)
        {
            CustomNetworkManager customNetworkManager = FindObjectOfType<CustomNetworkManager>();
            if (customNetworkManager != null)
            {
                customNetworkManager.ServerChangeScene("MapSelection");
            }
        }
        else if (NetworkClient.active)
        {
            CustomNetworkManager customNetworkManager = FindObjectOfType<CustomNetworkManager>();
            if (customNetworkManager != null)
            {
                NetworkClient.connection.Send(new ChangeSceneMessage { sceneName = "MapSelection" });
            }
        }
    }



    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (NetworkClient.active && scene.name == "MapSelection")
        {
            //StartCoroutine(ChangeSceneCoroutine());
        }
        if (scene.name == "CharacterSelection")
        {
            playerReadyNumber = 0;
        }

        
    }
}