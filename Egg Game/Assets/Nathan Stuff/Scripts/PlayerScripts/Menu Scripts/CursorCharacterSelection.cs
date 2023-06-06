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
    }

    // Update is called once per frame
    void Update()
    {
        // Process movement input
        Vector2 movement = movementInput.normalized;
        transform.Translate(movement * speed * Time.deltaTime);

        // Check for selection
        if (selectedObject != null)
        {
            // Perform selection logic on the selected object
            selectedCanvasRenderer.SetColor(selectedColor);
        }
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