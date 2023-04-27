using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIMGUI : MonoBehaviour
{
    private PlayerDetails playerDetails;
    
    private GameObject playerOne;
    private Sprite playerOneSprite;

    private GameObject playerTwo;
    private Sprite playerTwoSprite;

    private GameObject playerThree;
    private Sprite playerThreeSprite;

    private GameObject playerFour;
    private Sprite playerFourSprite;

    private GameObject playerFive;
    private Sprite playerFiveSprite;

    private GameObject playerSix;
    private Sprite playerSixSprite;


    // Start is called before the first frame update
    void Start()
    {
        if(GameObject.FindWithTag("Player1")){
            playerOne = GameObject.FindWithTag("Player1");
            playerOneSprite = playerOne.GetComponent<SpriteRenderer>().sprite;   
            transform.GetChild(1).GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = playerOneSprite;
        }

        if(GameObject.FindWithTag("Player2")){
            playerTwo = GameObject.FindWithTag("Player2");
            playerTwoSprite = playerTwo.GetComponent<SpriteRenderer>().sprite;   
            transform.GetChild(1).GetChild(1).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = playerTwoSprite;
        }

        if(GameObject.FindWithTag("Player3")){
            playerThree = GameObject.FindWithTag("Player3");
            playerThreeSprite = playerThree.GetComponent<SpriteRenderer>().sprite;   
            transform.GetChild(1).GetChild(2).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = playerOneSprite;
        }
        
        if(GameObject.FindWithTag("Player4")){
            playerFour = GameObject.FindWithTag("Player4");
            playerFourSprite = playerFour.GetComponent<SpriteRenderer>().sprite;   
            transform.GetChild(1).GetChild(3).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = playerOneSprite;
        }

        if(GameObject.FindWithTag("Player5")){
            playerFive = GameObject.FindWithTag("Player5");
            playerFiveSprite = playerFive.GetComponent<SpriteRenderer>().sprite;   
            transform.GetChild(1).GetChild(4).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = playerFiveSprite;
        }

        if(GameObject.FindWithTag("Player6")){
            playerSix = GameObject.FindWithTag("Player6");
            playerSixSprite = playerSix.GetComponent<SpriteRenderer>().sprite;   
            transform.GetChild(1).GetChild(5).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = playerSixSprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
