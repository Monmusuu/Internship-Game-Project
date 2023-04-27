using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiTargetCamera : MonoBehaviour
{

    public List<GameObject> players = new List<GameObject>();
    public Vector3 offset;
    public float smoothTime = 0.5f;
    private Vector3 velocity;
    public float minZoom = 40f;
    public float maxZoom = 10f;
    public float zoomLimiter = 50f;
    private Camera cam;

    public bool hasPlayer1;
    bool hasPlayer2;
    bool hasPlayer3;
    bool hasPlayer4;
    bool hasPlayer5;
    bool hasPlayer6;

    void Start() {
        cam = GetComponent<Camera>();
    }

    private void Update() {
        if(GameObject.FindWithTag("Player1") && (!hasPlayer1)){
            players.AddRange(GameObject.FindGameObjectsWithTag("Player1"));
            hasPlayer1 = true;
        }

        if(GameObject.FindWithTag("Player2") && (!hasPlayer2)){
            players.AddRange(GameObject.FindGameObjectsWithTag("Player2"));
            hasPlayer2 = true;
        }

        if(GameObject.FindWithTag("Player3") && (!hasPlayer3)){
            players.AddRange(GameObject.FindGameObjectsWithTag("Player3"));
            hasPlayer3 = true;
        }

        if(GameObject.FindWithTag("Player4") && (!hasPlayer4)){
            players.AddRange(GameObject.FindGameObjectsWithTag("Player4"));
            hasPlayer4 = true;
        }

        if(GameObject.FindWithTag("Player5") && (!hasPlayer5)){
            players.AddRange(GameObject.FindGameObjectsWithTag("Player5"));
            hasPlayer5 = true;
        }

        if(GameObject.FindWithTag("Player6") && (!hasPlayer6)){
            players.AddRange(GameObject.FindGameObjectsWithTag("Player6"));
            hasPlayer6 = true;
        }
    }

    private void LateUpdate() {
        if(players.Count == 0)
            return;

       Move();
       Zoom();
    }


    void Zoom(){
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
        //Debug.Log(GetGreatestDistance());
    }

    void Move(){
        Vector3 centerPoint = GetCenterPoint();

        Vector3 newPosition = centerPoint + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }


    float GetGreatestDistance(){
        var bounds = new Bounds(players[0].transform.position, Vector3.zero);

        for(int i = 0; i < players.Count; i++){
            bounds.Encapsulate(players[i].transform.position);
        }

        return bounds.size.x;
    }

    Vector3 GetCenterPoint(){
        if(players.Count == 1){
            return players[0].transform.position;
        }

        var bounds = new Bounds(players[0].transform.position, Vector3.zero);

        for(int i = 0; i < players.Count; i++){
            bounds.Encapsulate(players[i].transform.position);
        }

        return bounds.center;
    }
}
