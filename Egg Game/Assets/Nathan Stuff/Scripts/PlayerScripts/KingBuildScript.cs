using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Mirror;

public class KingBuildScript : NetworkBehaviour
{
    // Variables
    public Tilemap kingTilemap;
    public GameObject[] autoTrapTileObjects;
    public GameObject[] manualTrapTileObjects;
    public GameObject[] manualTrap2TileObjects;
    private int selectedAutoTrapIndex;
    private int selectedManualTrapIndex;
    private int selectedManualTrap2Index;
    private int previousAutoTrapIndex;
    private int previousManualTrapIndex;
    private int previousManualTrap2Index;
    private int selectedTile = 0;
    //private int removeTile = 0;
    private bool autoTrapPlaced = false;
    private bool manualTrapPlaced = false;
    private bool manualTrapPlaced2 = false;

    public Transform tileGridUI;
    public float moveSpeed = 5f;

    private Vector2 movementInput;
    private GameObject previewObject;
    private SpriteRenderer previewSpriteRenderer;
    private Rigidbody2D rb;
    private Vector3 initialPosition;
    [SerializeField]
    private RoundControl roundControl;
    private int kingLayerValue;
    public LayerMask borderLayer;
    private Collider2D cursorCollider;
    private float rotationAngle = 0f;
    private bool isGameFocused = true;

    private void Awake()
    {
        cursorCollider = GetComponent<Collider2D>();
        initialPosition = transform.position;
    }

    private void Start()
    {
        kingLayerValue = LayerMask.NameToLayer("King");
        selectedTile = 0;
        kingTilemap = GameObject.Find("KingTilemap").GetComponent<Tilemap>();
        roundControl = GameObject.Find("RoundControl").GetComponent<RoundControl>();

        Application.focusChanged += OnApplicationFocus;

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // Disable gravity for the cursor

        // Reset the position to the initial position
        transform.localPosition = initialPosition;
    }

