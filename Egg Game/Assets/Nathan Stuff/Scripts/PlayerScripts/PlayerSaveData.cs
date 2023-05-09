using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerSaveData : MonoBehaviour
{
        private Scene scene;
        public CharacterSelection characterSelection1;
        public CharacterSelection characterSelection2;
        public CharacterSelection characterSelection3;
        public CharacterSelection characterSelection4;
        public CharacterSelection characterSelection5;
        public CharacterSelection characterSelection6;
        public static int playerNumber = 0;
        public Sprite[] allHats;
        public Sprite[] allBodies;
        public Sprite[] allWeapons;

        public static int player1HatSpriteNumber;
        public static int player2HatSpriteNumber;
        public static int player3HatSpriteNumber;
        public static int player4HatSpriteNumber;
        public static int player5HatSpriteNumber;
        public static int player6HatSpriteNumber;

        public Sprite player1SpriteHat;
        public Sprite player2SpriteHat;
        public Sprite player3SpriteHat;
        public Sprite player4SpriteHat;
        public Sprite player5SpriteHat;
        public Sprite player6SpriteHat;

        public static int player1BodySpriteNumber;
        public static int player2BodySpriteNumber;
        public static int player3BodySpriteNumber;
        public static int player4BodySpriteNumber;
        public static int player5BodySpriteNumber;
        public static int player6BodySpriteNumber;

        public Sprite player1SpriteBody;
        public Sprite player2SpriteBody;
        public Sprite player3SpriteBody;
        public Sprite player4SpriteBody;
        public Sprite player5SpriteBody;
        public Sprite player6SpriteBody;

        public static int player1WeaponSpriteNumber;
        public static int player2WeaponSpriteNumber;
        public static int player3WeaponSpriteNumber;
        public static int player4WeaponSpriteNumber;
        public static int player5WeaponSpriteNumber;
        public static int player6WeaponSpriteNumber;

        public Sprite player1SpriteWeapon;
        public Sprite player2SpriteWeapon;
        public Sprite player3SpriteWeapon;
        public Sprite player4SpriteWeapon;
        public Sprite player5SpriteWeapon;
        public Sprite player6SpriteWeapon;

        [SerializeField] private GameObject player1;
        [SerializeField] private GameObject player2;
        [SerializeField] private GameObject player3;
        [SerializeField] private GameObject player4;
        [SerializeField] private GameObject player5;
        [SerializeField] private GameObject player6;

        [SerializeField] private GameObject player1Text;
        [SerializeField] private GameObject player2Text;
        [SerializeField] private GameObject player3Text;
        [SerializeField] private GameObject player4Text;
        [SerializeField] private GameObject player5Text;
        [SerializeField] private GameObject player6Text;

        private void Start() {
                Debug.Log("Players "+playerNumber);
                scene = SceneManager.GetActiveScene();

                if (scene.name != "CharacterSelection"){
                        player1 = GameObject.Find("Player");
                        player2 = GameObject.Find("Player 2");
                        player3 = GameObject.Find("Player 3");
                        player4 = GameObject.Find("Player 4");
                        player5 = GameObject.Find("Player 5");
                        player6 = GameObject.Find("Player 6");

                        player1Text = GameObject.Find("Player One Text");
                        player2Text = GameObject.Find("Player Two Text");
                        player3Text = GameObject.Find("Player Three Text");
                        player4Text = GameObject.Find("Player Four Text");
                        player5Text = GameObject.Find("Player Five Text");
                        player6Text = GameObject.Find("Player Six Text");
                        
                        if(playerNumber == 1){
                                player1SpriteHat = allHats[player1HatSpriteNumber];
                                player1.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteHat;

                                player1SpriteWeapon = allWeapons[player1WeaponSpriteNumber];
                                player1.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteWeapon;

                                player1SpriteBody = allBodies[player1BodySpriteNumber];
                                player1.transform.gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteBody;

                                player2.SetActive(false);
                                player3.SetActive(false);
                                player4.SetActive(false);
                                player5.SetActive(false);
                                player6.SetActive(false);

                                player2Text.SetActive(false);
                                player3Text.SetActive(false);
                                player4Text.SetActive(false);
                                player5Text.SetActive(false);
                                player6Text.SetActive(false);
                        }else if(playerNumber == 2){
                                player1SpriteHat = allHats[player1HatSpriteNumber];
                                player1.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteHat;

                                player1SpriteWeapon = allWeapons[player1WeaponSpriteNumber];
                                player1.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteWeapon;

                                player1SpriteBody = allBodies[player1BodySpriteNumber];
                                player1.transform.gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteBody;
                                
                                player2SpriteHat = allHats[player2HatSpriteNumber];
                                player2.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteHat;

                                player2SpriteWeapon = allWeapons[player2WeaponSpriteNumber];
                                player2.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteWeapon;

                                player2SpriteBody = allBodies[player2BodySpriteNumber];
                                player2.transform.gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteBody;

                                player3.SetActive(false);
                                player4.SetActive(false);
                                player5.SetActive(false);
                                player6.SetActive(false);

                                player3Text.SetActive(false);
                                player4Text.SetActive(false);
                                player5Text.SetActive(false);
                                player6Text.SetActive(false);
                        }else if(playerNumber == 3){

                                player1SpriteHat = allHats[player1HatSpriteNumber];
                                player1.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteHat;

                                player1SpriteWeapon = allWeapons[player1WeaponSpriteNumber];
                                player1.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteWeapon;

                                player1SpriteBody = allBodies[player1BodySpriteNumber];
                                player1.transform.gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteBody;
                                
                                player2SpriteHat = allHats[player2HatSpriteNumber];
                                player2.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteHat;

                                player2SpriteWeapon = allWeapons[player2WeaponSpriteNumber];
                                player2.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteWeapon;

                                player2SpriteBody = allBodies[player2BodySpriteNumber];
                                player2.transform.gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteBody;

                                player3SpriteHat = allHats[player3HatSpriteNumber];
                                player3.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player3SpriteHat;

                                player3SpriteWeapon = allWeapons[player3WeaponSpriteNumber];
                                player3.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = player3SpriteWeapon;

                                player3SpriteBody = allBodies[player3BodySpriteNumber];
                                player3.transform.gameObject.GetComponent<SpriteRenderer>().sprite = player3SpriteBody;

                                player4.SetActive(false);
                                player5.SetActive(false);
                                player6.SetActive(false);

                                player4Text.SetActive(false);
                                player5Text.SetActive(false);
                                player6Text.SetActive(false);
                        }else if(playerNumber == 4){
                                player1SpriteHat = allHats[player1HatSpriteNumber];
                                player1.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteHat;

                                player1SpriteWeapon = allWeapons[player1WeaponSpriteNumber];
                                player1.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteWeapon;

                                player1SpriteBody = allBodies[player1BodySpriteNumber];
                                player1.transform.gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteBody;
                                
                                player2SpriteHat = allHats[player2HatSpriteNumber];
                                player2.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteHat;

                                player2SpriteWeapon = allWeapons[player2WeaponSpriteNumber];
                                player2.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteWeapon;

                                player2SpriteBody = allBodies[player2BodySpriteNumber];
                                player2.transform.gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteBody;

                                player3SpriteHat = allHats[player3HatSpriteNumber];
                                player3.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player3SpriteHat;

                                player3SpriteWeapon = allWeapons[player3WeaponSpriteNumber];
                                player3.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = player3SpriteWeapon;

                                player3SpriteBody = allBodies[player3BodySpriteNumber];
                                player3.transform.gameObject.GetComponent<SpriteRenderer>().sprite = player3SpriteBody;

                                player4SpriteHat = allHats[player4HatSpriteNumber];
                                player4.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player4SpriteHat;

                                player4SpriteWeapon = allWeapons[player4WeaponSpriteNumber];
                                player4.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = player4SpriteWeapon;

                                player4SpriteBody = allBodies[player4BodySpriteNumber];
                                player4.transform.gameObject.GetComponent<SpriteRenderer>().sprite = player4SpriteBody;

                                player5.SetActive(false);
                                player6.SetActive(false);

                                player5Text.SetActive(false);
                                player6Text.SetActive(false);
                        }else if(playerNumber == 5){
                                player1SpriteHat = allHats[player1HatSpriteNumber];
                                player1.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteHat;

                                player1SpriteWeapon = allWeapons[player1WeaponSpriteNumber];
                                player1.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteWeapon;

                                player1SpriteBody = allBodies[player1BodySpriteNumber];
                                player1.transform.gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteBody;
                                
                                player2SpriteHat = allHats[player2HatSpriteNumber];
                                player2.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteHat;

                                player2SpriteWeapon = allWeapons[player2WeaponSpriteNumber];
                                player2.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteWeapon;

                                player2SpriteBody = allBodies[player2BodySpriteNumber];
                                player2.transform.gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteBody;

                                player3SpriteHat = allHats[player3HatSpriteNumber];
                                player3.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player3SpriteHat;

                                player3SpriteWeapon = allWeapons[player3WeaponSpriteNumber];
                                player3.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = player3SpriteWeapon;

                                player3SpriteBody = allBodies[player3BodySpriteNumber];
                                player3.transform.gameObject.GetComponent<SpriteRenderer>().sprite = player3SpriteBody;

                                player4SpriteHat = allHats[player4HatSpriteNumber];
                                player4.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player4SpriteHat;

                                player4SpriteWeapon = allWeapons[player4WeaponSpriteNumber];
                                player4.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = player4SpriteWeapon;

                                player4SpriteBody = allBodies[player4BodySpriteNumber];
                                player4.transform.gameObject.GetComponent<SpriteRenderer>().sprite = player4SpriteBody;

                                player5SpriteHat = allHats[player5HatSpriteNumber];
                                player5.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player5SpriteHat;

                                player5SpriteWeapon = allWeapons[player5WeaponSpriteNumber];
                                player5.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = player5SpriteWeapon;

                                player5SpriteBody = allBodies[player5BodySpriteNumber];
                                player5.transform.gameObject.GetComponent<SpriteRenderer>().sprite = player5SpriteBody;

                                player6.SetActive(false);
                                player6Text.SetActive(false);
                        }else if(playerNumber == 6){
                                player1SpriteHat = allHats[player1HatSpriteNumber];
                                player1.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteHat;

                                player1SpriteWeapon = allWeapons[player1WeaponSpriteNumber];
                                player1.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteWeapon;

                                player1SpriteBody = allBodies[player1BodySpriteNumber];
                                player1.transform.gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteBody;
                                
                                player2SpriteHat = allHats[player2HatSpriteNumber];
                                player2.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteHat;

                                player2SpriteWeapon = allWeapons[player2WeaponSpriteNumber];
                                player2.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteWeapon;

                                player2SpriteBody = allBodies[player2BodySpriteNumber];
                                player2.transform.gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteBody;

                                player3SpriteHat = allHats[player3HatSpriteNumber];
                                player3.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player3SpriteHat;

                                player3SpriteWeapon = allWeapons[player3WeaponSpriteNumber];
                                player3.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = player3SpriteWeapon;

                                player3SpriteBody = allBodies[player3BodySpriteNumber];
                                player3.transform.gameObject.GetComponent<SpriteRenderer>().sprite = player3SpriteBody;

                                player4SpriteHat = allHats[player4HatSpriteNumber];
                                player4.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player4SpriteHat;

                                player4SpriteWeapon = allWeapons[player4WeaponSpriteNumber];
                                player4.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = player4SpriteWeapon;

                                player4SpriteBody = allBodies[player4BodySpriteNumber];
                                player4.transform.gameObject.GetComponent<SpriteRenderer>().sprite = player4SpriteBody;

                                player5SpriteHat = allHats[player5HatSpriteNumber];
                                player5.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player5SpriteHat;

                                player5SpriteWeapon = allWeapons[player5WeaponSpriteNumber];
                                player5.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = player5SpriteWeapon;

                                player5SpriteBody = allBodies[player5BodySpriteNumber];
                                player5.transform.gameObject.GetComponent<SpriteRenderer>().sprite = player5SpriteBody;

                                player6SpriteHat = allHats[player6HatSpriteNumber];
                                player6.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player6SpriteHat;

                                player6SpriteWeapon = allWeapons[player6WeaponSpriteNumber];
                                player6.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = player6SpriteWeapon;

                                player6SpriteBody = allBodies[player6BodySpriteNumber];
                                player6.transform.gameObject.GetComponent<SpriteRenderer>().sprite = player6SpriteBody;
                        }
                }
                Debug.Log("Hat " + player1HatSpriteNumber);
        }

        private void Update() {
                if (scene.name == "CharacterSelection"){
                        if(GameObject.FindGameObjectWithTag("Player1")){
                                characterSelection1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<CharacterSelection>();
                                if(characterSelection1.readiedUp){
                                        player1HatSpriteNumber = characterSelection1.hatValue;
                                        player1BodySpriteNumber = characterSelection1.bodyValue;
                                        player1WeaponSpriteNumber = characterSelection1.weaponValue;
                                        //Debug.Log("Hat " + player1HatSpriteNumber);
                                }
                        }
                        if(GameObject.FindGameObjectWithTag("Player2")){
                                characterSelection2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<CharacterSelection>();
                                if(characterSelection2.readiedUp){
                                        player2HatSpriteNumber = characterSelection2.hatValue;
                                        player2BodySpriteNumber = characterSelection2.bodyValue;
                                        player2WeaponSpriteNumber = characterSelection2.weaponValue;
                                }
                        }
                        if(GameObject.FindGameObjectWithTag("Player3")){
                                characterSelection3 = GameObject.FindGameObjectWithTag("Player3").GetComponent<CharacterSelection>();
                                if(characterSelection3.readiedUp){
                                        player3HatSpriteNumber = characterSelection3.hatValue;
                                        player3BodySpriteNumber = characterSelection3.bodyValue;
                                        player3WeaponSpriteNumber = characterSelection3.weaponValue;
                                }
                        }
                        if(GameObject.FindGameObjectWithTag("Player4")){
                                characterSelection4 = GameObject.FindGameObjectWithTag("Player4").GetComponent<CharacterSelection>();
                                if(characterSelection4.readiedUp){ 
                                        player4HatSpriteNumber = characterSelection4.hatValue;
                                        player4BodySpriteNumber = characterSelection4.bodyValue;
                                        player4WeaponSpriteNumber = characterSelection4.weaponValue;
                                }
                        }
                        if(GameObject.FindGameObjectWithTag("Player5")){
                                characterSelection5 = GameObject.FindGameObjectWithTag("Player5").GetComponent<CharacterSelection>();
                                if(characterSelection5.readiedUp){
                                        player5HatSpriteNumber = characterSelection5.hatValue;
                                        player5BodySpriteNumber = characterSelection5.bodyValue;
                                        player5WeaponSpriteNumber = characterSelection5.weaponValue;
                                }
                        }
                        if(GameObject.FindGameObjectWithTag("Player6")){
                                characterSelection6 = GameObject.FindGameObjectWithTag("Player6").GetComponent<CharacterSelection>();
                                if(characterSelection6.readiedUp){
                                        player6HatSpriteNumber = characterSelection6.hatValue;
                                        player6BodySpriteNumber = characterSelection6.bodyValue;
                                        player6WeaponSpriteNumber = characterSelection6.weaponValue;
                                }
                        }
                }
        }
}
