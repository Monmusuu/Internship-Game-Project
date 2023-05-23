using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapSelection : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Camera mainCamera;
    public Transform[] mapButtons;  // Array of map buttons
    public Transform[] cursors;     // Array of cursor objects
    public Transform pointer;

    private Vector2[] cursorMoveInputs;  // Array to store move inputs for each cursor
    private EventSystem eventSystem;
    private Dictionary<int, MapButton> cursorButtonDictionary = new Dictionary<int, MapButton>();

    private void Awake()
    {
        eventSystem = EventSystem.current;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        cursorMoveInputs = new Vector2[cursors.Length];
    }

    private void FixedUpdate()
    {
        // Update cursor positions
        for (int i = 0; i < cursors.Length; i++)
        {
            Vector2 cursorInput = GetCursorInput(i);

            // Stop cursor movement if the input is zero
            if (cursorInput == Vector2.zero)
            {
                cursorMoveInputs[i] = Vector2.zero;
            }

            Vector3 newPos = cursors[i].position + new Vector3(cursorInput.x, cursorInput.y, 0f) * moveSpeed * Time.fixedDeltaTime;
            cursors[i].position = newPos;
        }
    }

    private void Update()
    {
        // Cast a ray from the pointer's position
        Vector2 pointerPos = mainCamera.WorldToScreenPoint(pointer.position);
        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.position = pointerPos;

        List<RaycastResult> results = new List<RaycastResult>();
        eventSystem.RaycastAll(eventData, results);

        // Check if any map button is selected by any cursor
        foreach (RaycastResult result in results)
        {
            for (int i = 0; i < mapButtons.Length; i++)
            {
                if (result.gameObject.transform == mapButtons[i])
                {
                    MapButton selectedButton = mapButtons[i].GetComponent<MapButton>();

                    // Check if the button is already selected by another cursor
                    bool isButtonSelected = false;
                    foreach (var kvp in cursorButtonDictionary)
                    {
                        if (kvp.Value == selectedButton)
                        {
                            isButtonSelected = true;
                            break;
                        }
                    }

                    // If the button is not selected by any cursor, select it
                    if (!isButtonSelected)
                    {
                        selectedButton.GetComponent<Button>().Select();
                        Debug.Log("Button Selected: " + selectedButton.name);

                        // Update the cursor-button dictionary
                        cursorButtonDictionary[GetCursorId(i)] = selectedButton;
                    }

                    break;
                }
            }
        }

        // Check if no map button is selected by any cursor
        if (eventSystem.currentSelectedGameObject == null)
        {
            for (int i = 0; i < cursors.Length; i++)
            {
                // If a cursor had a selected button previously, deselect it
                if (cursorButtonDictionary.ContainsKey(GetCursorId(i)))
                {
                    MapButton previouslySelectedButton = cursorButtonDictionary[GetCursorId(i)];
                    Debug.Log("Button Deselected: " + previouslySelectedButton.name);
                    cursorButtonDictionary.Remove(GetCursorId(i));
                }
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        InputDevice device = context.control.device;
        int cursorIndex = -1;

        // Map specific devices to cursor indices
        if (device == Keyboard.current)
        {
            // Apply dead zone for keyboard input
            if (moveInput.magnitude < 0.1f)
            {
                moveInput = Vector2.zero;
            }

            // Check if gamepad cursor is not moving
            if (cursorMoveInputs[1] == Vector2.zero)
            {
                cursorIndex = 0; // Map Keyboard to Cursor 0
            }
        }
        else if (device == Gamepad.current)
        {
            // Apply dead zone for gamepad input
            if (moveInput.magnitude < 0.2f)
            {
                moveInput = Vector2.zero;
            }

            cursorIndex = 1; // Map Gamepad to Cursor 1
        }

        if (IsValidCursorIndex(cursorIndex))
        {
            cursorMoveInputs[cursorIndex] = moveInput;
        }
        else
        {
            Debug.LogError("Invalid cursorIndex: " + cursorIndex);
        }
    }

    private int GetCursorId(int cursorIndex)
    {
        // Return the cursor ID based on the cursor index
        if (IsValidCursorIndex(cursorIndex))
        {
            return cursorIndex;
        }
        else
        {
            Debug.LogError("Invalid cursorIndex: " + cursorIndex);
            return 0;
        }
    }

    public Vector2 GetCursorInput(int cursorIndex)
    {
        // Check if the cursorIndex is valid
        if (IsValidCursorIndex(cursorIndex))
        {
            // Return the input value for the specified cursorIndex
            return cursorMoveInputs[cursorIndex];
        }
        else
        {
            Debug.LogError("Invalid cursorIndex: " + cursorIndex);
            return Vector2.zero;
        }
    }

    private bool IsValidCursorIndex(int cursorIndex)
    {
        // Check if the cursorIndex is within the valid range
        return cursorIndex >= 0 && cursorIndex < cursors.Length;
    }
}