using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Mirror;
using System.Linq;

[System.Serializable]
public class MapObjectContainer
{
    public GameObject[] mapObjects;
}

public class MapSelection : NetworkBehaviour
{
    public float speed = 5f;
    private Vector2 movementInput;

    [SerializeField]
    private Button mapButton;

    private Collider2D cursorCollider;

    // Game focus variable
    private bool isGameFocused = true;

    [SerializeField]
    private MapObjectContainer mapObjectContainer;

    private void Awake()
    {
        cursorCollider = GetComponent<Collider2D>();
        Application.focusChanged += OnGameFocusChanged;

        // If the mapObjectContainer is not assigned, find map objects with the "Map" tag
        if (mapObjectContainer.mapObjects == null || mapObjectContainer.mapObjects.Length == 0)
        {
            // Manually find and sort the map objects by their names
            List<GameObject> maps = new List<GameObject>();
            int mapIndex = 1;
            GameObject mapObject;
            while ((mapObject = GameObject.Find("Map " + mapIndex)) != null)
            {
                maps.Add(mapObject);
                mapIndex++;
            }

            mapObjectContainer.mapObjects = maps.ToArray();
        }
    }

    private void OnGameFocusChanged(bool hasFocus)
    {
        isGameFocused = hasFocus;

        // If the game lost focus, reset the movement input
        if (!isGameFocused)
        {
            ResetMovementInput();
        }
    }

    private void Start()
    {
        UnityEngine.Cursor.visible = false;

        // Find and assign the button object
        mapButton = GameObject.Find("Back Button").GetComponent<Button>();
    }

    private void Update()
    {
        if (!isGameFocused) return; // Only process input if the game is focused

        // Process movement input based on the mouse position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 newPosition = new Vector3(mousePosition.x, mousePosition.y, 0);

        // Clamp the cursor position to stay within the screen boundaries
        ClampPosition(newPosition);

        if (isLocalPlayer && NetworkClient.ready)
        {
            CmdMoveCursor(transform.position);
        }

        // Process mouse click
        if (isLocalPlayer && Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
    }

    [Command]
    private void CmdMoveCursor(Vector3 position)
    {
        // Update the cursor position on the server
        transform.position = position;
    }

    private void ResetMovementInput()
    {
        // Reset the movement input
        movementInput = Vector2.zero;
    }

    private void ClampPosition(Vector3 position)
    {
        // Get the screen boundaries in world coordinates
        float screenAspect = (float)Screen.width / Screen.height;
        float screenHorizontalSize = Camera.main.orthographicSize * screenAspect;
        float screenVerticalSize = Camera.main.orthographicSize;

        // Get the object's dimensions based on the colliders
        float objectWidth = cursorCollider.bounds.size.x;
        float objectHeight = cursorCollider.bounds.size.y;

        // Clamp the position to stay within the screen boundaries
        float clampedX = Mathf.Clamp(position.x, -screenHorizontalSize + objectWidth / 2f, screenHorizontalSize - objectWidth / 2f);
        float clampedY = Mathf.Clamp(position.y, -screenVerticalSize + objectHeight / 2f, screenVerticalSize - objectHeight / 2f);
        float clampedZ = transform.position.z;

        transform.position = new Vector3(clampedX, clampedY, clampedZ);
    }

    public override void OnStartAuthority()
    {
        // Enable input handling for the local player
        enabled = true;
    }

    public override void OnStopAuthority()
    {
        // Disable input handling for non-local players
        enabled = false;
    }

    private void HandleMouseClick()
    {
        if (mapButton != null && RectTransformUtility.RectangleContainsScreenPoint(mapButton.GetComponent<RectTransform>(), Input.mousePosition))
        {
            // Handle the button click event
            mapButton.onClick.Invoke();
        }
        else
        {
            // Find the exact map index
            int exactMapIndex = GetExactMapIndex(mapObjectContainer.mapObjects);

            if (exactMapIndex != -1)
            {
                // Vote for the selected map
                CmdVoteForMap(exactMapIndex);
            }
        }
    }

    [Command]
    private void CmdVoteForMap(int mapIndex)
    {
        // Call the voting system's Vote method on the server
        VotingSystem.Instance.Vote(GetComponent<NetworkIdentity>().connectionToClient.connectionId, mapIndex);
    }

    private int GetExactMapIndex(GameObject[] mapObjects)
    {
        int mapIndex = -1;
        Vector3 playerPosition = transform.position;

        for (int i = 0; i < mapObjects.Length; i++)
        {
            GameObject mapObject = mapObjects[i];

            // Check for CanvasRenderer
            CanvasRenderer canvasRenderer = mapObject.GetComponent<CanvasRenderer>();
            if (canvasRenderer != null)
            {
                RectTransform rectTransform = mapObject.GetComponent<RectTransform>();
                if (rectTransform != null && RectTransformUtility.RectangleContainsScreenPoint(rectTransform, playerPosition))
                {
                    // Player is within the bounds of the current map object
                    mapIndex = i;
                    break;
                }
            }

            // Check for SpriteRenderer
            SpriteRenderer spriteRenderer = mapObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                if (spriteRenderer.bounds.Contains(playerPosition))
                {
                    // Player is within the bounds of the current map object
                    mapIndex = i;
                    break;
                }
            }
        }

        return mapIndex;
    }
}