    private void OnEnable()
    {
        selectedTile = 0;

        autoTrapPlaced = false;
        manualTrapPlaced = false;
        manualTrapPlaced2 = false;

        // Randomly select a new tile from the auto trap array
        do
        {
            selectedAutoTrapIndex = Random.Range(0, autoTrapTileObjects.Length);
        } while (selectedAutoTrapIndex == previousAutoTrapIndex);

        // Randomly select a new tile from the manual trap array
        do
        {
            selectedManualTrapIndex = Random.Range(0, manualTrapTileObjects.Length);
        } while (selectedManualTrapIndex == previousManualTrapIndex);

        // Randomly select a new tile from the manual trap 2 array
        do
        {
            selectedManualTrap2Index = Random.Range(0, manualTrap2TileObjects.Length);
        } while (selectedManualTrap2Index == previousManualTrap2Index);

        previousAutoTrapIndex = selectedAutoTrapIndex;
        previousManualTrapIndex = selectedManualTrapIndex;
        previousManualTrap2Index = selectedManualTrap2Index;

        // Check if the previewObject is null
        if (previewObject == null)
        {
            // Set the randomly selected tiles as the initial preview objects
            CreatePreviewObject(autoTrapTileObjects, selectedAutoTrapIndex);
            CreatePreviewObject(manualTrapTileObjects, selectedManualTrapIndex);
            CreatePreviewObject(manualTrap2TileObjects, selectedManualTrap2Index);
        }
        else
        {
            // If the previewObject already exists, set it active again
            previewObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from the application focus event
        Application.focusChanged += OnApplicationFocus;

        if (previewObject != null)
        {
            previewObject.SetActive(false);
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        isGameFocused = hasFocus;

        // If the game lost focus, reset the movement input
        if (!isGameFocused)
        {
            ResetMovementInput();
        }
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        
        if (!isGameFocused) return; // Only process input if the game is focused

        // Process movement input based on the mouse position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 newPosition = new Vector3(mousePosition.x, mousePosition.y, 0);

        // Clamp the cursor position to stay within the screen boundaries
        ClampPosition(newPosition);

        // Check for scroll wheel input to rotate the trap preview
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0f)
        {
            RotatePreviewObject(scrollInput);
        }

        if (isLocalPlayer && NetworkClient.ready)
        {
            CmdMoveCursor(transform.position);
        }

        // Create the preview object based on the selected tile
        CreateAllPreviews();

        // Check for mouse click to place the trap
        if (isLocalPlayer && Input.GetMouseButtonDown(0))
        {
            Vector3 cursorPosition = transform.position;
            Vector3Int cellPosition = kingTilemap.WorldToCell(cursorPosition);
            Vector3 tilePosition = kingTilemap.CellToWorld(cellPosition) + kingTilemap.cellSize / 2f;
            CmdPlaceTrap(tilePosition, Quaternion.Euler(0f, 0f, rotationAngle));
        }
    }

    private void FixedUpdate()
    {
        Vector2 velocity = movementInput * moveSpeed;
        rb.velocity = velocity;
    }

    private void ResetMovementInput()
    {
        // Reset the movement input
        movementInput = Vector2.zero;
    }

    private void CreateAllPreviews()
    {
        if (!autoTrapPlaced && selectedTile == 0)
            CreatePreviewObject(autoTrapTileObjects, selectedAutoTrapIndex);
        else if (!manualTrapPlaced && selectedTile == 1)
            CreatePreviewObject(manualTrapTileObjects, selectedManualTrapIndex);
        else if (!manualTrapPlaced2 && selectedTile == 2 && manualTrap2TileObjects.Length > 0)
            CreatePreviewObject(manualTrap2TileObjects, selectedManualTrap2Index);
        else
            Destroy(previewObject);
    }

    private void CreatePreviewObject(GameObject[] tileObjects, int selectedIndex)
    {
        if (previewObject != null)
            Destroy(previewObject);

        int selectedObjectIndex = Mathf.Clamp(selectedIndex, 0, tileObjects.Length - 1);
        GameObject tileObject = tileObjects[selectedObjectIndex];
        previewObject = Instantiate(tileObject, transform.position, Quaternion.Euler(0f, 0f, rotationAngle)); // Set rotation
        previewSpriteRenderer = previewObject.GetComponent<SpriteRenderer>();
        previewSpriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);

        // Get the NetworkIdentity component of the preview object
        NetworkIdentity previewObjectNetworkIdentity = previewObject.GetComponent<NetworkIdentity>();
        if (previewObjectNetworkIdentity != null)
        {
            // Spawn the preview object on the server so that it's visible to all clients
            NetworkServer.Spawn(previewObject);
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

        // Get the object's dimensions based on the colliders
        float objectWidth = cursorCollider.bounds.size.x;
        float objectHeight = cursorCollider.bounds.size.y;

        // Clamp the position to stay within the screen boundaries
        float clampedX = Mathf.Clamp(position.x, -screenHorizontalSize + objectWidth / 2f, screenHorizontalSize - objectWidth / 2f);
        float clampedY = Mathf.Clamp(position.y, -screenVerticalSize + objectHeight / 2f, screenVerticalSize - objectHeight / 2f);
        float clampedZ = transform.position.z;

        transform.position = new Vector3(clampedX, clampedY, clampedZ);
    }

    private void RotatePreviewObject(float scrollInput)
    {
        if (selectedTile != 3)
        {
            rotationAngle += scrollInput * 90f;
            rotationAngle %= 360f;

            previewObject.transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);
        }
    }

