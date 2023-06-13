using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultiTargetCamera : MonoBehaviour
{

    public List<GameObject> players = new List<GameObject>();
    public GameObject mapObject;
    public RoundControl roundControl;
    public Vector3 offset;
    public float smoothTime = 0.5f;
    private Vector3 velocity;
    public float minZoom = 25f;
    public float maxZoom = 9f;
    public float zoomLimiter = 35f;
    public float height = 0;
    public float width = 0;
    private Camera cam;
    private float initialZ; // Store the initial Z position of the camera
    public bool hasPlayer1;
    public bool hasPlayer2;
    bool hasPlayer3;
    bool hasPlayer4;
    bool hasPlayer5;
    bool hasPlayer6;

    void Start() {
        cam = GetComponent<Camera>();
        roundControl = GameObject.Find("RoundControl").GetComponent<RoundControl>();
        initialZ = transform.position.z; // Store the initial Z position
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

void LateUpdate()
{
    if (players.Count == 0)
        return;

    if (roundControl.placingItems)
    {
        ZoomOutToSeeMap();
    }
    else
    {
        Move();
        Zoom();
    }
}

    void ZoomOutToSeeMap()
    {
        // Define a custom bounding box or rectangle that encapsulates the desired area
        Bounds customBounds = new Bounds(mapObject.transform.position, new Vector3(width, height, 0f));

        // Calculate the aspect ratio of the custom bounds
        float customAspectRatio = customBounds.size.x / customBounds.size.y;

        // Calculate the target size for the camera based on the larger dimension of the custom bounds
        float targetSize = Mathf.Max(customBounds.size.x, customBounds.size.y) * 0.5f;

        // Adjust the target size based on the aspect ratio of the custom bounds and the camera's aspect ratio
        targetSize = Mathf.Max(targetSize, Mathf.Max(minZoom, maxZoom / cam.aspect * customAspectRatio));

        // Set the camera's orthographic size to match the adjusted target size
        cam.orthographicSize = targetSize;

        // Set the camera's position to the center of the custom bounds
        transform.position = customBounds.center + offset;
    }

void Zoom()
{
    float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);

    // Calculate the maximum allowed zoom-out position based on the width and height of the map
    float maxZoomOutWidth = mapObject.GetComponent<Renderer>().bounds.size.x * 0.5f;
    float maxZoomOutHeight = mapObject.GetComponent<Renderer>().bounds.size.y * 0.5f;

    // Limit the new zoom value based on the maximum zoom-out position
    newZoom = Mathf.Min(newZoom, Mathf.Max(maxZoomOutWidth / cam.aspect, maxZoomOutHeight));

    // Calculate the maximum allowed zoom-in position based on the width and height of the map
    float maxZoomInWidth = width * 0.5f;
    float maxZoomInHeight = height * 0.5f;

    // Limit the new zoom value based on the maximum zoom-in position
    newZoom = Mathf.Max(newZoom, Mathf.Min(maxZoomInWidth / cam.aspect, maxZoomInHeight));

    cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
}

    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();

        // Calculate the maximum allowed position based on the width and height of the map
        float maxPositionX = mapObject.transform.position.x + mapObject.GetComponent<Renderer>().bounds.size.x * 0.5f - width * 0.5f;
        float minPositionX = mapObject.transform.position.x - mapObject.GetComponent<Renderer>().bounds.size.x * 0.5f + width * 0.5f;
        float maxPositionY = mapObject.transform.position.y + mapObject.GetComponent<Renderer>().bounds.size.y * 0.5f - height * 0.5f;
        float minPositionY = mapObject.transform.position.y - mapObject.GetComponent<Renderer>().bounds.size.y * 0.5f + height * 0.5f;

        float newX = Mathf.Clamp(centerPoint.x, minPositionX, maxPositionX);
        float newY = Mathf.Clamp(centerPoint.y, minPositionY, maxPositionY);

        Vector3 newPosition = new Vector3(newX, newY, initialZ) + offset; // Use the initial Z position

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
