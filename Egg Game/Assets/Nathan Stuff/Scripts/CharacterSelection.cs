using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class CharacterSelection : MonoBehaviour
{
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


    private int[] MenuArray = new int[4];
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

    [SerializeField] private Sprite[] allBodies;
    //private int bodyPos = 0;
    [SerializeField] private Sprite[] allWeapons;
    //private int weaponPos = 0;


    [SerializeField]private bool isReady = false;
    [SerializeField]private bool clickedUP = false;
    [SerializeField]private bool clickedDown = false;
    [SerializeField]private bool clickedLeft = false;
    [SerializeField]private bool clickedRight = false;

    public void OnUP(InputAction.CallbackContext context){
        clickedUP = context.action.triggered;
    }
    public void OnDown(InputAction.CallbackContext context){
        clickedDown = context.action.triggered;
    }
    public void OnLeft(InputAction.CallbackContext context){
        clickedLeft = context.action.triggered;
    }   
    public void OnRight(InputAction.CallbackContext context){
        clickedRight = context.action.triggered;
    }
    public void OnReady(InputAction.CallbackContext context){
        isReady = context.action.triggered;
    }
    // Start is called before the first frame update
    void Start()
    {
        playerSpriteHat = transform.GetChild(0).GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite;
        playerSpriteBody = transform.GetChild(0).GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite;
        playerSpriteWeapon = transform.GetChild(0).GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite;
    }

    // Update is called once per frame
    void Update()
    {
        if(clickedUP){
            if(menuPos >= MenuArray.Length -1){
                menuPos = 0;
            }else{
                menuPos += 1;
            }
        }
        if(clickedDown){
            if(menuPos == 0){
                menuPos = MenuArray.Length -1;
            }else{
                menuPos -= 1;
            }
        }
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
            }
        }
        if(menuPos == 1){
            //Debug.Log("Bodies");
            bodyBox.Select();
        }
        if(menuPos == 2){
            //Debug.Log("Weapons");
            weaponBox.Select();
        }
        if(isReady){
            Ready.GetComponent<Image>().color = Color.green;
        }
    }
}
