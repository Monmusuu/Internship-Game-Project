using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;
public class CursorCharacterSelection : MonoBehaviour
{

    PlayerSaveData playerSaveData;
    public float speed = 5f;
    public float selectionRadius = 1f; // Radius within which objects can be selected
    public Color selectedColor = Color.green; // Color to apply to the selected object

    private PlayerInput playerInput;
    private Vector2 movementInput;

    private GameObject selectedObject; // The currently selected object
    private CanvasRenderer selectedCanvasRenderer; // The CanvasRenderer component of the selected object
    private Color originalColor; // The original color of the selected object

    public GameObject[] buttonObjects; // Array to store the button objects

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        PlayerSaveData.playerNumber += 1;
        buttonObjects = GameObject.FindGameObjectsWithTag("button" + playerInput.playerIndex)
        .Concat(GameObject.FindGameObjectsWithTag("SpecialButton"))
        .ToArray();

        UnityEngine.Cursor.visible = false;

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
            selectedCanvasRenderer.SetColor(selectedColor);
        }
    }

    private void ClampPosition(Vector3 position)
    {
        // Get the screen boundaries in world coordinates
        float screenAspect = (float)Screen.width / Screen.height;
        float screenHorizontalSize = Camera.main.orthographicSize * screenAspect;
        float screenVerticalSize = Camera.main.orthographicSize;

        // Get the object's dimensions
        Renderer renderer = GetComponent<Renderer>();
        float objectWidth = renderer.bounds.size.x;
        float objectHeight = renderer.bounds.size.y;

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
            // Find the closest button index
            int closestButtonIndex = GetClosestButtonIndex();

            // Handle the selection
            HandleSelection(closestButtonIndex);
        }
    }

    private int GetClosestButtonIndex()
    {
        int buttonIndex = -1;
        float closestDistance = Mathf.Infinity;
        Vector3 playerPosition = transform.position;

        for (int i = 0; i < buttonObjects.Length; i++)
        {
            Button buttonComponent = buttonObjects[i].GetComponent<Button>();
            if (buttonComponent != null && buttonComponent.interactable)
            {
                Vector3 buttonObjectPosition = buttonObjects[i].transform.position;
                float distance = Vector3.Distance(playerPosition, buttonObjectPosition);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    buttonIndex = i;
                }
            }
        }

        return buttonIndex;
    }

    private void HandleSelection(int buttonIndex)
    {
        if (buttonIndex >= 0 && buttonIndex < buttonObjects.Length)
        {
            Debug.Log("Player " + playerInput.playerIndex + " selected button " + buttonIndex);

            // Deselect the previously selected object if there is any
            if (selectedObject != null)
            {
                selectedCanvasRenderer.SetColor(originalColor);
            }

            // Select the new object
            selectedObject = buttonObjects[buttonIndex];
            selectedCanvasRenderer = selectedObject.GetComponent<CanvasRenderer>();
            originalColor = selectedCanvasRenderer.GetColor();

            // Apply the selected color to the new object
            selectedCanvasRenderer.SetColor(selectedColor);

            // Invoke the onClick event of the selected button object
            Button selectedButton = selectedObject.GetComponent<Button>();
            if (selectedButton != null)
            {
                selectedButton.onClick.Invoke();
            }
            else
            {
                Debug.Log("Button component not found on selected object.");
            }
        }
        else
        {
            Debug.Log("Invalid button index.");
        }
    }
}