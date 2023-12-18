using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class MultiTargetCamera : MonoBehaviour
{
    public List<Player> players = new List<Player>();
    public GameObject mapObject;
    public float smoothTime = 0.5f;
    private Vector3 velocity;
    public float minZoom = 25f;
    public float maxZoom = 9f;
    public float zoomLimiter = 35f;
    public float height = 10f;
    public float width = 10f;
    [SerializeField] private RoundControl roundControl;

    public Vector3 offset;

    [SerializeField] private Camera cam;
    private float initialZ;

    private void Start()
    {
        mapObject = GameObject.Find("MapArea");
        // Set the cam reference for the local player
        cam = Camera.main;
        
        initialZ = transform.position.z;

        // Assuming each player has a Player script attached
        Player[] playersInScene = FindObjectsOfType<Player>();
        foreach (Player player in playersInScene)
        {
            AddPlayer(player);
        }

        StartCoroutine(WaitForRoundControl());
    }

    private void LateUpdate()
    {
        if (roundControl != null && roundControl.placingItems)
        {
            ZoomOutToSeeMap();
        }
        else
        {
            if(roundControl != null && roundControl.victoryScreen && !roundControl.victoryTimer){
                mapObject = GameObject.Find("VictoryBackground");
                ZoomOutToSeeMap();
            }else{

                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].isLocalKing)
                    {
                        ZoomOutToSeeMap();
                        //Debug.Log("Zooming Out");
                    }else{
                        Move();
                        Zoom();
                    }
                }
            }
        }
    }

    // Method to add a player to the players list
    public void AddPlayer(Player newPlayer)
    {
        players.Add(newPlayer);
    }

    // Method to remove a player from the players list
    public void RemovePlayer(Player playerToRemove)
    {
        players.Remove(playerToRemove);
    }

    void ZoomOutToSeeMap()
    {
        Bounds customBounds = new Bounds(mapObject.transform.position, new Vector3(width, height, 0f));
        float customAspectRatio = customBounds.size.x / customBounds.size.y;
        float targetSize = Mathf.Max(customBounds.size.x, customBounds.size.y) * 0.5f;
        targetSize = Mathf.Max(targetSize, Mathf.Max(minZoom, maxZoom / cam.aspect * customAspectRatio));

        // Calculate the maximum and minimum orthographic size based on map bounds
        float maxOrthographicSize = Mathf.Min(customBounds.size.x / cam.aspect, customBounds.size.y);
        float minOrthographicSize = Mathf.Max(width / cam.aspect, height);

        // Clamp the targetSize to stay within the bounds of the map area
        targetSize = Mathf.Clamp(targetSize, minOrthographicSize, maxOrthographicSize);

        // Set the new orthographic size while maintaining the current aspect ratio
        cam.orthographicSize = targetSize;

        // Calculate the center position of the map
        Vector3 mapCenter = customBounds.center;

        // Calculate the camera position based on the map center and the new orthographic size
        Vector3 newPosition = new Vector3(mapCenter.x, mapCenter.y, initialZ) + offset;
        
        // Apply the new camera position
        transform.position = newPosition;
    }

    void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);

        float maxZoomOutWidth = mapObject.GetComponent<Renderer>().bounds.size.x * 0.5f;
        float maxZoomOutHeight = mapObject.GetComponent<Renderer>().bounds.size.y * 0.5f;
        newZoom = Mathf.Min(newZoom, Mathf.Max(maxZoomOutWidth / cam.aspect, maxZoomOutHeight));

        float maxZoomInWidth = width * 0.5f;
        float maxZoomInHeight = height * 0.5f;
        newZoom = Mathf.Max(newZoom, Mathf.Min(maxZoomInWidth / cam.aspect, maxZoomInHeight));

        // Calculate the maximum and minimum orthographic size based on map bounds
        float maxOrthographicSize = Mathf.Min(maxZoomOutWidth / cam.aspect, maxZoomOutHeight);
        float minOrthographicSize = Mathf.Max(maxZoomInWidth / cam.aspect, maxZoomInHeight);

        // Clamp the newZoom to stay within the bounds of the map area
        newZoom = Mathf.Clamp(newZoom, minOrthographicSize, maxOrthographicSize);

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
    }


    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        float maxPositionX = mapObject.transform.position.x + mapObject.GetComponent<Renderer>().bounds.size.x * 0.5f - cam.orthographicSize * cam.aspect;
        float minPositionX = mapObject.transform.position.x - mapObject.GetComponent<Renderer>().bounds.size.x * 0.5f + cam.orthographicSize * cam.aspect;
        float maxPositionY = mapObject.transform.position.y + mapObject.GetComponent<Renderer>().bounds.size.y * 0.5f - cam.orthographicSize;
        float minPositionY = mapObject.transform.position.y - mapObject.GetComponent<Renderer>().bounds.size.y * 0.5f + cam.orthographicSize;

        float newX = Mathf.Clamp(centerPoint.x, minPositionX, maxPositionX);
        float newY = Mathf.Clamp(centerPoint.y, minPositionY, maxPositionY);
        Vector3 newPosition = new Vector3(newX, newY, initialZ) + offset;
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    float GetGreatestDistance()
    {
        var bounds = new Bounds(Vector3.zero, Vector3.zero);
        bool firstPlayer = true;

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] != null && !players[i].isKing)
            {
                if (firstPlayer)
                {
                    bounds = new Bounds(players[i].transform.position, Vector3.zero);
                    firstPlayer = false;
                }
                else
                {
                    bounds.Encapsulate(players[i].transform.position);
                }
            }
        }

        return bounds.size.x;
    }

    Vector3 GetCenterPoint()
    {
        if (players.Count == 1)
        {
            return players[0].transform.position;
        }

        var bounds = new Bounds(Vector3.zero, Vector3.zero);
        bool firstPlayer = true;

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] != null && !players[i].isKing)
            {
                if (firstPlayer)
                {
                    bounds = new Bounds(players[i].transform.position, Vector3.zero);
                    firstPlayer = false;
                }
                else
                {
                    bounds.Encapsulate(players[i].transform.position);
                }
            }
        }

        return bounds.center;
    }

    IEnumerator WaitForRoundControl() {
        while (roundControl == null) {
            GameObject roundControlObject = GameObject.Find("RoundControl(Clone)");

            if (roundControlObject!= null) {
                roundControl = roundControlObject.GetComponent<RoundControl>();

                break;
            }

            yield return null; // Wait for a frame before checking again
        }
    }
}
