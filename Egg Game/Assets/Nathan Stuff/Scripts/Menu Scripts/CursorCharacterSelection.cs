using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;
using System;

public class CursorCharacterSelection : MonoBehaviour
{
    PlayerSaveData playerSaveData;
    public float speed = 5f;
    public Color selectedColor = Color.green; // Color to apply to the selected object

    private PlayerInput playerInput;
    private Vector2 movementInput;

    private GameObject selectedObject; // The currently selected object
    private Image selectedImage; // The Image component of the selected object
    private Color originalColor; // The original color of the selected object

    [SerializeField]
    private ClickableImage[] clickableImages; // Array to store the ClickableImage components

    private Collider2D cursorCollider; // Collider component of the cursor object

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        PlayerSaveData.playerNumber += 1;
        clickableImages = GameObject.FindGameObjectsWithTag("button" + playerInput.playerIndex)
            .Concat(GameObject.FindGameObjectsWithTag("SpecialButton"))
            .Select(go => go.GetComponent<ClickableImage>())
            .Where(clickableImage => clickableImage != null)
            .ToArray();

        UnityEngine.Cursor.visible = false;

        cursorCollider = GetComponent<Collider2D>(); // Assign the collider component of the cursor object
    }

    private void Update()
    {
        // Process movement input
        Vector2 movement = movementInput.normalized;
        Vector3 newPosition = transform.position + new Vector3(movement.x, movement.y, 0) * speed * Time.deltaTime;

        // Update the cursor position based on input
        if (playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            // Get the mouse position in world coordinates
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Clamp the cursor position to stay within the screen boundaries
            ClampPosition(mousePosition);
        }
        else if (playerInput.currentControlScheme == "Gamepad")
        {
            ClampPosition(newPosition);
        }

        // Check for selection
        if (selectedObject != null)
        {
            // Perform selection logic on the selected object
            selectedImage.color = selectedColor;
        }
    }

    private void ClampPosition(Vector3 position)
    {
        // Get the screen boundaries in world coordinates
        float screenAspect = (float)Screen.width / Screen.height;
        float screenHorizontalSize = Camera.main.orthographicSize * screenAspect;
        float screenVerticalSize = Camera.main.orthographicSize;

        // Get the object's dimensions based on the colliders
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        float objectWidth = colliders.Max(collider => collider.bounds.size.x);
        float objectHeight = colliders.Max(collider => collider.bounds.size.y);

        // Clamp the position to stay within the screen boundaries
        float clampedX = Mathf.Clamp(position.x, -screenHorizontalSize + objectWidth / 2f, screenHorizontalSize - objectWidth / 2f);
        float clampedY = Mathf.Clamp(position.y, -screenVerticalSize + objectHeight / 2f, screenVerticalSize - objectHeight / 2f);
        float clampedZ = transform.position.z;

        transform.position = new Vector3(clampedX, clampedY, clampedZ);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

public void OnClick(InputAction.CallbackContext context)
{
    if (context.performed)
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
                HandleSelection(imageIndex);
                break;
            }
        }
    }
}


private void HandleSelection(int imageIndex)
{
    if (imageIndex >= 0 && imageIndex < clickableImages.Length)
    {
        Debug.Log("Player " + playerInput.playerIndex + " selected image " + imageIndex);

        // Deselect the previously selected object if there is any
        if (selectedObject != null)
        {
            selectedImage.color = originalColor;
        }

        // Select the new object
        selectedObject = clickableImages[imageIndex].gameObject;
        selectedImage = selectedObject.GetComponent<Image>();
        originalColor = selectedImage.color;

        // Apply the selected color to the new object
        selectedImage.color = selectedColor;

        // Simulate the click event
        clickableImages[imageIndex].onClick.Invoke();
    }
}
}
