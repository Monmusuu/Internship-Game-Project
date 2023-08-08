using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class TrapActivation : NetworkBehaviour
{
    public GameObject customCursor; // Assign your custom cursor GameObject in the inspector
    public Tilemap kingTilemap;

    private void Start()
    {
        kingTilemap = GameObject.Find("KingTilemap").GetComponent<Tilemap>();
    }

    void Update()
    {
        if (!isOwned) return;
        
        // Process movement input based on the mouse position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 newPosition = new Vector3(mousePosition.x, mousePosition.y, 0);

        ClampPosition(newPosition);
        CmdMoveCursor(transform.position);

        if (Input.GetMouseButtonDown(0))
        {
            CmdHandleClick(newPosition);
        }
    }

    [Command]
    private void CmdMoveCursor(Vector3 position)
    {
        // Update the cursor position on the server
        transform.position = position;
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

    [Command]
    private void CmdHandleClick(Vector3 cursorPosition)
    {
        // Set up the layer mask to include only the desired layers for raycasting
        int layerMask = LayerMask.GetMask("King"); // Replace "YourLayerNameX" with your actual layer names

        // Cast a ray from the cursor's position
        RaycastHit2D hit = Physics2D.Raycast(cursorPosition, Vector2.zero, float.MaxValue, layerMask);

        if (hit.collider != null)
        {
            HandleClickOnObject(hit.collider.gameObject);
        }
    }

     [Client]
    private void HandleClickOnObject(GameObject clickedObject)
    {
        GuillotineScript guillotineScript = clickedObject.GetComponent<GuillotineScript>();
        if (guillotineScript != null)
        {
            guillotineScript.ActivateFunction();
        }

        LightBulb lightBulb = clickedObject.GetComponent<LightBulb>();
        if (lightBulb != null)
        {
            SpriteRenderer childSpriteRenderer = lightBulb.GetComponentInChildren<SpriteRenderer>();
            if (childSpriteRenderer != null)
            {
                Sprite sprite = childSpriteRenderer.sprite;
                lightBulb.ActivateFunction(sprite);
            }
        }

        OliveTurret oliveTurret = clickedObject.GetComponent<OliveTurret>();
        if (oliveTurret != null)
        {
            oliveTurret.ActivateFunction();
        }

        TrapMicrophone trapMicrophone = clickedObject.GetComponent<TrapMicrophone>();
        if (trapMicrophone != null)
        {
            trapMicrophone.ActivateFunction();
        }
    }
}