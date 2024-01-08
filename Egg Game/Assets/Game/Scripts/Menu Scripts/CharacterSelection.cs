using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;
using TMPro;
using Steamworks;

public class CharacterSelection : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHatValueChanged))]
    public int _hatValue = 0;

    [SyncVar(hook = nameof(OnAnimatorValueChanged))]
    public int _animatorValue = 0;

    [SyncVar(hook = nameof(OnBodyValueChanged))]
    public int _bodyValue = 0;

    [SyncVar(hook = nameof(OnWeaponValueChanged))]
    public int _weaponValue = 0;

    [SyncVar(hook = nameof(OnReadyStateChanged))]
    public bool _isReady = false;

    [SerializeField] private Sprite[] allHats;
    [SerializeField] private RuntimeAnimatorController[] allAnimators;
    [SerializeField] public Sprite[] allBodies;
    [SerializeField] private Sprite[] allWeapons;

    public bool isReady;

    private SpriteRenderer hatRenderer;
    private SpriteRenderer bodyRenderer;
    private SpriteRenderer weaponRenderer;

    [SerializeField] private Transform MapCanvas;

    [SerializeField] private GameObject MapCanvasHolder;
    [SerializeField] private Transform canvas;
    
    public PlayerSaveData playerSaveData;

    private const int MaxPlayers = 6;

    public string playerTag;
    
    [SerializeField]
    private SpriteRenderer cursor;

    public TMP_Text playerName = null;

    [SyncVar(hook = nameof(HandleSteamIdUpdate))] private ulong steamId;

    public void SetSteamId(ulong steamId){
        this.steamId = steamId;
    }

    private void HandleSteamIdUpdate(ulong oldSteamId, ulong newSteamId){
        var cSteamId = new CSteamID(newSteamId);
        
        playerName.text = SteamFriends.GetFriendPersonaName(cSteamId);
    }


    public override void OnStartClient()
    {
        base.OnStartClient();
        hatRenderer = transform.GetChild(0).GetChild(2).GetComponent<SpriteRenderer>();
        bodyRenderer = transform.GetChild(0).GetChild(3).GetComponent<SpriteRenderer>();
        weaponRenderer = transform.GetChild(0).GetChild(4).GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        AssignPlayerTag();
        StartCoroutine(WaitForPlayerSaveData());
        StartCoroutine(WaitForMapCanvas());
        canvas = GameObject.Find("Canvas").transform;
        this.transform.SetParent(canvas.transform);
        // Check if the mapButton GameObject exists before attempting to find the Button component
        GameObject backButton2 = GameObject.Find("Back Button 2");

        hatRenderer = transform.GetChild(0).GetChild(2).GetComponent<SpriteRenderer>();
        bodyRenderer = transform.GetChild(0).GetChild(3).GetComponent<SpriteRenderer>();
        weaponRenderer = transform.GetChild(0).GetChild(4).GetComponent<SpriteRenderer>();
    }

    void AssignPlayerTag()
    {
        string[] playerColors = { "Red", "Blue", "Yellow", "Green", "Purple", "Brown" };

        for (int i = 1; i <= MaxPlayers; i++)
        {
            playerTag = "Player" + i;

            // Check if an object with this tag already exists
            GameObject existingPlayer = GameObject.FindGameObjectWithTag(playerTag);

            if (existingPlayer == null)
            {
                // No existing player with this tag, it's available
                gameObject.tag = playerTag;
                // Assign color based on the index in playerColors array
                Color playerColor = GetPlayerColor(i - 1); // Subtract 1 from the index
                Debug.Log("Assigned tag: " + playerTag);
                cursor.color = playerColor;
                if(playerName == null || string.IsNullOrEmpty(playerName.text)){
                    playerName.text = gameObject.tag;
                    Debug.Log("Player Name set to tag: " + playerName);
                }
                break;
            }
        }
    }

    Color GetPlayerColor(int index)
    {
        Color[] colors = { Color.red, Color.blue, Color.yellow, Color.green, new Color(0.5f, 0, 0.5f), new Color(0.6f, 0.4f, 0.2f) };
        
        if (index < colors.Length)
        {
            return colors[index];
        }
        else
        {
            // Return a default color if the index is out of bounds
            return Color.white;
        }
    }

    private IEnumerator WaitForPlayerSaveData()
    {
        while (playerSaveData == null)
        {
            playerSaveData = FindObjectOfType<PlayerSaveData>();

            // if (playerSaveData != null)
            // {
            //     _hatValue = playerSaveData.playerHatSpriteNumbers[connectionToClient.connectionId];
            //     _bodyValue = playerSaveData.playerBodySpriteNumbers[connectionToClient.connectionId];
            //     _weaponValue = playerSaveData.playerWeaponSpriteNumbers[connectionToClient.connectionId];
            //     _animatorValue = playerSaveData.playerAnimatorNumbers[connectionToClient.connectionId];
            // }

            yield return null;
        }
    }


    IEnumerator WaitForMapCanvas() {
        while (MapCanvasHolder == null) {

            MapCanvasHolder = GameObject.Find("MapCanvasHolder");

            if (MapCanvasHolder != null)
            {
                // Get the first child of MapCanvasHolder
                MapCanvas = MapCanvasHolder.transform.GetChild(0);
            }

            if (isLocalPlayer){
                MapCanvas.gameObject.SetActive(false);
            }
            yield return null; // Wait for a frame before checking again
        }

        // Continue with your Start logic...
        canvas = GameObject.Find("Canvas").transform;
        this.transform.SetParent(canvas.transform);

        // Update the initial sprites for all player objects on the client
        if (isServer) {
            RpcUpdateSprites(_hatValue, _bodyValue, _weaponValue);
        }

        // Add this line to ensure the sprites update on start
        UpdateSprites();
    }

    [Command]
    private void CmdSetHatValue(int value)
    {
        _hatValue = value;
        RpcUpdateSprites(_hatValue, _bodyValue, _weaponValue);
        Debug.Log($"CmdSetHatValue called on server. New Hat Value: {_hatValue}");
    }

    [Command]
    private void CmdSetBodyValue(int bodyValue, int animatorValue)
    {
        _bodyValue = bodyValue;
        _animatorValue = animatorValue;
        RpcUpdateSprites(_hatValue, _bodyValue, _weaponValue);
        Debug.Log($"CmdSetBodyValue called on server. New Body Value: {_bodyValue}, New Animator Value: {_animatorValue}");
    }

    [Command]
    private void CmdSetWeaponValue(int value)
    {
        _weaponValue = value;
        RpcUpdateSprites(_hatValue, _bodyValue, _weaponValue);
        Debug.Log($"CmdSetWeaponValue called on server. New Weapon Value: {_weaponValue}");
    }

    [ClientRpc]
    private void RpcUpdateSprites(int hatValue, int bodyValue, int weaponValue)
    {
        Debug.Log("RpcUpdateSprites called on client.");
        _hatValue = hatValue;
        _bodyValue = bodyValue;
        _weaponValue = weaponValue;
        UpdateSprites();
    }

    private void UpdateSprites()
    {
        hatRenderer.sprite = allHats[_hatValue];
        bodyRenderer.sprite = allBodies[_bodyValue];
        weaponRenderer.sprite = allWeapons[_weaponValue];
    }

    private void OnHatValueChanged(int oldValue, int newValue)
    {
        UpdateSprites();
    }

    private void OnAnimatorValueChanged(int oldValue, int newValue)
    {
        // Update the animator value as needed
    }

    private void OnBodyValueChanged(int oldValue, int newValue)
    {
        UpdateSprites();
    }

    private void OnWeaponValueChanged(int oldValue, int newValue)
    {
        UpdateSprites();
    }


    public void PreviousHat()
    {
        if (isLocalPlayer)
        {
            if (_hatValue == 0)
            {
                _hatValue = allHats.Length - 1;
            }
            else
            {
                _hatValue -= 1;
            }
            CmdSetHatValue(_hatValue);
        }
    }

    public void NextHat()
    {
        if (isLocalPlayer)
        {
            if (_hatValue >= allHats.Length - 1)
            {
                _hatValue = 0;
            }
            else
            {
                _hatValue += 1;
            }
            CmdSetHatValue(_hatValue);
        }
    }

    public void PreviousBody()
    {
        if (isLocalPlayer)
        {
            if (_bodyValue == 0)
            {
                _bodyValue = allBodies.Length - 1;
                _animatorValue = allAnimators.Length - 1;
            }
            else
            {
                _bodyValue -= 1;
                _animatorValue -= 1;
            }
            CmdSetBodyValue(_bodyValue, _animatorValue);
        }
    }

    public void NextBody()
    {
        if (isLocalPlayer)
        {
            if (_bodyValue >= allBodies.Length - 1)
            {
                _bodyValue = 0;
                _animatorValue = 0;
            }
            else
            {
                _bodyValue += 1;
                _animatorValue += 1;
            }
            CmdSetBodyValue(_bodyValue, _animatorValue);
        }
    }

    public void PreviousWeapon()
    {
        if (isLocalPlayer)
        {
            if (_weaponValue == 0)
            {
                _weaponValue = allWeapons.Length - 1;
            }
            else
            {
                _weaponValue -= 1;
            }
            CmdSetWeaponValue(_weaponValue);
        }
    }

    public void NextWeapon()
    {
        if (isLocalPlayer)
        {
            if (_weaponValue >= allWeapons.Length - 1)
            {
                _weaponValue = 0;
            }
            else
            {
                _weaponValue += 1;
            }
            CmdSetWeaponValue(_weaponValue);
        }
    }
    
    public void Ready()
    {
        if (isLocalPlayer)
        {
            Debug.Log("Initial Trying to Ready/UnReady");
            CmdSetReadyState(!_isReady);
        }
    }

    [Command]
    public void CmdSetReadyState(bool readyState)
    {
        Debug.Log("Trying to Ready/UnReady");
        
        if (_isReady != readyState)
        {
            _isReady = readyState;

            if (_isReady)
            {
                RpcSaveCustomizationChoices(_hatValue, _bodyValue, _weaponValue, _animatorValue);
                
                playerSaveData.playerReadyNumber += 1;
                Debug.Log("Players Ready: " + playerSaveData.playerReadyNumber);

                // Send a command to activate the MapCanvas for the local player only
                TargetActivateMapCanvas(connectionToClient, true);
            }
            else
            {
                playerSaveData.playerReadyNumber -= 1;
                Debug.Log("Players Ready: " + playerSaveData.playerReadyNumber);
            }
        }
    }

    [TargetRpc]
    private void TargetActivateMapCanvas(NetworkConnection target, bool activate)
    {
        // Enable or disable the MapCanvas based on the 'activate' parameter
        MapCanvas.gameObject.SetActive(activate);
    }

    private void OnReadyStateChanged(bool oldValue, bool newValue)
    {
        isReady = newValue;
    }

    [ClientRpc]
    private void RpcSaveCustomizationChoices(int hatValue, int bodyValue, int weaponValue, int animatorValue)
    {
        Debug.Log("RpcSaveCustomizationChoices called on client.");

        // Extract the last digit from the playerTag
        char lastDigitChar = playerTag[playerTag.Length - 1];
        int lastDigit = int.Parse(lastDigitChar.ToString());

        for (int i = 0; i < playerSaveData.playerCount; i++)
        {
            playerSaveData.playerHatSpriteNumbers[lastDigit-1] = hatValue;
            playerSaveData.playerBodySpriteNumbers[lastDigit-1] = bodyValue;
            playerSaveData.playerWeaponSpriteNumbers[lastDigit-1] = weaponValue;
            playerSaveData.playerAnimatorNumbers[lastDigit-1] = animatorValue;
        }
    }
}
