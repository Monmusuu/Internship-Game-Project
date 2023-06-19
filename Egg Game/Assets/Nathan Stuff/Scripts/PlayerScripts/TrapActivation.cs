using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrapActivation : MonoBehaviour
{
    public PlayerInput playerInput;
    public GameObject customCursor; // Assign your custom cursor GameObject in the inspector
    public Tilemap kingTilemap;
    private Vector2 movementInput;
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    private void Start()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        kingTilemap = GameObject.Find("KingTilemap").GetComponent<Tilemap>();

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // Disable gravity for the cursor
    }

    private void FixedUpdate()
    {
        Vector2 velocity = movementInput * moveSpeed;
        rb.velocity = velocity;
    }

    void Update()
    {
        movementInput = playerInput.actions["CursorMove"].ReadValue<Vector2>();

        if (movementInput != Vector2.zero)
        {
            MoveCursor();
        }

        if (playerInput.actions["CursorClick"].triggered)
        {
            OnClick();
        }
    }

public void OnClick()
{
    Debug.Log("OnClick called");

    // Calculate the cursor position in world space
    Vector3 cursorPosition = customCursor.transform.position;

    // Set up the layer mask to include only the desired layers for raycasting
    int layerMask = LayerMask.GetMask("King"); // Replace "YourLayerNameX" with your actual layer names

    // Cast a ray from the cursor's position
    RaycastHit2D hit = Physics2D.Raycast(cursorPosition, Vector2.zero, float.MaxValue, layerMask);

    if (hit.collider != null)
    {
        Debug.Log("Collider hit: " + hit.collider.gameObject.name);

        // Check if the raycast hits a GameObject with a specific script attached
        GuillotineScript guillotineScript = hit.collider.GetComponent<GuillotineScript>();
        if (guillotineScript != null)
        {
            guillotineScript.ActivateFunction();
        }

        // Check if the raycast hits a GameObject with a specific script attached
        LightBulb lightBulb = hit.collider.GetComponent<LightBulb>();
        if (lightBulb != null)
        {
            SpriteRenderer childSpriteRenderer = lightBulb.GetComponentInChildren<SpriteRenderer>();
            if (childSpriteRenderer != null)
            {
                Sprite sprite = childSpriteRenderer.sprite;
                lightBulb.ActivateFunction(sprite);
            }
        }

        OliveTurret oliveTurret = hit.collider.GetComponent<OliveTurret>();
        if(oliveTurret != null){
            oliveTurret.ActivateFunction();
        }
    }
    else
    {
        Debug.Log("No collider hit");
    }
}

    private void MoveCursor()
    {
        Vector3Int cellPosition = kingTilemap.WorldToCell(transform.position);
        Vector3 tilePosition = kingTilemap.CellToWorld(cellPosition) + kingTilemap.cellSize / 2f;
    }
}