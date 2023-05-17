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

    public int connectedPlayers = 0;

private void Start()
{
    Debug.Log("Players " + playerNumber);
    scene = SceneManager.GetActiveScene();

    if (scene.name != "CharacterSelection")
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

            if (i >= 1)
            {
                players[i].SetActive(false);
                playerTexts[i].SetActive(false);
            }
        }
    }

    //Debug.Log("Hat " + playerHatSpriteNumbers[0]);
}

    private void Update()
    {
        if (scene.name == "CharacterSelection")
        return;

        for (int i = 0; i < playerNumber; i++)
        {
                if (players[i].activeInHierarchy)
                {
                players[i].transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteHats[i];
                players[i].transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteWeapons[i];
                players[i].transform.gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteBodies[i];
                }
        }

    }
}
