using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class CharacterSelection : MonoBehaviour
{
<<<<<<< Updated upstream
    PlayerSaveData playerSaveData;
    [SerializeField] private Sprite playerSpriteHat;
    [SerializeField] private Sprite playerSpriteBody;
    [SerializeField] private Sprite playerSpriteWeapon;
=======
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
>>>>>>> Stashed changes

    public Transform canvas;

<<<<<<< Updated upstream
    public Sprite[] allHats;
    private int _HatValue = 0;
    public int hatValue
=======
    [SerializeField]
    private CustomNetworkManager customNetworkManager;

    public override void OnStartClient()
>>>>>>> Stashed changes
    {
        get => _HatValue;
        set
        {
            _HatValue = value;
            playerSpriteHat = allHats[_HatValue];
        }
    }

    [SerializeField] private RuntimeAnimatorController[] allAnimators;
    private int _AnimatorValue = 0;
    public int animatorValue
    {
        get => _AnimatorValue;
        set
        {
            _AnimatorValue = value;
        }
    }

    [SerializeField] public Sprite[] allBodies;
    private int _BodyValue = 0;
    public int bodyValue
    {
        get => _BodyValue;
        set
        {
            _BodyValue = value;
            playerSpriteBody = allBodies[_BodyValue];
        }
    }

    [SerializeField] private Sprite[] allWeapons;
    private int _WeaponValue = 0;
    public int weaponValue
    {
        get => _WeaponValue;
        set
        {
            _WeaponValue = value;
            playerSpriteWeapon = allWeapons[_WeaponValue];
        }
    }

    public bool isReady = false;

    void Start()
    {
        customNetworkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
        canvas = GameObject.Find("Canvas").transform;
        playerSpriteHat = transform.GetChild(0).GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite;
        playerSpriteBody = transform.GetChild(0).GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite;
        playerSpriteWeapon = transform.GetChild(0).GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite;
        this.transform.SetParent(canvas.transform);
    }

    private void Update() {
        if (PlayerSaveData.playerReadyNumber == PlayerSaveData.playerNumber && PlayerSaveData.playerNumber >= 1)
        {
            Debug.Log("Player is readied up. Moving to the next scene...");
            SceneManager.LoadScene("MapSelection"); // Replace "NextSceneName" with the actual name of the next scene
        }
    }
    public void PreviousHat(){
        if (_HatValue == 0)
        {
            _HatValue = allHats.Length - 1;
        }
        else
        {
            _HatValue -= 1;
        }
        playerSpriteHat = allHats[hatValue];
        transform.GetChild(0).GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteHat;
    }

    public void NextHat(){
        if (_HatValue >= allHats.Length - 1)
        {
            _HatValue = 0;
        }
        else
        {
            _HatValue += 1;
        }
        playerSpriteHat = allHats[hatValue];
        transform.GetChild(0).GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteHat;
    }

    public void PreviousBody(){
        if (_BodyValue == 0)
        {
            _BodyValue = allBodies.Length - 1;
            _AnimatorValue = allAnimators.Length - 1;
        }
        else
        {
            _BodyValue -= 1;
            _AnimatorValue -=1;
        }
        playerSpriteBody = allBodies[bodyValue];
        transform.GetChild(0).GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteBody;
    }

    public void NextBody(){
        if (_BodyValue >= allBodies.Length - 1)
        {
            _BodyValue = 0;
            _AnimatorValue = 0;
        }
        else
        {
            _BodyValue += 1;
            _AnimatorValue +=1;
        }
        playerSpriteBody = allBodies[bodyValue];
        transform.GetChild(0).GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteBody;
    }

    public void PreviousWeapon(){
        if (_WeaponValue == 0)
        {
            _WeaponValue = allWeapons.Length - 1;
        }
        else
        {
            _WeaponValue -= 1;
        }
        playerSpriteWeapon = allWeapons[weaponValue];
        transform.GetChild(0).GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteWeapon;
    }

    public void NextWeapon(){
        if (_WeaponValue >= allWeapons.Length - 1)
        {
            _WeaponValue = 0;
        }
        else
        {
            _WeaponValue += 1;
        }
        playerSpriteWeapon = allWeapons[weaponValue];
        transform.GetChild(0).GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteWeapon;
    }

    public void Ready(){
        isReady = !isReady;

        if(isReady){
            PlayerSaveData.playerReadyNumber += 1;
        }else{
            PlayerSaveData.playerReadyNumber -= 1;
        }
    }
<<<<<<< Updated upstream
=======

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
            CmdSetReadyState(!_isReady);
        }
    }

    [Command]
    private void CmdSetReadyState(bool readyState)
    {
        if (_isReady != readyState)
        {
            _isReady = readyState;

            if (_isReady)
            {
                RpcSaveCustomizationChoices(_hatValue, _bodyValue, _weaponValue, _animatorValue);

                playerSaveData.playerReadyNumber += 1;
                Debug.Log("Players Ready: " + playerSaveData.playerReadyNumber);
            }
            else
            {
                playerSaveData.playerReadyNumber -= 1;
                Debug.Log("Players Ready: " + playerSaveData.playerReadyNumber);
            }
        }
    }

    private void OnReadyStateChanged(bool oldValue, bool newValue)
    {
        isReady = newValue;
    }

    [ClientRpc]
    private void RpcSaveCustomizationChoices(int hatValue, int bodyValue, int weaponValue, int animatorValue)
    {
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
>>>>>>> Stashed changes
}
