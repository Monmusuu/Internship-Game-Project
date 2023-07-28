using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MultiTargetCamera : NetworkBehaviour
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

    private Camera cam;
    private float initialZ;

    private void Start()
    {
        cam = GetComponent<Camera>();
        initialZ = transform.position.z;

        // If this script is on the local player object, set the offset
        if (isLocalPlayer)
        {
            offset = new Vector3(0f, 5f, -10f); // Adjust the values according to your needs
        }

        // Try to find the RoundControl component and log the result
        roundControl = FindObjectOfType<RoundControl>();
        if (roundControl != null)
        {
            Debug.Log("RoundControl found in MultiTargetCamera.");
        }
        else
        {
            Debug.LogWarning("RoundControl not found in MultiTargetCamera. Ensure that RoundControl exists in the scene and is active before the camera script starts.");
        }
    }

    private void LateUpdate()
    {
        // If the camera is not attached to the local player (host-side), follow the center point of all players
        if (!isLocalPlayer)
        {
            Move();
            Zoom();
        }
        else // If the camera is attached to the local player (client-side), follow the local player
        {
            if (roundControl.placingItems)
            {
                ZoomOutToSeeMap();
            }
            else
            {
                Move();
                // Zoom(); // We won't call the Zoom function on the client side.
            }
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
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
        cam.orthographicSize = targetSize;
        transform.position = customBounds.center + offset;

        // Synchronize camera zoom level and target position for clients
        currentZoomLevel = targetSize;
        cameraTargetPosition = transform.position;
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

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);

        // Synchronize camera zoom level for clients
        currentZoomLevel = cam.orthographicSize;
    }

    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        float maxPositionX = mapObject.transform.position.x + mapObject.GetComponent<Renderer>().bounds.size.x * 0.5f - width * 0.5f;
        float minPositionX = mapObject.transform.position.x - mapObject.GetComponent<Renderer>().bounds.size.x * 0.5f + width * 0.5f;
        float maxPositionY = mapObject.transform.position.y + mapObject.GetComponent<Renderer>().bounds.size.y * 0.5f - height * 0.5f;
        float minPositionY = mapObject.transform.position.y - mapObject.GetComponent<Renderer>().bounds.size.y * 0.5f + height * 0.5f;

        float newX = Mathf.Clamp(centerPoint.x, minPositionX, maxPositionX);
        float newY = Mathf.Clamp(centerPoint.y, minPositionY, maxPositionY);
        Vector3 newPosition = new Vector3(newX, newY, initialZ) + offset;
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);

        // Synchronize camera target position for clients
        cameraTargetPosition = transform.position;
    }

    float GetGreatestDistance()
    {
        var bounds = new Bounds(players[0].transform.position, Vector3.zero);
        for (int i = 0; i < players.Count; i++)
        {
            bounds.Encapsulate(players[i].transform.position);
        }
        return bounds.size.x;
    }

    Vector3 GetCenterPoint()
    {
        if (players.Count == 1)
        {
            return players[0].transform.position;
        }

        var bounds = new Bounds(players[0].transform.position, Vector3.zero);
        for (int i = 0; i < players.Count; i++)
        {
            bounds.Encapsulate(players[i].transform.position);
        }
        return bounds.center;
    }

    // SyncVars for camera zoom level and target position
    [SyncVar(hook = nameof(OnZoomLevelUpdated))]
    private float currentZoomLevel;

    [SyncVar(hook = nameof(OnCameraTargetPositionUpdated))]
    private Vector3 cameraTargetPosition;

    // Hook method for the camera zoom level sync var update.
    private void OnZoomLevelUpdated(float oldZoomLevel, float newZoomLevel)
    {
        cam.orthographicSize = newZoomLevel;
    }

    // Hook method for the camera target position sync var update.
    private void OnCameraTargetPositionUpdated(Vector3 oldPosition, Vector3 newPosition)
    {
        transform.position = newPosition;
    }
}
