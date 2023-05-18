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

    private void Awake()
    {
        eventSystem = EventSystem.current;
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
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void ClickButton(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            Button[] buttons = FindObjectsOfType<Button>();

            if (buttons.Length > 0)
            {
                Button nearestButton = buttons[0];
                float nearestDistance = Vector3.Distance(transform.position, nearestButton.transform.position);

                for (int i = 1; i < buttons.Length; i++)
                {
                    float distance = Vector3.Distance(transform.position, buttons[i].transform.position);

                    if (distance < nearestDistance)
                    {
                        nearestButton = buttons[i];
                        nearestDistance = distance;
                    }
                }

                eventSystem.SetSelectedGameObject(nearestButton.gameObject);

                // Perform additional logic with the context if needed
                // For example, you can check if the button is clicked
                if (context.performed)
                {
                    nearestButton.onClick.Invoke();
                }
            }
        }
    }
}