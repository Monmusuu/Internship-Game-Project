using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Mirror;

public class CharacterSelection : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHatValueChanged))]
    private int _hatValue = 0;

    [SyncVar(hook = nameof(OnAnimatorValueChanged))]
    private int _animatorValue = 0;

    [SyncVar(hook = nameof(OnBodyValueChanged))]
    private int _bodyValue = 0;

    [SyncVar(hook = nameof(OnWeaponValueChanged))]
    private int _weaponValue = 0;

    [SyncVar(hook = nameof(OnReadyStateChanged))]
    private bool _isReady = false;

    [SerializeField] private Sprite[] allHats;
    [SerializeField] private RuntimeAnimatorController[] allAnimators;
    [SerializeField] public Sprite[] allBodies;
    [SerializeField] private Sprite[] allWeapons;

    public bool isReady;

    private SpriteRenderer hatRenderer;
    private SpriteRenderer bodyRenderer;
    private SpriteRenderer weaponRenderer;

    public Transform canvas;
    
    public PlayerSaveData playerSaveData;

    public override void OnStartClient()
    {
        base.OnStartClient();
        hatRenderer = transform.GetChild(0).GetChild(3).GetComponent<SpriteRenderer>();
        bodyRenderer = transform.GetChild(0).GetChild(4).GetComponent<SpriteRenderer>();
        weaponRenderer = transform.GetChild(0).GetChild(5).GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        canvas = GameObject.Find("Canvas").transform;
        this.transform.SetParent(canvas.transform);
        playerSaveData = GameObject.Find("GameManager").GetComponent<PlayerSaveData>();

        hatRenderer = transform.GetChild(0).GetChild(3).GetComponent<SpriteRenderer>();
        bodyRenderer = transform.GetChild(0).GetChild(4).GetComponent<SpriteRenderer>();
        weaponRenderer = transform.GetChild(0).GetChild(5).GetComponent<SpriteRenderer>();

        // Update the initial sprites for all player objects on the client
        if (isServer)
        {
            RpcUpdateSprites(_hatValue, _bodyValue, _weaponValue);
        }
    }

    [Command]
    private void CmdSetHatValue(int value)
    {
        _hatValue = value;
        RpcUpdateSprites(_hatValue, _bodyValue, _weaponValue);
    }

    [Command]
    private void CmdSetBodyValue(int bodyValue, int animatorValue)
    {
        _bodyValue = bodyValue;
        _animatorValue = animatorValue;
        RpcUpdateSprites(_hatValue, _bodyValue, _weaponValue);
    }

    [Command]
    private void CmdSetWeaponValue(int value)
    {
        _weaponValue = value;
        RpcUpdateSprites(_hatValue, _bodyValue, _weaponValue);
    }

    [ClientRpc]
    private void RpcUpdateSprites(int hatValue, int bodyValue, int weaponValue)
    {
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

}
