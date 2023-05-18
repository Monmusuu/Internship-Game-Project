using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class CharacterSelection : MonoBehaviour
{
    PlayerSaveData playerSaveData;
    [SerializeField] private Sprite playerSpriteHat;
    [SerializeField] private Sprite playerSpriteBody;
    [SerializeField] private Sprite playerSpriteWeapon;
    [SerializeField] private Button hatBox;
    [SerializeField] private Button bodyBox;
    [SerializeField] private Button weaponBox;
    [SerializeField] private Button Left1;
    [SerializeField] private Button Left2;
    [SerializeField] private Button Left3;
    [SerializeField] private Button Right1;
    [SerializeField] private Button Right2;
    [SerializeField] private Button Right3;
    [SerializeField] private Button Ready;

    public Transform canvas;

    private int[] MenuArray = new int[3];
    private int menuPos = 0;

    public Sprite[] allHats;
    private int _HatValue = 0;
    public int hatValue
    {
        get => _HatValue;
        set
        {
            _HatValue = value;
            playerSpriteHat = allHats[_HatValue];
        }
    }

    public Sprite[] allBodies;
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

    [SerializeField] private bool isReady = false;
    public bool readiedUp = false;
    [SerializeField] private bool clickedUP = false;
    [SerializeField] private bool clickedDown = false;
    [SerializeField] private bool clickedLeft = false;
    [SerializeField] private bool clickedRight = false;

    public void OnUP(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            clickedUP = true;
        }
    }

    public void OnDown(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            clickedDown = true;
        }
    }

    public void OnLeft(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            clickedLeft = true;
        }
    }

    public void OnRight(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            clickedRight = true;
        }
    }

    public void OnReady(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            isReady = !isReady;

            if (isReady)
            {
                Ready.GetComponent<Image>().color = Color.green;
                PlayerSaveData.playerReadyNumber += 1;
            }
            else
            {
                Ready.GetComponent<Image>().color = Color.white;
                PlayerSaveData.playerReadyNumber -= 1;
            }
        }
    }

    void Start()
    {
        PlayerSaveData.playerNumber += 1;
        canvas = GameObject.Find("Canvas").transform;
        playerSpriteHat = transform.GetChild(0).GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite;
        playerSpriteBody = transform.GetChild(0).GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite;
        playerSpriteWeapon = transform.GetChild(0).GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite;
        this.transform.SetParent(canvas.transform);
    }

    void Update()
    {
        if (clickedUP)
        {
            if (menuPos == 0)
            {
                menuPos = MenuArray.Length - 1;
            }
            else
            {
                menuPos -= 1;
            }
            clickedUP = false;
        }
        if (clickedDown)
        {
            if (menuPos >= MenuArray.Length - 1)
            {
                menuPos = 0;
            }
            else
            {
                menuPos += 1;
            }
            clickedDown = false;
        }

        if (menuPos == 0)
        {
            hatBox.Select();
            if (clickedRight)
            {
                Right1.onClick.Invoke();
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
                clickedRight = false;
            }

            if (clickedLeft)
            {
                Left1.onClick.Invoke();
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
                clickedLeft = false;
            }
        }

        if (menuPos == 1)
        {
            bodyBox.Select();

            if (clickedRight)
            {
                Right1.onClick.Invoke();
                if (_BodyValue >= allBodies.Length - 1)
                {
                    _BodyValue = 0;
                }
                else
                {
                    _BodyValue += 1;
                }
                playerSpriteBody = allBodies[bodyValue];
                transform.GetChild(0).GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteBody;
                clickedRight = false;
            }

            if (clickedLeft)
            {
                Left1.onClick.Invoke();
                if (_BodyValue == 0)
                {
                    _BodyValue = allBodies.Length - 1;
                }
                else
                {
                    _BodyValue -= 1;
                }
                playerSpriteBody = allBodies[bodyValue];
                transform.GetChild(0).GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteBody;
                clickedLeft = false;
            }
        }

        if (menuPos == 2)
        {
            weaponBox.Select();

            if (clickedRight)
            {
                Right1.onClick.Invoke();
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
                clickedRight = false;
            }

            if (clickedLeft)
            {
                Left1.onClick.Invoke();
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
                clickedLeft = false;
            }
        }

        if (PlayerSaveData.playerReadyNumber == PlayerSaveData.playerNumber)
        {
            Debug.Log("Player is readied up. Moving to the next scene...");
            SceneManager.LoadScene("MapSelection"); // Replace "NextSceneName" with the actual name of the next scene
        }
    }
}
