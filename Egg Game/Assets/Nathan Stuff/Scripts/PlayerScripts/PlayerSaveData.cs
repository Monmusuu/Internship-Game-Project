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
    public static int playerNumber = 0;
    public static int playerReadyNumber = 0;

    public int[] playerHatSpriteNumbers;
    public int[] playerBodySpriteNumbers;
    public int[] playerWeaponSpriteNumbers;

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
        Debug.Log("Players: " + playerNumber);
        scene = SceneManager.GetActiveScene();

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
        if (scene.name == "CharacterSelection")
        {
            characterSelections = new CharacterSelection[playerNumber]; // Initialize the array

            for (int i = 0; i < playerNumber; i++)
            {
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player" + (i + 1));
                if (playerObject != null)
                {
                    CharacterSelection characterSelection = playerObject.GetComponent<CharacterSelection>();
                    if (characterSelection != null)
                    {
                        characterSelections[i] = characterSelection; // Add the component to the array

                        if (characterSelection.isReady)
                        {
                            // Check if the arrays need resizing before accessing the indices
                            if (playerHatSpriteNumbers.Length <= i)
                                Array.Resize(ref playerHatSpriteNumbers, i + 1);
                            if (playerBodySpriteNumbers.Length <= i)
                                Array.Resize(ref playerBodySpriteNumbers, i + 1);
                            if (playerWeaponSpriteNumbers.Length <= i)
                                Array.Resize(ref playerWeaponSpriteNumbers, i + 1);

                            playerHatSpriteNumbers[i] = characterSelection.hatValue;
                            playerBodySpriteNumbers[i] = characterSelection.bodyValue;
                            playerWeaponSpriteNumbers[i] = characterSelection.weaponValue;

                            Debug.Log("Hat " + playerHatSpriteNumbers[i]);
                            Debug.Log("Body " + playerBodySpriteNumbers[i]);
                            Debug.Log("Weapon " + playerWeaponSpriteNumbers[i]);
                        }
                    }
                }
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "CharacterSelection")
        {
            playerNumber = 0;
            playerReadyNumber = 0;
        }

        if (scene.name != "CharacterSelection" && scene.name != "MapSelection")
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

                players[i].transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteHats[i];
                players[i].transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteWeapons[i];
                players[i].transform.gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteBodies[i];
            }
        }
    }
}