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

    public bool isReady = false;

    void Start()
    {
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
        }
        else
        {
            _BodyValue -= 1;
        }
        playerSpriteBody = allBodies[bodyValue];
        transform.GetChild(0).GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteBody;
    }

    public void NextBody(){
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
}
