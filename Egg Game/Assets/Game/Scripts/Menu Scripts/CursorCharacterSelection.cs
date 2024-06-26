using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using Mirror;


[System.Serializable]
public class MapObjectContainer
{
    public GameObject[] mapObjects;
}

public class CursorCharacterSelection : NetworkBehaviour
{
    // Serialized fields
    public float speed = 5f;
    public Color selectedColor = Color.green;

    // Selection variables
    private GameObject selectedObject;
    private Image selectedImage;
    private Color originalColor;

    [SerializeField]
    private ClickableImage[] clickableImages;

    private Collider2D cursorCollider;

    // Game focus variable
    private bool isGameFocused = true;

    [SerializeField]
    private MapObjectContainer mapObjectContainer;

    [SerializeField]
    private Button mapButton;

    [SerializeField] 
    private Transform MapCanvas;

    [SerializeField] 
    private GameObject MapCanvasHolder;

    [SerializeField]
    private MenuScript menuScript;

    [SerializeField]
    private VotingSystem votingSystem;

    public AudioSource audioSource; // Reference to the AudioSource component
    public AudioClip clickAudioClip; // The audio clip to be played

    private void Awake()
    {
        cursorCollider = GetComponent<Collider2D>();

        Application.focusChanged += OnGameFocusChanged;
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

    private void Start()
    {
        StartCoroutine(WaitForMapCanvas());
        StartCoroutine(WaitForVotingSystem());
        
        menuScript = FindObjectOfType<MenuScript>();
    }

    private void Update()
    {

        if (!isGameFocused) return; // Only process input if the game is focused

        // Process movement input based on the mouse position

        if (isLocalPlayer && !menuScript.isPause){
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 newPosition = new Vector3(mousePosition.x, mousePosition.y, 0);

            // Check if the mapCanvas is active
            ClampPosition(newPosition);

            if (NetworkClient.ready)
            {
                CmdMoveCursor(transform.position);
            }

            // Check if MapCanvas is null before using it
            if (MapCanvas == null)
            {
                return;
            }
            
            bool isMapSelectionActive = MapCanvas.gameObject.activeSelf;

            // Process mouse click
            if (Input.GetMouseButtonDown(0))
            {
                if(isMapSelectionActive){
                    //Debug.Log("Clicking Map");
                    HandleMapMouseClick();
                }else{
                    //Debug.Log("Clicking Customs");
                    HandleMouseClick();
                }
            }
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

    IEnumerator WaitForMapCanvas()
    {
        while (MapCanvasHolder == null) {

            MapCanvasHolder = GameObject.Find("MapCanvasHolder");

            if (MapCanvasHolder != null)
            {
                // Get the first child of MapCanvasHolder
                MapCanvas = MapCanvasHolder.transform.GetChild(0);

                // Now 'firstChild' holds a reference to the first child of MapCanvasHolder
            }
            yield return null; // Wait for a frame before checking again
        }

        // Find and assign the button object
        mapButton = MapCanvasHolder.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Button>();

        // If the mapObjectContainer is not assigned, find map objects by name
        if (mapObjectContainer.mapObjects == null || mapObjectContainer.mapObjects.Length == 0)
        {
            // Manually find and sort the map objects by their names
            List<GameObject> maps = new List<GameObject>();

            // Assuming MapCanvasHolderTransform is a reference to a Transform of the MapCanvasHolder
            Transform child2 = MapCanvasHolder.transform.GetChild(0).GetChild(3);

            // Accessing children in the range 1-6 under child2
            for (int i = 1; i <= 6; i++)
            {
                if (child2 != null && i - 1 < child2.childCount)
                {
                    // Add the child to the list
                    maps.Add(child2.GetChild(i - 1).gameObject);
                }
            }

            // Assign the found map objects to mapObjectContainer.mapObjects
            mapObjectContainer.mapObjects = maps.ToArray();
        }

        // Add a listener to the backButton's onClick event
        if (mapButton != null)
        {
            mapButton.onClick.AddListener(OnClickBackButton);
        }
    }

    private IEnumerator WaitForVotingSystem()
    {
        while (votingSystem == null)
        {
            votingSystem = FindObjectOfType<VotingSystem>();
            yield return null;
        }
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

    [Command]
    private void CmdMoveCursor(Vector3 position)
    {
        // Update the cursor position on the server
        transform.position = position;
    }

    private void ResetMovementInput()
    {
        // Reset the movement input
        Vector2 movementInput = Vector2.zero;
    }

    private void HandleMouseClick()
    {
        // Perform a collider-based check for object clicks
        Collider2D[] hitColliders = new Collider2D[10]; // Adjust the size if needed
        ContactFilter2D contactFilter = new ContactFilter2D().NoFilter();
        int hitCount = cursorCollider.OverlapCollider(contactFilter, hitColliders);

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hitCollider = hitColliders[i];

            ClickableImage clickedImage = hitCollider.GetComponent<ClickableImage>();
            int imageIndex = Array.IndexOf(clickableImages, clickedImage);

            // Only handle selection if a valid image is clicked
            if (imageIndex >= 0)
            {
                // Deselect the previously selected object if there is any
                if (selectedObject != null)
                {
                    DeselectObject();
                }

                // Select the new object
                selectedObject = clickableImages[imageIndex].gameObject;
                selectedImage = selectedObject.GetComponent<Image>();
                originalColor = selectedImage.color;

                // Apply the selected color to the new object
                selectedImage.color = selectedColor;

                // Simulate the click event
                clickableImages[imageIndex].onClick.Invoke();

                // Debug message
                Debug.Log("Selected object: " + selectedObject.name);
                audioSource.PlayOneShot(clickAudioClip);

                break;
            }
        }
    }

    private void HandleMapMouseClick()
    {
        if (mapButton != null && RectTransformUtility.RectangleContainsScreenPoint(mapButton.GetComponent<RectTransform>(), Input.mousePosition))
        {
            Debug.Log("Found Map");
            // Handle the button click event
            mapButton.onClick.Invoke();
        }
        else
        {
            // Find the exact map index
            int exactMapIndex = GetExactMapIndex(mapObjectContainer.mapObjects);

            if (exactMapIndex != -1)
            {
                Debug.Log("Found Map");
                // Vote for the selected map
                audioSource.PlayOneShot(clickAudioClip);
                CmdVoteForMap(exactMapIndex);
            }
        }
    }

    [Command]
    private void CmdVoteForMap(int mapIndex)
    {
        // Find the parent object with NetworkIdentity
        NetworkIdentity networkIdentity = GetComponentInParent<NetworkIdentity>();

        if (networkIdentity != null)
        {
            // Call the voting system's Vote method on the server
            VotingSystem.Instance.Vote(networkIdentity.connectionToClient.connectionId, mapIndex);
        }
        else
        {
            Debug.LogError("NetworkIdentity not found on parent object.");
        }
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

    [Command]
    private void CmdUndoVote()
    {
        // Find the parent object with NetworkIdentity
        NetworkIdentity networkIdentity = GetComponentInParent<NetworkIdentity>();

        if (networkIdentity != null)
        {
            // Call the voting system's Undo-Vote method on the server
            VotingSystem.Instance.UndoVote(networkIdentity.connectionToClient.connectionId);
        }
        else
        {
            Debug.LogError("NetworkIdentity not found on parent object.");
        }
    }

    [ClientRpc]
    private void RpcUndoVote()
    {
        // Update UI or perform other client-side actions after undoing the vote
    }

    public void OnClickBackButton()
    {
        Debug.Log("Local player clicked Back Button 2.");

        // Check if this is the local player
        if (!isLocalPlayer)
        {
            //Debug.LogWarning("Only the local player can undo the vote.");
            return;
        }

        // Call the command to undo the vote on the server
        CmdUndoVote();
    }

    private void DeselectObject()
    {
        // Deselect the previously selected object
        selectedImage.color = originalColor;

        // Clear the selection variables
        selectedObject = null;
        selectedImage = null;
        originalColor = Color.white;

        // Debug message
        Debug.Log("Deselected object.");
    }
}
