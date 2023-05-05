using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerSaveData : MonoBehaviour
{
        private Scene scene;
        public CharacterSelection[] characterSelection;
        public static int playerNumber = 0;
        public Sprite[] allHats;
        public Sprite[] allBodies;
        public Sprite[] allWeapons;

        public static int player1HatSpriteNumber = 0;
        public static int player2HatSpriteNumber = 0;
        public static int player3HatSpriteNumber = 0;
        public static int player4HatSpriteNumber = 0;
        public static int player5HatSpriteNumber = 0;
        public static int player6HatSpriteNumber = 0;

        public Sprite player1SpriteHat;
        public Sprite player2SpriteHat;
        public Sprite player3SpriteHat;
        public Sprite player4SpriteHat;
        public Sprite player5SpriteHat;
        public Sprite player6SpriteHat;

        public static int player1BodySpriteNumber = 0;
        public static int player2BodySpriteNumber = 0;
        public static int player3BodySpriteNumber = 0;
        public static int player4BodySpriteNumber = 0;
        public static int player5BodySpriteNumber = 0;
        public static int player6BodySpriteNumber = 0;

        public Sprite player1SpriteBody;
        public Sprite player2SpriteBody;
        public Sprite player3SpriteBody;
        public Sprite player4SpriteBody;
        public Sprite player5SpriteBody;
        public Sprite player6SpriteBody;

        public static int player1WeaponSpriteNumber = 0;
        public static int player2WeaponSpriteNumber = 0;
        public static int player3WeaponSpriteNumber = 0;
        public static int player4WeaponSpriteNumber = 0;
        public static int player5WeaponSpriteNumber = 0;
        public static int player6WeaponSpriteNumber = 0;

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
                                player1.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteWeapon;

                                player1SpriteBody = allBodies[player1BodySpriteNumber];
                                player1.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteBody;

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
                                player1.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteWeapon;

                                player1SpriteBody = allBodies[player1BodySpriteNumber];
                                player1.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteBody;
                                
                                player2SpriteHat = allHats[player2HatSpriteNumber];
                                player2.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteHat;

                                player2SpriteWeapon = allWeapons[player2WeaponSpriteNumber];
                                player2.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteWeapon;

                                player2SpriteBody = allBodies[player2BodySpriteNumber];
                                player2.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteBody;

                                

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
                                player1.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteWeapon;

                                player1SpriteBody = allBodies[player1BodySpriteNumber];
                                player1.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteBody;
                                
                                player2SpriteHat = allHats[player2HatSpriteNumber];
                                player2.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteHat;

                                player2SpriteWeapon = allWeapons[player2WeaponSpriteNumber];
                                player2.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteWeapon;

                                player2SpriteBody = allBodies[player2BodySpriteNumber];
                                player2.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteBody;

                                player3SpriteHat = allHats[player3HatSpriteNumber];
                                player3.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player3SpriteHat;

                                player3SpriteWeapon = allWeapons[player3WeaponSpriteNumber];
                                player3.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player3SpriteWeapon;

                                player3SpriteBody = allBodies[player3BodySpriteNumber];
                                player3.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player3SpriteBody;

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
                                player1.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteWeapon;

                                player1SpriteBody = allBodies[player1BodySpriteNumber];
                                player1.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteBody;
                                
                                player2SpriteHat = allHats[player2HatSpriteNumber];
                                player2.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteHat;

                                player2SpriteWeapon = allWeapons[player2WeaponSpriteNumber];
                                player2.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteWeapon;

                                player2SpriteBody = allBodies[player2BodySpriteNumber];
                                player2.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteBody;

                                player3SpriteHat = allHats[player3HatSpriteNumber];
                                player3.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player3SpriteHat;

                                player3SpriteWeapon = allWeapons[player3WeaponSpriteNumber];
                                player3.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player3SpriteWeapon;

                                player3SpriteBody = allBodies[player3BodySpriteNumber];
                                player3.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player3SpriteBody;

                                player4SpriteHat = allHats[player4HatSpriteNumber];
                                player4.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player4SpriteHat;

                                player4SpriteWeapon = allWeapons[player4WeaponSpriteNumber];
                                player4.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player4SpriteWeapon;

                                player4SpriteBody = allBodies[player4BodySpriteNumber];
                                player4.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player4SpriteBody;

                                player5.SetActive(false);
                                player6.SetActive(false);

                                player5Text.SetActive(false);
                                player6Text.SetActive(false);
                        }else if(playerNumber == 5){
                                player1SpriteHat = allHats[player1HatSpriteNumber];
                                player1.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteHat;

                                player1SpriteWeapon = allWeapons[player1WeaponSpriteNumber];
                                player1.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteWeapon;

                                player1SpriteBody = allBodies[player1BodySpriteNumber];
                                player1.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteBody;
                                
                                player2SpriteHat = allHats[player2HatSpriteNumber];
                                player2.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteHat;

                                player2SpriteWeapon = allWeapons[player2WeaponSpriteNumber];
                                player2.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteWeapon;

                                player2SpriteBody = allBodies[player2BodySpriteNumber];
                                player2.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteBody;

                                player3SpriteHat = allHats[player3HatSpriteNumber];
                                player3.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player3SpriteHat;

                                player3SpriteWeapon = allWeapons[player3WeaponSpriteNumber];
                                player3.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player3SpriteWeapon;

                                player3SpriteBody = allBodies[player3BodySpriteNumber];
                                player3.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player3SpriteBody;

                                player4SpriteHat = allHats[player4HatSpriteNumber];
                                player4.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player4SpriteHat;

                                player4SpriteWeapon = allWeapons[player4WeaponSpriteNumber];
                                player4.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player4SpriteWeapon;

                                player4SpriteBody = allBodies[player4BodySpriteNumber];
                                player4.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player4SpriteBody;

                                player5SpriteHat = allHats[player5HatSpriteNumber];
                                player5.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player5SpriteHat;

                                player5SpriteWeapon = allWeapons[player5WeaponSpriteNumber];
                                player5.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player5SpriteWeapon;

                                player5SpriteBody = allBodies[player5BodySpriteNumber];
                                player5.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player5SpriteBody;

                                player6.SetActive(false);
                                player6Text.SetActive(false);
                        }else if(playerNumber == 6){
                                player1SpriteHat = allHats[player1HatSpriteNumber];
                                player1.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteHat;

                                player1SpriteWeapon = allWeapons[player1WeaponSpriteNumber];
                                player1.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteWeapon;

                                player1SpriteBody = allBodies[player1BodySpriteNumber];
                                player1.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player1SpriteBody;
                                
                                player2SpriteHat = allHats[player2HatSpriteNumber];
                                player2.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteHat;

                                player2SpriteWeapon = allWeapons[player2WeaponSpriteNumber];
                                player2.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteWeapon;

                                player2SpriteBody = allBodies[player2BodySpriteNumber];
                                player2.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player2SpriteBody;

                                player3SpriteHat = allHats[player3HatSpriteNumber];
                                player3.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player3SpriteHat;

                                player3SpriteWeapon = allWeapons[player3WeaponSpriteNumber];
                                player3.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player3SpriteWeapon;

                                player3SpriteBody = allBodies[player3BodySpriteNumber];
                                player3.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player3SpriteBody;

                                player4SpriteHat = allHats[player4HatSpriteNumber];
                                player4.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player4SpriteHat;

                                player4SpriteWeapon = allWeapons[player4WeaponSpriteNumber];
                                player4.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player4SpriteWeapon;

                                player4SpriteBody = allBodies[player4BodySpriteNumber];
                                player4.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player4SpriteBody;

                                player5SpriteHat = allHats[player5HatSpriteNumber];
                                player5.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player5SpriteHat;

                                player5SpriteWeapon = allWeapons[player5WeaponSpriteNumber];
                                player5.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player5SpriteWeapon;

                                player5SpriteBody = allBodies[player5BodySpriteNumber];
                                player5.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player5SpriteBody;

                                player6SpriteHat = allHats[player6HatSpriteNumber];
                                player6.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player6SpriteHat;

                                player6SpriteWeapon = allWeapons[player6WeaponSpriteNumber];
                                player6.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player6SpriteWeapon;

                                player6SpriteBody = allBodies[player6BodySpriteNumber];
                                player6.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = player6SpriteBody;
                        }
                }
                Debug.Log("Hat " + player1HatSpriteNumber);
        }

        private void Update() {
                if (scene.name == "CharacterSelection"){
                        player1HatSpriteNumber = characterSelection[0].hatValue;
                        player2HatSpriteNumber = characterSelection[1].hatValue;
                        player3HatSpriteNumber = characterSelection[2].hatValue;
                        player4HatSpriteNumber = characterSelection[3].hatValue;
                        player5HatSpriteNumber = characterSelection[4].hatValue;
                        player6HatSpriteNumber = characterSelection[5].hatValue;

                        player1BodySpriteNumber = characterSelection[0].bodyValue;
                        player2BodySpriteNumber = characterSelection[1].bodyValue;
                        player3BodySpriteNumber = characterSelection[2].bodyValue;
                        player4BodySpriteNumber = characterSelection[3].bodyValue;
                        player5BodySpriteNumber = characterSelection[4].bodyValue;
                        player6BodySpriteNumber = characterSelection[5].bodyValue;

                        player1WeaponSpriteNumber = characterSelection[0].weaponValue;
                        player2WeaponSpriteNumber = characterSelection[1].weaponValue;
                        player3WeaponSpriteNumber = characterSelection[2].weaponValue;
                        player4WeaponSpriteNumber = characterSelection[3].weaponValue;
                        player5WeaponSpriteNumber = characterSelection[4].weaponValue;
                        player6WeaponSpriteNumber = characterSelection[5].weaponValue;
                }
        }
}
