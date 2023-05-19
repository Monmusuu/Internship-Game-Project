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

    public GameObject[] playerCursors;

    public int connectedPlayers = 0;

private void Start()
{
    Debug.Log("Players " + playerNumber);
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

        for (int i = 0; i < players.Length; i++)
        {
            players[i].SetActive(i < playerNumber);
            playerTexts[i].SetActive(i < playerNumber);
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
    }
    else if (scene.name == "CharacterSelection")
    {
        playerNumber = 0;
        playerReadyNumber = 0;
    }

    if (scene.name == "MapSelection")
    {
        playerCursors = new GameObject[]
        {
            GameObject.Find("Cursor"),
            GameObject.Find("Cursor 2"),
            GameObject.Find("Cursor 3"),
            GameObject.Find("Cursor 4"),
            GameObject.Find("Cursor 5"),
            GameObject.Find("Cursor 6")
        };

        for (int i = 0; i < playerCursors.Length; i++)
        {
            playerCursors[i].SetActive(i < playerNumber);
        }
    }
}
}