    private void OnClick()
    {
        Vector3 cursorPosition = transform.position;
        Vector3Int cellPosition = kingTilemap.WorldToCell(cursorPosition);
        Vector3 tilePosition = kingTilemap.CellToWorld(cellPosition) + kingTilemap.cellSize / 2f;

        if (selectedTile == 0 && !autoTrapPlaced)
        {
            // Check if the placement position is valid (e.g., within certain bounds or not occupied)
            if (IsPlacementValid(cellPosition))
            {
                // Place auto trap
                transform.position = transform.parent.position;
                gameObject.layer = 7;
                autoTrapPlaced = true;
                GameObject placedBlock = Instantiate(autoTrapTileObjects[selectedAutoTrapIndex], tilePosition, Quaternion.identity);
                placedBlock.transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle); // Apply rotation
                NetworkServer.Spawn(placedBlock);
                placedBlock.layer = kingLayerValue;

                // Change the layer of the placed block's children to the "King" layer as well
                foreach (Transform child in placedBlock.transform)
                {
                    child.gameObject.layer = kingLayerValue;
                }

                // Set properties or perform any additional setup for the auto trap
                roundControl.playersPlacedBlocks += 1;
                // Move to the next selected object
                selectedTile = 1;
                // Reset rotation angle to 0
                rotationAngle = 0f;
                CreateAllPreviews();
            }
            else
            {
                Debug.Log("Invalid placement position!");
                return; // Return without destroying the preview object
            }
        }
        else if (selectedTile == 1 && !manualTrapPlaced)
        {
            // Check if the placement position is valid (e.g., within certain bounds or not occupied)
            if (IsPlacementValid(cellPosition))
            {
                // Place manual trap 1
                transform.position = transform.parent.position;
                gameObject.layer = 7;
                manualTrapPlaced = true;
                GameObject placedBlock = Instantiate(manualTrapTileObjects[selectedManualTrapIndex], tilePosition, Quaternion.identity);
                placedBlock.transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle); // Apply rotation
                NetworkServer.Spawn(placedBlock);
                placedBlock.layer = kingLayerValue;

                // Change the layer of the placed block's children to the "King" layer as well
                foreach (Transform child in placedBlock.transform)
                {
                    child.gameObject.layer = kingLayerValue;
                }
                // Set properties or perform any additional setup for the manual trap
                roundControl.playersPlacedBlocks += 1;
                // Move to the next selected object
                selectedTile = 2;
                // Reset rotation angle to 0
                rotationAngle = 0f;
                CreateAllPreviews();
            }
            else
            {
                Debug.Log("Invalid placement position!");
                return; // Return without destroying the preview object
            }
        }
        else if (selectedTile == 2 && !manualTrapPlaced2)
        {
            if (manualTrap2TileObjects.Length > 0) // Check array length before placing manual trap 2
            {
                // Check if the placement position is valid (e.g., within certain bounds or not occupied)
                if (IsPlacementValid(cellPosition))
                {
                    // Place manual trap 2
                    transform.position = transform.parent.position;
                    gameObject.layer = 7;
                    manualTrapPlaced2 = true;
                    GameObject placedBlock = Instantiate(manualTrap2TileObjects[selectedManualTrap2Index], tilePosition, Quaternion.identity);
                    placedBlock.transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle); // Apply rotation
                    NetworkServer.Spawn(placedBlock);
                    placedBlock.layer = kingLayerValue;

                    // Change the layer of the placed block's children to the "King" layer as well
                    foreach (Transform child in placedBlock.transform)
                    {
                        child.gameObject.layer = kingLayerValue;
                    }
                    // Set properties or perform any additional setup for the manual trap 2
                    roundControl.playersPlacedBlocks += 1;
                    selectedTile = 3;
                    // Reset rotation angle to 0
                    rotationAngle = 0f;
                    CreateAllPreviews();
                }
                else
                {
                    Debug.Log("Invalid placement position!");
                    return; // Return without destroying the preview object
                }
            }
        }

        Destroy(previewObject);
    }

    [Command]
    private void CmdPlaceTrap(Vector3 tilePosition, Quaternion rotation)
    {
        // Place the trap on the server
        GameObject placedBlock;
        if (selectedTile == 0 && !autoTrapPlaced)
        {
            autoTrapPlaced = true;
            placedBlock = Instantiate(autoTrapTileObjects[selectedAutoTrapIndex], tilePosition, rotation);
        }
        else if (selectedTile == 1 && !manualTrapPlaced)
        {
            manualTrapPlaced = true;
            placedBlock = Instantiate(manualTrapTileObjects[selectedManualTrapIndex], tilePosition, rotation);
        }
        else if (selectedTile == 2 && !manualTrapPlaced2 && manualTrap2TileObjects.Length > 0)
        {
            manualTrapPlaced2 = true;
            placedBlock = Instantiate(manualTrap2TileObjects[selectedManualTrap2Index], tilePosition, rotation);
        }
        else
        {
            return;
        }

        // Set properties or perform any additional setup for the placed trap
        roundControl.playersPlacedBlocks += 1;

        // Spawn the placed trap on the server so that it's visible to all clients
        NetworkServer.Spawn(placedBlock);
        placedBlock.layer = kingLayerValue;

        // Change the layer of the placed block's children to the "King" layer as well
        foreach (Transform child in placedBlock.transform)
        {
            child.gameObject.layer = kingLayerValue;
        }

        // Move to the next selected object
        selectedTile = (selectedTile + 1) % 4;
        CreateAllPreviews();
    }


    private bool IsPlacementValid(Vector3Int position)
    {
        Vector3 positionWorld = kingTilemap.CellToWorld(position) + kingTilemap.cellSize / 2f;
        Collider2D overlapCollider = Physics2D.OverlapPoint(positionWorld, borderLayer);

        if (overlapCollider != null)
        {
            // Placement is invalid if there is an overlap with the borderLayer
            return false;
        }

        Collider2D[] overlapColliders = Physics2D.OverlapPointAll(positionWorld);

        foreach (Collider2D collider in overlapColliders)
        {
            if (collider.gameObject == gameObject)
                continue;

            if (collider.gameObject.layer == kingLayerValue)
            {
                // Placement is invalid if there is an overlap with the "King" layer
                return false;
            }
        }

        return true;
    }
}
