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
    public int hatValue{
        get => _HatValue;
        set{
            _HatValue = hatValue;
            playerSpriteHat = allHats[hatValue];
        }
    }

    public Sprite[] allBodies;
    private int _BodyValue = 0;
    public int bodyValue{
        get => _BodyValue;
        set{
            _BodyValue = bodyValue;
            playerSpriteBody = allBodies[bodyValue];
        }
    }

    [SerializeField] private Sprite[] allWeapons;
    private int _WeaponValue = 0;
    public int weaponValue{
        get => _WeaponValue;
        set{
            _WeaponValue = weaponValue;
            playerSpriteWeapon = allWeapons[weaponValue];
        }
    }


    [SerializeField]private bool isReady = false;
    public bool readiedUp = false;
    [SerializeField]private bool clickedUP = false;
    [SerializeField]private bool clickedDown = false;
    [SerializeField]private bool clickedLeft = false;
    [SerializeField]private bool clickedRight = false;
    //[SerializeField]private bool switchMenu = false;

    public void OnUP(InputAction.CallbackContext context){
        //clickedUP = context.action.triggered;
        
        if (context.started){
            //Debug.Log("Action was started");
        }
        else if (context.performed){
            //Debug.Log("Action was performed");
            clickedUP = true;
        }
        else if (context.canceled){
            //Debug.Log("Action was cancelled");
        }
            
            
    }
    public void OnDown(InputAction.CallbackContext context){
        if (context.started){
            //Debug.Log("Action was started");
        }
        else if (context.performed){
            //Debug.Log("Action was performed");
            clickedDown = true;
        }
        else if (context.canceled){
            //Debug.Log("Action was cancelled");
        }
    }
    public void OnLeft(InputAction.CallbackContext context){
        if (context.started){
            //Debug.Log("Action was started");
        }
        else if (context.performed){
            //Debug.Log("Action was performed");
            clickedLeft = true;
        }
        else if (context.canceled){
            //Debug.Log("Action was cancelled");
        }
    }   
    public void OnRight(InputAction.CallbackContext context){
        if (context.started){
            //Debug.Log("Action was started");
        }
        else if (context.performed){
            //Debug.Log("Action was performed");
            clickedRight = true;
        }
        else if (context.canceled){
            //Debug.Log("Action was cancelled");
        }
    }
    public void OnReady(InputAction.CallbackContext context){
        isReady = context.action.triggered;
    }
    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas").transform;
        playerSpriteHat = transform.GetChild(0).GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite;
        playerSpriteBody = transform.GetChild(0).GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite;
        playerSpriteWeapon = transform.GetChild(0).GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite;
        this.transform.SetParent (canvas.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if(clickedUP){
            if(menuPos == 0){
                menuPos = MenuArray.Length -1;
                //Debug.Log(menuPos);
            }else{
                menuPos -= 1;
                //Debug.Log(menuPos);
            }
            clickedUP = false;
        }
        if(clickedDown){
            if(menuPos >= MenuArray.Length -1){
                menuPos = 0;
                //Debug.Log(menuPos);
            }else{
                menuPos += 1;
                //Debug.Log(menuPos);
            }
            clickedDown = false;
        }
        //Hats
        if(menuPos == 0){
            hatBox.Select();
            //Debug.Log("Hats");
            if(clickedRight){
                Right1.onClick.Invoke();
                if(_HatValue >= allHats.Length-1){
                    _HatValue = 0;
                }else{
                    _HatValue += 1;
                }
                playerSpriteHat = allHats[hatValue];
                transform.GetChild(0).GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteHat;
                clickedRight = false;
                // Debug.Log("Next Hat");
            }

            if(clickedLeft){
                Left1.onClick.Invoke();
                if(_HatValue == 0){
                    _HatValue = allHats.Length-1;
                }else{
                    _HatValue -= 1;
                }
                playerSpriteHat = allHats[hatValue];
                transform.GetChild(0).GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteHat;
                // Debug.Log("Previous Hat");
                clickedLeft = false;
            }
        }
        //Bodies
        if(menuPos == 1){
            //Debug.Log("Bodies");
            bodyBox.Select();
            
            if(clickedRight){
                Right1.onClick.Invoke();
                if(_BodyValue >= allBodies.Length-1){
                    _BodyValue = 0;
                }else{
                    _BodyValue += 1;
                }
                playerSpriteBody = allBodies[bodyValue];
                transform.GetChild(0).GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteBody;
                // Debug.Log("Next Body");
                clickedRight = false;
            }

            if(clickedLeft){
                Left1.onClick.Invoke();
                if(_BodyValue == 0){
                    _BodyValue = allBodies.Length-1;
                }else{
                    _BodyValue -= 1;
                }
                playerSpriteBody = allBodies[bodyValue];
                transform.GetChild(0).GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteBody;
                // Debug.Log("Previous Body");
                clickedLeft = false;
            }
        }
        //Weapons
        if(menuPos == 2){
            //Debug.Log("Weapons");
            weaponBox.Select();

            if(clickedRight){
                Right1.onClick.Invoke();
                if(_WeaponValue >= allWeapons.Length-1){
                    _WeaponValue = 0;
                }else{
                    _WeaponValue += 1;
                }
                playerSpriteWeapon = allWeapons[weaponValue];
                transform.GetChild(0).GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteWeapon;
                // Debug.Log("Next Weapon");
                clickedRight = false;
            }

            if(clickedLeft){
                Left1.onClick.Invoke();
                if(_WeaponValue == 0){
                    _WeaponValue = allWeapons.Length-1;
                }else{
                    _WeaponValue -= 1;
                }
                playerSpriteWeapon = allWeapons[weaponValue];
                transform.GetChild(0).GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite = playerSpriteWeapon;
                // Debug.Log("Previous Weapon");
                clickedLeft = false;
            }
        }
        
        if(isReady && !readiedUp){
            Ready.GetComponent<Image>().color = Color.green;
            PlayerSaveData.playerNumber +=1;
            readiedUp = true;
            Debug.Log(PlayerSaveData.playerNumber);
        }

        // if(isReady && readiedUp){
        //     Ready.GetComponent<Image>().color = Color.white;
        //     PlayerSaveData.playerNumber -=1;
        //     readiedUp = false;
        //     Debug.Log(PlayerSaveData.playerNumber);
        // }
    }
}
