using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundControl : MonoBehaviour
{
    public Player[] player;
    public float RoundTime = 10f;
    public int Round = 0;
    public bool timerOn = false;
    public bool Respawn = false;
    public bool itemsPlaced = false;
    public bool placingItems = false;
    public bool playerRemovingItem = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(!placingItems){
            if(player[0].becameKing){
                Respawn = true;
                Round += 1;
                RoundTime = 10f;
                itemsPlaced = false;
                timerOn = false;
            }
            if(player[1].becameKing){
                Respawn = true;
                Round += 1;
                RoundTime = 10f;
                itemsPlaced = false;
                timerOn = false;
            }
            if(player[2].becameKing){
                Respawn = true;
                Round += 1;
                RoundTime = 10f;
                itemsPlaced = false;
                timerOn = false;
            }
            if(player[3].becameKing){
                Respawn = true;
                Round += 1;
                RoundTime = 10f;
                itemsPlaced = false;
                timerOn = false;
            }
            if(player[4].becameKing){
                Respawn = true;
                Round += 1;
                RoundTime = 10f;
                itemsPlaced = false;
                timerOn = false;
            }
            if(player[5].becameKing){
                Respawn = true;
                Round += 1;
                RoundTime = 10f;
                itemsPlaced = false;
                timerOn = false;
            }
            

            if(itemsPlaced && Round >= 1){
                if(!playerRemovingItem){
                    if(player[0].isKing && itemsPlaced && Round >= 1){
                        timerOn = true;
                    }
                    if(player[1].isKing && itemsPlaced && Round >= 1){
                        timerOn = true;
                    }
                    if(player[2].isKing && itemsPlaced && Round >= 1){
                        timerOn = true;
                    }
                    if(player[3].isKing && itemsPlaced && Round >= 1){
                        timerOn = true;
                    }
                    if(player[4].isKing && itemsPlaced && Round >= 1){
                        timerOn = true;
                    }
                    if(player[5].isKing && itemsPlaced && Round >= 1){
                        timerOn = true;
                    }

                    if(timerOn){
                        RoundTime -= Time.deltaTime;
                    }

                    if(RoundTime <= 0){
                        Respawn = true;
                        Debug.Log("Round Over");
                        itemsPlaced = false;
                        timerOn = false;
                        RoundTime = 10f;
                    }
                    if(RoundTime <= 10f && itemsPlaced){
                        Respawn = false;
                    }
                }
            }
        }
    }
}
