
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayerBlockPlacement : MonoBehaviour
{
public Tilemap kingTilemap;
public GameObject[] blockTileObjects;
public List<GameObject> UITiles;
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

private void Awake()
{
    initialPosition = transform.position;
}

private void Start()
{
    roundControl = GameObject.Find("RoundControl").GetComponent<RoundControl>();
    playerInput = GetComponentInParent<PlayerInput>();
    kingTilemap = GameObject.Find("KingTilemap").GetComponent<Tilemap>();

    int i = 0;

    foreach (GameObject tileObject in blockTileObjects)
    {
        GameObject UITile = new GameObject("UI Tile");
        UITile.transform.parent = tileGridUI;
        UITile.transform.localScale = new Vector3(1f, 1f, 1f);

        UnityEngine.UI.Image UIImage = UITile.AddComponent<UnityEngine.UI.Image>();
        SpriteRenderer spriteRenderer = tileObject.GetComponent<SpriteRenderer>();
        Sprite sprite = spriteRenderer ? spriteRenderer.sprite : null;
        UIImage.sprite = sprite;

        Color tileColor = UIImage.color;
        tileColor.a = 0.5f;

        if (i == selectedTile)
        {
            tileColor.a = 1f;
        }

        UIImage.color = tileColor;
        UITiles.Add(UITile);

        i++;
    }

    int randomIndex = Random.Range(0, blockTileObjects.Length); // Randomly select an index
    previewObject = Instantiate(blockTileObjects[randomIndex], transform.position, Quaternion.identity);
    previewSpriteRenderer = previewObject.GetComponent<SpriteRenderer>();
    previewSpriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);

    rb = GetComponent<Rigidbody2D>();
    rb.gravityScale = 0f; // Disable gravity for the cursor
}

private void Update()
{
    movementInput = playerInput.actions["CursorMove"].ReadValue<Vector2>();

    if (movementInput != Vector2.zero)
    {
        MoveCursor();
    }

    selectedTile = 0;
    RenderUITiles();

    if (blockPlaced == true)
    {
        selectedTile = 3;
    }

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

public void OnClick()
{
    Vector3 cursorPosition = transform.position;
    Vector3Int cellPosition = kingTilemap.WorldToCell(cursorPosition);
    Vector3 tilePosition = kingTilemap.CellToWorld(cellPosition) + kingTilemap.cellSize / 2f;

    if (selectedTile == 0 && !blockPlaced)
    {
        transform.position = initialPosition;
        gameObject.layer = 7;
        blockPlaced = true;
        RenderUITiles();
        GameObject placedBlock = Instantiate(previewObject, tilePosition, Quaternion.identity);
        SpriteRenderer placedSpriteRenderer = placedBlock.GetComponent<SpriteRenderer>();
        Color blockColor = placedSpriteRenderer.color;
        blockColor.a = 1f; // Set the alpha value to 0 (fully transparent)
        placedSpriteRenderer.color = blockColor;
        roundControl.playersPlacedBlocks += 1;
    }

    Destroy(previewObject);
}

void RenderUITiles()
{
    int i = 0;
    foreach (GameObject tileObject in UITiles)
    {
        UnityEngine.UI.Image UIImage = tileObject.GetComponent<UnityEngine.UI.Image>();
        Color tileColor = UIImage.color;
        tileColor.a = 0.5f;

        if (i == selectedTile)
        {
            tileColor.a = 1f;
        }

        UIImage.color = tileColor;

        i++;
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