using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    private GameObject[] playerCursors;

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

        if (scene.name != "CharacterSelection" && scene.name != "MapSelection")
        {
            players = new GameObject[]
            {
                GameObject.Find("Player"),
                GameObject.Find("Player 2"),
                GameObject.Find("Player 3"),
                GameObject.Find("Player 4"),
                GameObject.Find("Player 5"),
                GameObject.Find("Player 6")
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
                if (players[i] != null)
                    players[i].SetActive(false);
                if (playerTexts[i] != null)
                    playerTexts[i].SetActive(false);
            }

            // Initialize the sprite arrays with the correct sizes
            playerSpriteHats = new Sprite[playerNumber];
            playerSpriteWeapons = new Sprite[playerNumber];
            playerSpriteBodies = new Sprite[playerNumber];

            // Initialize the sprite number arrays with the correct sizes
            playerHatSpriteNumbers = new int[playerNumber];
            playerBodySpriteNumbers = new int[playerNumber];
            playerWeaponSpriteNumbers = new int[playerNumber];

            for (int i = 0; i < playerNumber; i++)
            {
                playerHatSpriteNumbers[i] = 0;
                playerBodySpriteNumbers[i] = 0;
                playerWeaponSpriteNumbers[i] = 0;

                playerSpriteHats[i] = allHats[playerHatSpriteNumbers[i]];
                playerSpriteWeapons[i] = allWeapons[playerWeaponSpriteNumbers[i]];
                playerSpriteBodies[i] = allBodies[playerBodySpriteNumbers[i]];

                players[i].transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteHats[i];
                players[i].transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteWeapons[i];
                players[i].transform.gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteBodies[i];
            }
        }  else if (scene.name == "CharacterSelection")
        {
            playerNumber = 0;
            playerReadyNumber = 0;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MapSelection")
        {
            playerNumber = Mathf.Min(playerNumber, 6); // Limit playerNumber to 6

            // Find all cursors in the scene
            playerCursors = GameObject.FindGameObjectsWithTag("Cursor");

            // Activate or deactivate cursors based on playerNumber
            for (int i = 0; i < playerCursors.Length; i++)
            {
                playerCursors[i].SetActive(i < playerNumber);
            }

            for (int i = 0; i < playerNumber; i++)
            {
                players[i].transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteHats[i];
                players[i].transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteWeapons[i];
                players[i].transform.gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteBodies[i];
                playerCursors[i].SetActive(true);
            }

            for (int i = playerNumber; i < players.Length; i++)
            {
                players[i].SetActive(false);
                playerTexts[i].SetActive(false);
            }
        }
    }
}