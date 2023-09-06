using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PlayerSaveData : MonoBehaviour
{
    private Scene scene;
    public CharacterSelection[] characterSelections;
    public Sprite[] allHats;
    public Sprite[] allBodies;
    public Sprite[] allWeapons;
    public RuntimeAnimatorController[] allAnimators; 

    public static int playerNumber = 0;
    public static int playerReadyNumber = 0;

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
<<<<<<< Updated upstream
        Debug.Log("Players: " + playerNumber);
        scene = SceneManager.GetActiveScene();

=======
        customNetworkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
>>>>>>> Stashed changes
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void Update()
    {
        scene = SceneManager.GetActiveScene();

        if (scene.name == "CharacterSelection")
        {
            for (int i = 0; i < customNetworkManager.playerCount; i++)
            {
                GameObject selectionObject = GameObject.FindGameObjectWithTag("Player" + (i + 1));
                if (selectionObject != null)
                {
                    CharacterSelection characterSelection = selectionObject.GetComponent<CharacterSelection>();
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
<<<<<<< Updated upstream

                            playerHatSpriteNumbers[i] = characterSelection.hatValue;
                            playerBodySpriteNumbers[i] = characterSelection.bodyValue;
                            playerWeaponSpriteNumbers[i] = characterSelection.weaponValue;
                            playerAnimatorNumbers[i] = characterSelection.animatorValue;

                            // Debug.Log("Hat " + playerHatSpriteNumbers[i]);
                            // Debug.Log("Body " + playerBodySpriteNumbers[i]);
                            // Debug.Log("Weapon " + playerWeaponSpriteNumbers[i]);
                            // Debug.Log("Animator " + playerAnimatorNumbers[i]);
=======
>>>>>>> Stashed changes
                        }
                    }
                }
            }
        }

        if (scene.name != "CharacterSelection" && scene.name != "MapSelection" && scene.name != "Menu")
        {

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
            //         playerTexts[i].SetActive(false);
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

<<<<<<< Updated upstream
=======


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

>>>>>>> Stashed changes
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "CharacterSelection")
        {
            playerNumber = 0;
            playerReadyNumber = 0;
        }
<<<<<<< Updated upstream

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

            playerTexts = new GameObject[]
            {
                GameObject.Find("Player One Text"),
                GameObject.Find("Player Two Text"),
                GameObject.Find("Player Three Text"),
                GameObject.Find("Player Four Text"),
                GameObject.Find("Player Five Text"),
                GameObject.Find("Player Six Text")
            };

            for (int i = playerNumber; i < players.Length; i++)
            {
                if (playerTexts[i] != null)
                    playerTexts[i].SetActive(false);
            }

            // Initialize the sprite arrays with the correct sizes
            playerSpriteHats = new Sprite[playerNumber];
            playerSpriteWeapons = new Sprite[playerNumber];
            playerSpriteBodies = new Sprite[playerNumber];

            // // Debug log to check saved values
            // Debug.Log("Player Hat Sprite Numbers: " + string.Join(", ", playerHatSpriteNumbers));
            // Debug.Log("Player Body Sprite Numbers: " + string.Join(", ", playerBodySpriteNumbers));
            // Debug.Log("Player Weapon Sprite Numbers: " + string.Join(", ", playerWeaponSpriteNumbers));

            for (int i = 0; i < playerNumber; i++)
            {
                playerSpriteHats[i] = allHats[playerHatSpriteNumbers[i]];
                playerSpriteWeapons[i] = allWeapons[playerWeaponSpriteNumbers[i]];
                playerSpriteBodies[i] = allBodies[playerBodySpriteNumbers[i]];

                players[i].transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteHats[i];
                players[i].transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteWeapons[i];
                players[i].transform.gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteBodies[i];

                Animator playerAnimator = players[i].GetComponent<Animator>();
                playerAnimator.runtimeAnimatorController = allAnimators[playerAnimatorNumbers[i]];

                 // Assign the sprite to a child object of the corresponding player text
                SpriteRenderer childSpriteRenderer = playerTexts[i].GetComponentInChildren<SpriteRenderer>();
                if (childSpriteRenderer != null)
                    childSpriteRenderer.sprite = playerSpriteBodies[i];
            }
            UnityEngine.Cursor.visible = false;
        }
=======
>>>>>>> Stashed changes
    }
}