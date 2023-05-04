using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSaveData : MonoBehaviour
{
        public static int playerNumber = 0;

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
                Debug.Log(playerNumber);
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
                        player3.SetActive(false);
                        player4.SetActive(false);
                        player5.SetActive(false);
                        player6.SetActive(false);

                        player3Text.SetActive(false);
                        player4Text.SetActive(false);
                        player5Text.SetActive(false);
                        player6Text.SetActive(false);
                }else if(playerNumber == 3){
                        player4.SetActive(false);
                        player5.SetActive(false);
                        player6.SetActive(false);

                        player4Text.SetActive(false);
                        player5Text.SetActive(false);
                        player6Text.SetActive(false);
                }else if(playerNumber == 4){
                        player5.SetActive(false);
                        player6.SetActive(false);

                        player5Text.SetActive(false);
                        player6Text.SetActive(false);
                }else if(playerNumber == 5){
                        player6.SetActive(false);
                        player6Text.SetActive(false);
                }
        }
}
