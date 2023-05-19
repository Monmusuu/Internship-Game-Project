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
    private Vector2 velocity;
    private Vector2 moveInput;
    private EventSystem eventSystem;
    private Button selectedButton;

    public Transform pointer;

    private void Awake()
    {
        eventSystem = EventSystem.current;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    private void Update()
    {
        // Get player input
        velocity = moveInput * moveSpeed;

        // Update player position within camera space
        Vector3 newPos = transform.position + new Vector3(velocity.x, velocity.y, 0f) * Time.deltaTime;
        Vector3 clampedPos = mainCamera.WorldToViewportPoint(newPos);
        clampedPos.x = Mathf.Clamp01(clampedPos.x);
        clampedPos.y = Mathf.Clamp01(clampedPos.y);
        transform.position = mainCamera.ViewportToWorldPoint(clampedPos);

        // Check if any UI element is selected
        if (eventSystem.currentSelectedGameObject == null)
        {
            // Cast a ray from the pointer's position
            Vector2 pointerPos = mainCamera.WorldToScreenPoint(pointer.position);
            PointerEventData eventData = new PointerEventData(eventSystem);
            eventData.position = pointerPos;

            List<RaycastResult> results = new List<RaycastResult>();
            eventSystem.RaycastAll(eventData, results);

            foreach (RaycastResult result in results)
            {
                Button button = result.gameObject.GetComponent<Button>();
                if (button != null)
                {
                    selectedButton = button;
                    selectedButton.Select();
                    Debug.Log("Button Selected: " + selectedButton.name);
                    break;
                }
            }
        }
        else
        {
            selectedButton = eventSystem.currentSelectedGameObject.GetComponent<Button>();
            if (selectedButton != null)
            {
                Debug.Log("Button Selected: " + selectedButton.name);
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.action.triggered && selectedButton != null)
        {
            // Perform button click
            selectedButton.onClick.Invoke();
            
        }
    }
}