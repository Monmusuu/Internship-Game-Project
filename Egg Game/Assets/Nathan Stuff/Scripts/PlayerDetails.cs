using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetails : MonoBehaviour
{
    public int playerID;

    public Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = startPos;
    }

    private void Update() {
        if(playerID == 1){
            gameObject.tag = "Player1";
        }
        if(playerID == 2){
            gameObject.tag = "Player2";
        }
        if(playerID == 3){
            gameObject.tag = "Player3";
        }
        if(playerID == 4){
            gameObject.tag = "Player4";
        }
        if(playerID == 5){
            gameObject.tag = "Player5";
        }
        if(playerID == 6){
            gameObject.tag = "Player6";
        }
    }
}
