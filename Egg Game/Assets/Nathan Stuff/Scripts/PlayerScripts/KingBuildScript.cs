using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class KingBuildScript : MonoBehaviour
{
    public Tilemap kingTilemap;
    public GameObject[] autoTrapTileObjects;
    public GameObject[] manualTrapTileObjects;
    public GameObject[] manualTrap2TileObjects;
    public List<GameObject> UITiles;

    private int selectedAutoTrapIndex;
    private int selectedManualTrapIndex;
    private int selectedManualTrap2Index;

    public int selectedTile = 0;
    public int removeTile = 0;
    public bool autoTrapPlaced = false;
    public bool manualTrapPlaced = false;
    public bool manualTrapPlaced2 = false;

    public Transform tileGridUI;

    public PlayerInput playerInput;
    public float moveSpeed = 5f;

    private Vector2 movementInput;
    private GameObject previewObject;
    private SpriteRenderer previewSpriteRenderer;
    private Rigidbody2D rb;

    private Vector3 initialPosition;
    private RoundControl roundControl;
    private int kingLayerValue;
    public LayerMask borderLayer;
    

    private void Awake()
    {
        initialPosition = transform.position;
    }

    private void Start()
    {
        kingLayerValue = LayerMask.NameToLayer("King");
        selectedTile = 0;
        
        playerInput = GetComponentInParent<PlayerInput>();
        kingTilemap = GameObject.Find("KingTilemap").GetComponent<Tilemap>();
        roundControl = GameObject.Find("RoundControl").GetComponent<RoundControl>();

        int i = 0;

        foreach (GameObject tileObject in autoTrapTileObjects)
        {
            GameObject UITile = CreateUITile(tileObject, i == selectedTile);
            UITiles.Add(UITile);

            i++;
        }

        i = 0;

        foreach (GameObject tileObject in manualTrapTileObjects)
        {
            GameObject UITile = CreateUITile(tileObject, i == selectedTile);
            UITiles.Add(UITile);

            i++;
        }

        i = 0;

        foreach (GameObject tileObject in manualTrap2TileObjects)
        {
            GameObject UITile = CreateUITile(tileObject, i == selectedTile);
            UITiles.Add(UITile);

            i++;
        }

        // Randomly select a tile from the manual trap array
        selectedAutoTrapIndex = Random.Range(0, autoTrapTileObjects.Length);
        selectedManualTrapIndex = Random.Range(0, manualTrapTileObjects.Length);
        selectedManualTrap2Index = Random.Range(0, manualTrap2TileObjects.Length);

        // Set the randomly selected tiles as the initial preview objects
        CreatePreviewObject(autoTrapTileObjects, selectedAutoTrapIndex);
        CreatePreviewObject(manualTrapTileObjects, selectedManualTrapIndex);
        CreatePreviewObject(manualTrap2TileObjects, selectedManualTrap2Index);

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // Disable gravity for the cursor

        // Reset the position to the initial position
        transform.localPosition = initialPosition;
    }

    private GameObject CreateUITile(GameObject tileObject, bool isSelected)
    {
        GameObject UITile = new GameObject("UI Tile");
        UITile.transform.parent = tileGridUI;
        UITile.transform.localScale = new Vector3(1f, 1f, 1f);

        UnityEngine.UI.Image UIImage = UITile.AddComponent<UnityEngine.UI.Image>();
        SpriteRenderer spriteRenderer = tileObject.GetComponent<SpriteRenderer>();
        Sprite sprite = spriteRenderer ? spriteRenderer.sprite : null;
        UIImage.sprite = sprite;

        Color tileColor = UIImage.color;
        tileColor.a = isSelected ? 1f : 0.5f;
        UIImage.color = tileColor;

        return UITile;
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
    previewObject = Instantiate(tileObject, transform.position, Quaternion.identity);
    previewSpriteRenderer = previewObject.GetComponent<SpriteRenderer>();
    previewSpriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
}

    private void Update()
    {
        movementInput = playerInput.actions["CursorMove"].ReadValue<Vector2>();

        if (movementInput != Vector2.zero)
        {
            MoveCursor();
        }

        // Create the preview object based on the selected tile
        CreateAllPreviews();

        if (playerInput.actions["CursorClick"].triggered)
        {
            OnClick();
        }
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
            RenderUITiles();
            GameObject placedBlock = Instantiate(autoTrapTileObjects[selectedAutoTrapIndex], tilePosition, Quaternion.identity);
            // Set the layer of the placed block to the "King" layer
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
            CreateAllPreviews();
        }else{
            Debug.Log("Invalid placement position!");
            return; // Return without destroying the preview object
        }
        
    }
    else if (selectedTile == 1 && !manualTrapPlaced)
    {
        // Check if the placement position is valid (e.g., within certain bounds or not occupied)
        if (IsPlacementValid(cellPosition)){
            // Place manual trap 1
            transform.position = transform.parent.position;
            gameObject.layer = 7;
            manualTrapPlaced = true;
            RenderUITiles();
            GameObject placedBlock = Instantiate(manualTrapTileObjects[selectedManualTrapIndex], tilePosition, Quaternion.identity);
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
            CreateAllPreviews();
        }else{
            Debug.Log("Invalid placement position!");
            return; // Return without destroying the preview object
        }
        
    }
    else if (selectedTile == 2 && !manualTrapPlaced2)
    {
        if (manualTrap2TileObjects.Length > 0)  // Check array length before placing manual trap 2
        {
            // Check if the placement position is valid (e.g., within certain bounds or not occupied)
            if (IsPlacementValid(cellPosition)){
                // Place manual trap 2
                transform.position = transform.parent.position;
                gameObject.layer = 7;
                manualTrapPlaced2 = true;
                RenderUITiles();
                GameObject placedBlock = Instantiate(manualTrap2TileObjects[selectedManualTrap2Index], tilePosition, Quaternion.identity);
                placedBlock.layer = kingLayerValue;

                // Change the layer of the placed block's children to the "King" layer as well
                foreach (Transform child in placedBlock.transform)
                {
                    child.gameObject.layer = kingLayerValue;
                }
                // Set properties or perform any additional setup for the manual trap 2
                roundControl.playersPlacedBlocks += 1;
                selectedTile = 3;
                CreateAllPreviews();
            }else{
                Debug.Log("Invalid placement position!");
                return; // Return without destroying the preview object
            }
        }
        
    }

    Destroy(previewObject);
}

    private void RenderUITiles()
    {
        for (int i = 0; i < UITiles.Count; i++)
        {
            GameObject UITile = UITiles[i];
            UnityEngine.UI.Image UIImage = UITile.GetComponent<UnityEngine.UI.Image>();
            Color tileColor = UIImage.color;
            tileColor.a = (i == selectedTile) ? 1f : 0.5f;
            UIImage.color = tileColor;
        }
    }

private int previousAutoTrapIndex;
private int previousManualTrapIndex;
private int previousManualTrap2Index;

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

    RenderUITiles();
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

        // Perform a point overlap check with the collider
        Collider2D overlapCollider = Physics2D.OverlapPoint(positionWorld, borderLayer);

        // Check if there is no overlap or if the overlap is with the current game object
        if (overlapCollider == null || overlapCollider.gameObject == gameObject)
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
}