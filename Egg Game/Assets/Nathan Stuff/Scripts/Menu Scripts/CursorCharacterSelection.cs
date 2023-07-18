using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using Mirror;

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

    public PlayerSaveData playerSaveData;

    private void Awake()
    {
        clickableImages = GameObject.FindObjectsOfType<ClickableImage>();

        cursorCollider = GetComponent<Collider2D>();

        Application.focusChanged += OnGameFocusChanged;
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

    [Command]
    private void CmdMoveCursor(Vector3 position)
    {
        // Update the cursor position on the server
        transform.position = position;
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

                break;
            }
        }
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

    private void ResetMovementInput()
    {
        // Reset the movement input
        Vector2 movementInput = Vector2.zero;
    }
}
