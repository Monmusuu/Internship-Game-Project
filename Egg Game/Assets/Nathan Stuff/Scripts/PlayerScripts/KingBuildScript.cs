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
    

    private void Awake()
    {
        initialPosition = transform.position;
    }

    private void Start()
    {

        selectedTile = 0;
        
        playerInput = GetComponentInParent<PlayerInput>();
        kingTilemap = GameObject.Find("KingTilemap").GetComponent<Tilemap>();

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
        CreatePreviewObject(autoTrapTileObjects, selectedTile == 0);
        CreatePreviewObject(manualTrapTileObjects, selectedTile == 1);
        CreatePreviewObject(manualTrap2TileObjects, selectedTile == 2);

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // Disable gravity for the cursor
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
        CreatePreviewObject(autoTrapTileObjects, true);
    else if (!manualTrapPlaced && selectedTile == 1)
        CreatePreviewObject(manualTrapTileObjects, true);
    else if (!manualTrapPlaced2 && selectedTile == 2) // Add check for valid index
        CreatePreviewObject(manualTrap2TileObjects, true);
    else
        Destroy(previewObject);
}

    private void CreatePreviewObject(GameObject[] tileObjects, bool isSelected)
    {
        if (previewObject != null)
            Destroy(previewObject);

        GameObject tileObject = tileObjects[selectedTile];
        previewObject = Instantiate(tileObject, transform.position, Quaternion.identity);
        previewSpriteRenderer = previewObject.GetComponent<SpriteRenderer>();
        previewSpriteRenderer.color = new Color(1f, 1f, 1f, isSelected ? 1f : 0.5f);
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
        // Place auto trap
        transform.position = initialPosition;
        gameObject.layer = 7;
        autoTrapPlaced = true;
        RenderUITiles();
        GameObject placedBlock = Instantiate(autoTrapTileObjects[selectedAutoTrapIndex], tilePosition, Quaternion.identity);
        // Set properties or perform any additional setup for the auto trap

        // Move to the next selected object
        selectedTile = 1;
        CreateAllPreviews();
    }
    else if (selectedTile == 1 && !manualTrapPlaced)
    {
        // Place manual trap 1
        transform.position = initialPosition;
        gameObject.layer = 7;
        manualTrapPlaced = true;
        RenderUITiles();
        GameObject placedBlock = Instantiate(manualTrapTileObjects[selectedManualTrapIndex], tilePosition, Quaternion.identity);
        // Set properties or perform any additional setup for the manual trap

        // Move to the next selected object
        selectedTile = 2;
        CreateAllPreviews();
    }
    else if (selectedTile == 2 && !manualTrapPlaced2)
    {
        if (manualTrap2TileObjects.Length > 0)  // Check array length before placing manual trap 2
        {
            // Place manual trap 2
            transform.position = initialPosition;
            gameObject.layer = 7;
            manualTrapPlaced2 = true;
            RenderUITiles();
            GameObject placedBlock = Instantiate(manualTrap2TileObjects[selectedManualTrap2Index], tilePosition, Quaternion.identity);
            // Set properties or perform any additional setup for the manual trap 2

            selectedTile = 3;
            CreateAllPreviews();
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

    private void OnEnable()
    {
        if (previewObject != null)
        {
            previewObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (previewObject != null)
        {
            previewObject.SetActive(false);
        }
    }
}