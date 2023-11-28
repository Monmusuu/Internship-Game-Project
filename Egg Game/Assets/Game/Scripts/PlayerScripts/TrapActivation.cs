using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class TrapActivation : NetworkBehaviour
{
    public GameObject customCursor; // Assign your custom cursor GameObject in the inspector
    public Tilemap kingTilemap;
    public GameObject boundingObject;
    private bool isGameFocused = true;

    private void Start()
    {
        boundingObject = GameObject.Find("MapArea");
        kingTilemap = GameObject.Find("KingTilemap").GetComponent<Tilemap>();
        Application.focusChanged += OnApplicationFocus;
    }

    void Update()
    {
        if (!isOwned) return;
        if (!isGameFocused) return; // Only process input if the game is focused
        if (!isLocalPlayer) return;
        
        // Process movement input based on the mouse position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 newPosition = new Vector3(mousePosition.x, mousePosition.y, 0);

        // Limit the cursor's movement within the bounds of the boundingObject
        Vector3 clampedPosition = LimitPositionWithinBounds(newPosition);

        // Update the cursor's position
        transform.position = clampedPosition;

        if (Input.GetMouseButtonDown(0))
        {
            CmdHandleClick(newPosition);
        }
    }

    private Vector3 LimitPositionWithinBounds(Vector3 position)
    {
        // Get the object's dimensions based on its colliders
        Collider2D[] colliders = boundingObject.GetComponentsInChildren<Collider2D>();
        Bounds objectBounds = new Bounds();

        foreach (Collider2D collider in colliders)
        {
            objectBounds.Encapsulate(collider.bounds);
        }

        // Calculate the clamped position based on the object's bounds
        float clampedX = Mathf.Clamp(position.x, objectBounds.min.x, objectBounds.max.x);
        float clampedY = Mathf.Clamp(position.y, objectBounds.min.y, objectBounds.max.y);
        float clampedZ = position.z;

        return new Vector3(clampedX, clampedY, clampedZ);
    }

    [Command]
    private void CmdHandleClick(Vector3 cursorPosition)
    {
        // Set up the layer mask to include only the desired layers for raycasting
        int layerMask = LayerMask.GetMask("King"); // Replace "YourLayerNameX" with your actual layer names

        // Cast a ray from the cursor's position
        RaycastHit2D hit = Physics2D.Raycast(cursorPosition, Vector2.zero, float.MaxValue, layerMask);

        if (hit.collider != null)
        {
            // Check if the clicked object has a Vacuum component
            Vacuum vacuum = hit.collider.GetComponent<Vacuum>();
            if (vacuum != null)
            {
                NetworkIdentity vacuumNetId = vacuum.GetComponentInParent<NetworkIdentity>();

                if (vacuumNetId != null)
                {
                    vacuumNetId.AssignClientAuthority(connectionToClient);
                }
                else
                {
                    Debug.LogError("NetworkIdentity not found on the parent of the Vacuum component.");
                }

                // Call the ActivateVacuum method on the client that owns the Vacuum
                vacuum.TargetActivateVacuum(connectionToClient);
            }
            else
            {
                // Handle other objects as before
                HandleClickOnObject(hit.collider.gameObject);
            }
        }
    }

    [Client]
    private void HandleClickOnObject(GameObject clickedObject)
    {
        GuillotineScript guillotineScript = clickedObject.GetComponent<GuillotineScript>();
        if (guillotineScript != null)
        {
            guillotineScript.ActivateFunction();
            Debug.Log("Activated Guillotine");
        }

        LightBulb lightBulb = clickedObject.GetComponent<LightBulb>();
        if (lightBulb != null)
        {
            SpriteRenderer childSpriteRenderer = lightBulb.GetComponentInChildren<SpriteRenderer>();
            if (childSpriteRenderer != null)
            {
                Sprite sprite = childSpriteRenderer.sprite;
                lightBulb.ActivateFunction(sprite);
                Debug.Log("Activated LightBulb");
            }
        }

        OliveTurret oliveTurret = clickedObject.GetComponent<OliveTurret>();
        if (oliveTurret != null)
        {
            oliveTurret.ActivateFunction();
            Debug.Log("Activated Olive Turret");
        }

        TrapMicrophone trapMicrophone = clickedObject.GetComponent<TrapMicrophone>();
        if (trapMicrophone != null)
        {
            Debug.Log("Activated Microphone");
            trapMicrophone.ActivateFunction();
        }

        FlameThrower flameThrower = clickedObject.GetComponent<FlameThrower>();
        if(flameThrower!= null){
            Debug.Log("Activated FlameThrower");
            flameThrower.ActivateFunction();
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from the application focus event
        Application.focusChanged -= OnApplicationFocus;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        isGameFocused = hasFocus;
    }
}