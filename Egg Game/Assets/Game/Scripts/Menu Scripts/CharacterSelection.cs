using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;

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

    [SerializeField] private GameObject MapCanvas;
    [SerializeField] private Transform canvas;
    
    public PlayerSaveData playerSaveData;

    private bool gameManagerFound = false;
    private bool mapCanvasFound = false;

    [SerializeField]
    private CustomNetworkManager customNetworkManager;

    public override void OnStartClient()
    {
        base.OnStartClient();
        hatRenderer = transform.GetChild(0).GetChild(3).GetComponent<SpriteRenderer>();
        bodyRenderer = transform.GetChild(0).GetChild(4).GetComponent<SpriteRenderer>();
        weaponRenderer = transform.GetChild(0).GetChild(5).GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        customNetworkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
        canvas = GameObject.Find("Canvas").transform;
        this.transform.SetParent(canvas.transform);
        // Check if the mapButton GameObject exists before attempting to find the Button component
        GameObject backButton2 = GameObject.Find("Back Button 2");

        hatRenderer = transform.GetChild(0).GetChild(3).GetComponent<SpriteRenderer>();
        bodyRenderer = transform.GetChild(0).GetChild(4).GetComponent<SpriteRenderer>();
        weaponRenderer = transform.GetChild(0).GetChild(5).GetComponent<SpriteRenderer>();



        // if (playerSaveData != null)
        // {
        //     _hatValue = playerSaveData.playerHatSpriteNumbers[connectionToClient.connectionId];
        //     _bodyValue = playerSaveData.playerBodySpriteNumbers[connectionToClient.connectionId];
        //     _weaponValue = playerSaveData.playerWeaponSpriteNumbers[connectionToClient.connectionId];
        // }


    }

    private void Update() {
        if (!gameManagerFound) {
            StartCoroutine(WaitForGameManager());
        }

        if (!mapCanvasFound) {
            StartCoroutine(WaitForMapCanvas());
        }
    }

    IEnumerator WaitForGameManager() {
        while (true) {
            GameObject gameManager = GameObject.Find("GameManager(Clone)");

            if (gameManager != null) {
                playerSaveData = gameManager.GetComponent<PlayerSaveData>();
                Debug.Log("GameManager found!");
                gameManagerFound = true;
                break;
            }

            yield return null; // Wait for a frame before checking again
        }
    }

    IEnumerator WaitForMapCanvas() {
        while (true) {
            GameObject mapCanvas = GameObject.Find("Map Canvas");

            if (mapCanvas != null) {
                MapCanvas = mapCanvas;
                Debug.Log("MapCanvas found!");
                mapCanvasFound = true;
                break;
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
        MapCanvas.SetActive(activate);
    }

    private void OnReadyStateChanged(bool oldValue, bool newValue)
    {
        isReady = newValue;
    }

    [ClientRpc]
    private void RpcSaveCustomizationChoices(int hatValue, int bodyValue, int weaponValue, int animatorValue)
    {
        Debug.Log("RpcSaveCustomizationChoices called on client.");

        for (int i = 0; i < customNetworkManager.playerCount; i++)
        {
            GameObject selectionObject = GameObject.FindGameObjectWithTag("Player" + (i + 1));
            if (selectionObject != null)
            {
                // Update the customization choices for all clients
                playerSaveData.playerHatSpriteNumbers[connectionToClient.connectionId] = hatValue;
                playerSaveData.playerBodySpriteNumbers[connectionToClient.connectionId] = bodyValue;
                playerSaveData.playerWeaponSpriteNumbers[connectionToClient.connectionId] = weaponValue;
                playerSaveData.playerAnimatorNumbers[connectionToClient.connectionId] = animatorValue;
            }
        }
    }
}
