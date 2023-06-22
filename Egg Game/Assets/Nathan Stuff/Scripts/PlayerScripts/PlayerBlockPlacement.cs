
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayerBlockPlacement : MonoBehaviour
{
    public Tilemap kingTilemap;
    public GameObject[] blockTileObjects;
    public int selectedTile = 0;
    public int removeTile = 0;
    public bool blockPlaced = false;

    public Transform tileGridUI;

    public PlayerInput playerInput;
    public float moveSpeed = 5f;

    private Vector2 movementInput;
    private GameObject previewObject;
    private SpriteRenderer previewSpriteRenderer;
    private Rigidbody2D rb;

    public float smoothingFactor = 0.5f; // Value between 0 and 1 (0 means no smoothing, 1 means maximum smoothing)
    private Vector2 smoothedInput;
    private Vector3 initialPosition;
    private RoundControl roundControl;

    public LayerMask previewLayer;
    public LayerMask kingLayer;
    public LayerMask borderLayer;

    private float rotationAngle = 0f;
    

    private void Awake()
    {
        initialPosition = transform.position;
    }

    private void Start()
    {
        roundControl = GameObject.Find("RoundControl").GetComponent<RoundControl>();
        playerInput = GetComponentInParent<PlayerInput>();
        kingTilemap = GameObject.Find("KingTilemap").GetComponent<Tilemap>();

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // Disable gravity for the cursor
    }

    private void Update()
    {
        movementInput = playerInput.actions["CursorMove"].ReadValue<Vector2>();
        Vector3 newPosition = transform.position + new Vector3(movementInput.x, movementInput.y, 0) * moveSpeed * Time.deltaTime;

        if (playerInput.currentControlScheme == "WADKeyBoard")
        {
            // Get the mouse position in world coordinates
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Clamp the cursor position to stay within the screen boundaries
            ClampPosition(mousePosition);
            MoveCursor();
        }
        else if (playerInput.currentControlScheme == "Controller")
        {
            movementInput = playerInput.actions["CursorMove"].ReadValue<Vector2>();
            ClampPosition(newPosition);
            MoveCursor();
        }

        selectedTile = 0;

        if (blockPlaced == true)
        {
            selectedTile = 3;
        }

        if (playerInput.actions["CursorClick"].triggered)
        {
            OnClick();
        }

        if (playerInput.actions["Rotate"].triggered)
        {
            RotatePreviewObject();
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

    private void FixedUpdate()
    {
        Vector2 velocity = movementInput * moveSpeed;
        rb.velocity = velocity;
    }

    private void MoveCursor()
    {
        Vector3Int cellPosition = kingTilemap.WorldToCell(transform.position);
        Vector3 tilePosition = kingTilemap.CellToWorld(cellPosition) + kingTilemap.cellSize / 2f;

        if (previewObject != null)
        {
            // Interpolate the movement between the current position and the target position
            float moveDistance = moveSpeed * Time.deltaTime;
            previewObject.transform.position = Vector3.Lerp(previewObject.transform.position, tilePosition, moveDistance);
        }
    }

public void OnClick()
{

    Vector3 cursorPosition = transform.position;
    Vector3Int cellPosition = kingTilemap.WorldToCell(cursorPosition);
    Vector3 tilePosition = kingTilemap.CellToWorld(cellPosition) + kingTilemap.cellSize / 2f;

    if (selectedTile == 0 && !blockPlaced)
    {
        // Check if the placement position is valid (e.g., within certain bounds or not occupied)
        if (IsPlacementValid(cellPosition))
        {
            transform.position = initialPosition;
            gameObject.layer = (int)Mathf.Log(kingLayer.value, 2); // Set the layer to the "King" layer
            blockPlaced = true;
            GameObject placedBlock = Instantiate(previewObject, tilePosition, Quaternion.Euler(0f, 0f, rotationAngle));
            SpriteRenderer placedSpriteRenderer = placedBlock.GetComponent<SpriteRenderer>();
            Color blockColor = placedSpriteRenderer.color;
            blockColor.a = 1f; // Set the alpha value to 1 (fully opaque)
            placedSpriteRenderer.color = blockColor;
            placedBlock.layer = (int)Mathf.Log(kingLayer.value, 2); // Set the layer of the placed block to the "King" layer

            // Change the layer of the placed block's children to the "King" layer as well
            foreach (Transform child in placedBlock.transform)
            {
                child.gameObject.layer = (int)Mathf.Log(kingLayer.value, 2);
            }

            roundControl.playersPlacedBlocks += 1;
            rotationAngle = 0f;
        }
        else
        {
            Debug.Log("Invalid placement position!");
            return; // Return without destroying the preview object
        }
    }

    Destroy(previewObject);
}

    private int previousRandomIndex = -1; // Store the previous random index

    private void OnEnable()
    {
        if (previewObject != null)
        {
            previewObject.SetActive(true);
            return; // Return early if there is a preview object already active
        }

        selectedTile = 0;
        blockPlaced = false;

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, blockTileObjects.Length); // Randomly select an index
        } while (randomIndex == previousRandomIndex); // Repeat until the index is different from the previous one

        previousRandomIndex = randomIndex; // Store the current random index as the previous one

        previewObject = Instantiate(blockTileObjects[randomIndex], transform.position, Quaternion.identity);
        previewSpriteRenderer = previewObject.GetComponent<SpriteRenderer>();
        previewSpriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
    }

    private void OnDisable()
    {
        if (previewObject != null)
        {
            previewObject.SetActive(false);
        }
    }

    private bool IsPlacementValid(Vector3Int position)
    {
        // Convert the position to world space
        Vector3 positionWorld = kingTilemap.CellToWorld(position) + kingTilemap.cellSize / 2f;

        // Perform a point overlap check with the colliders on the king and border layers
        Collider2D overlapColliderKing = Physics2D.OverlapPoint(positionWorld, kingLayer);
        Collider2D overlapColliderBorder = Physics2D.OverlapPoint(positionWorld, borderLayer);

        // Check if there is no overlap or if the overlap is with the current game object
        if ((overlapColliderKing == null || overlapColliderKing.gameObject == gameObject) && overlapColliderBorder == null)
        {
            // Placement is valid
            return true;
        }
        else
        {
            // Placement is invalid
            return false;
        }
    }

    private void RotatePreviewObject()
    {
        if (selectedTile == 0)
        {
            rotationAngle += 90f;
            if (rotationAngle >= 360f)
            {
                rotationAngle = 0f;
            }

            previewObject.transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);
        }
    }
}