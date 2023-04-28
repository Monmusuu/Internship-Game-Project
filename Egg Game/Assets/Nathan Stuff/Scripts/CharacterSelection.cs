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
    [SerializeField] private Button Left1;
    [SerializeField] private Button Left2;
    [SerializeField] private Button Left3;
    [SerializeField] private Button Right1;
    [SerializeField] private Button Right2;
    [SerializeField] private Button Right3;
    [SerializeField] private Button Ready;
    private int[] MenuArray = new int[4];
    private int menuPos = 0;
    [SerializeField] private Sprite[] allHats;
    private int hatPos = 0;
    [SerializeField] private Sprite[] allBodies;
    //private int bodyPos = 0;
    [SerializeField] private Sprite[] allWeapons;
    //private int weaponPos = 0;
    private bool isReady = false;
    private bool clickedUP = false;
    private bool clickedDown = false;
    private bool clickedLeft = false;
    private bool clickedRight = false;

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
        playerSpriteHat = transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>().sprite;
        playerSpriteBody = transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>().sprite;
        playerSpriteWeapon = transform.GetChild(6).gameObject.GetComponent<SpriteRenderer>().sprite;
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
            if(menuPos <= MenuArray.Length +1){
                menuPos = 0;
            }else{
                menuPos -= 1;
            }
        }
        if(menuPos == 0){
            Left1.Select();
            Right1.Select();
            //Debug.Log("Hats");
            if(clickedLeft){
                if(hatPos >= allHats.Length -1){
                    hatPos = 0;
                }else{
                    hatPos += 1;
                }
                Left1.onClick.Invoke();
                //yield WaitForSeconds(1);
                Debug.Log("Previous Hat");
            }
            if(clickedRight){
                Right1.onClick.Invoke();
                Debug.Log("Next Hat");
            }
        }
        if(menuPos == 1){
            //Debug.Log("Bodies");
            Left2.Select();
            Right2.Select();
        }
        if(menuPos == 2){
            //Debug.Log("Weapons");
            Left3.Select();
            Right3.Select();
        }
        if(isReady){
            Ready.GetComponent<Image>().color = Color.green;
        }
    }
}
