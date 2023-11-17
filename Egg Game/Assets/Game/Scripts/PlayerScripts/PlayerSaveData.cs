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

    [SyncVar]
    public int playerNumber;

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

    [SyncVar(hook = nameof(OnPlayerCountChanged))]
    public int playerCount = 0;

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
            for (int i = 0; i < playerCount; i++)
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
                        }
                    }
                }
            }
        }
    }

    public void ResetPlayerData(int connectionId)
    {
        int playerIndex = connectionId;

        if (playerIndex >= 0 && playerIndex < playerHatSpriteNumbers.Length)
        {
            playerHatSpriteNumbers[playerIndex] = 0;
            playerBodySpriteNumbers[playerIndex] = 0;
            playerWeaponSpriteNumbers[playerIndex] = 0;
            playerAnimatorNumbers[playerIndex] = 0;

            // Reset any other player-specific data as needed
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "CharacterSelection")
        {
            playerReadyNumber = 0;
        }
    }

    void OnPlayerCountChanged(int oldValue, int newValue)
    {
        playerCount = newValue;

        // Additional logic to handle player count changes on clients if needed
        Debug.Log("Player count changed to: " + playerCount);
    }
}