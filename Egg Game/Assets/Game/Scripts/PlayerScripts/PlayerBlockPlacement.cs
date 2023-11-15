using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Mirror;

public class PlayerBlockPlacement : NetworkBehaviour
{
    // Variables
    public Tilemap kingTilemap;
    public GameObject[] blockTileObjects;

    public GameObject[] autoTrapTileObjects;
    public GameObject[] manualTrapTileObjects;
    public GameObject[] manualTrap2TileObjects;

    public Transform tileGridUI;

    private Collider2D cursorCollider;
    private int kingLayerValue;
    private LayerMask borderLayer;
    [SerializeField] private RoundControl roundControl;
    private Vector2 movementInput;

    [SerializeField] private Player playerScript;

    [SerializeField][SyncVar(hook = nameof(OnSelectedTileChanged))]
    private int selectedTile = 0;


    [SerializeField][SyncVar(hook = nameof(OnSelectedBlockIndexChanged))]
    private int selectedBlockIndex;


    [SerializeField][SyncVar(hook = nameof(OnSelectedAutoTrapIndexChanged))]
    private int selectedAutoTrapIndex;
    
    [SerializeField][SyncVar(hook = nameof(OnSelectedManualTrapIndexChanged))]
    private int selectedManualTrapIndex;
    
    [SerializeField][SyncVar(hook = nameof(OnSelectedManualTrap2IndexChanged))]
    private int selectedManualTrap2Index;


    [SyncVar(hook = nameof(OnPreviewOpacityChanged))]
    private float previewOpacity = 0.5f;

    [SyncVar(hook = nameof(OnRotationAngleChanged))]
    private float rotationAngle = 0f;

    private int previousBlockIndex;
    private int previousAutoTrapIndex;
    private int previousManualTrapIndex;
    private int previousManualTrap2Index;

    private bool blockPlaced = false;
    private bool autoTrapPlaced = false;
    private bool manualTrapPlaced = false;
    private bool manualTrapPlaced2 = false;

    private GameObject previewObject;

    private SpriteRenderer previewSpriteRenderer;
    private Vector3 initialPosition;
    private bool isGameFocused = true;
    public GameObject boundingObject;

    private bool roundControlFound = false;

    private void Awake()
    {
        cursorCollider = GetComponent<Collider2D>();
        initialPosition = transform.position;
    }

    private void Start()
    {
        boundingObject = GameObject.Find("MapArea");
        kingLayerValue = LayerMask.NameToLayer("King");
        selectedTile = 0;
        kingTilemap = GameObject.Find("KingTilemap").GetComponent<Tilemap>();

        Application.focusChanged += OnApplicationFocus;

        if (isOwned)
        {
            CmdInitializeSelectedIndexes();
            CmdInitializeSelectedKingIndexes();
        }
    }

    private void OnEnable()
    {
        Debug.Log("Script has been enabled.");

        selectedTile = 0;
        playerScript = gameObject.transform.parent.GetComponent<Player>();
        
        if(playerScript.isKing){
            autoTrapPlaced = false;
            manualTrapPlaced = false;
            manualTrapPlaced2 = false;

            do
            {
                selectedAutoTrapIndex = Random.Range(0, autoTrapTileObjects.Length);
            } while (selectedAutoTrapIndex == previousAutoTrapIndex);

            Debug.Log("After selection: " + selectedAutoTrapIndex + ", " + selectedManualTrapIndex + ", " + selectedManualTrap2Index);

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

            // Use a command to sync the selected indexes with the server
            if (isOwned)
            {
                CmdSyncKingSelectedIndexes(selectedAutoTrapIndex, selectedManualTrapIndex, selectedManualTrap2Index);
            }
        }else if(playerScript.isPlayer && !playerScript.isKing){
            blockPlaced = false;

            do
            {
                selectedBlockIndex = Random.Range(0, blockTileObjects.Length);
            } while (selectedBlockIndex == previousBlockIndex);

            
            previousBlockIndex = selectedBlockIndex;
            
            if (isOwned)
            {
                CmdSyncSelectedIndexes(selectedBlockIndex);
            }
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from the application focus event
        Application.focusChanged -= OnApplicationFocus;

        if (previewObject != null)
        {
            DestroyPreviewObject();
        }
    }

    IEnumerator WaitForRoundControl() {
        while (true) {
            GameObject roundControlObject = GameObject.Find("RoundControl(Clone)");

            if (roundControlObject != null) {
                roundControl = roundControlObject.GetComponent<RoundControl>();
                Debug.Log("RoundControl found!");
                roundControlFound = true;
                break;
            }

            yield return null; // Wait for a frame before checking again
        }
    }

    private void Update()
    {

        if(!roundControlFound){
            Debug.Log("Looking for RoundControl for placement");
            StartCoroutine(WaitForRoundControl());
        }

        if (!isGameFocused) return; // Only process input if the game is focused

        if (!isLocalPlayer) return;

        // Process movement input based on the mouse position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 newPosition = new Vector3(mousePosition.x, mousePosition.y, 0);

        // Limit the cursor's movement within the bounds of the boundingObject
        Vector3 clampedPosition = LimitPositionWithinBounds(newPosition);

        // Update the cursor's position
        transform.position = clampedPosition;
        
        // Check for scroll wheel input to rotate the trap preview
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0f)
        {
            RotatePreviewObject(scrollInput);
        }

        CmdMoveCursor(transform.position);

        CmdCreateAllPreviews();

        // Check for mouse click to place the trap
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 cursorPosition = transform.position;
            Vector3Int cellPosition = kingTilemap.WorldToCell(cursorPosition);
            Vector3 tilePosition = kingTilemap.CellToWorld(cellPosition) + kingTilemap.cellSize / 2f;
            CmdPlaceBlock(tilePosition, Quaternion.Euler(0f, 0f, rotationAngle));
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

    [Command]
    private void CmdCreateAllPreviews()
    {
        if(playerScript.isPlayer){
            if (!blockPlaced && selectedTile == 0)
            {
                CreatePreviewObject(blockTileObjects, selectedBlockIndex);

                // Manually synchronize the preview opacity on the server and all clients
                RpcUpdatePreviewOpacity(previewObject, previewOpacity);
            }
            else
            {
                DestroyPreviewObject();
            }
        }

        if(playerScript.isKing){
            if (!autoTrapPlaced && selectedTile == 0)
            {
                CreatePreviewObject(autoTrapTileObjects, selectedAutoTrapIndex);

                // Manually synchronize the preview opacity on the server and all clients
                RpcUpdatePreviewOpacity(previewObject, previewOpacity);
            }
            else if (!manualTrapPlaced && selectedTile == 1)
            {
                CreatePreviewObject(manualTrapTileObjects, selectedManualTrapIndex);

                // Manually synchronize the preview opacity on the server and all clients
                RpcUpdatePreviewOpacity(previewObject, previewOpacity);
            }
            else if (!manualTrapPlaced2 && selectedTile == 2 && manualTrap2TileObjects.Length > 0)
            {
                CreatePreviewObject(manualTrap2TileObjects, selectedManualTrap2Index);

                // Manually synchronize the preview opacity on the server and all clients
                RpcUpdatePreviewOpacity(previewObject, previewOpacity);
            }
            else
            {
                DestroyPreviewObject();
            }
        }

    }

    [Command]
    private void CmdMoveCursor(Vector3 position)
    {
        // Update the cursor position on the server
        transform.position = position;
    }

    [Command]
    private void CmdPlaceBlock(Vector3 tilePosition, Quaternion rotation)
    {
        // Check if the placement position is valid (e.g., within certain bounds or not occupied)
        Vector3Int cellPosition = kingTilemap.WorldToCell(tilePosition);
        if (!IsPlacementValid(cellPosition))
        {
            Debug.Log("Invalid placement position!");
            return;
        }

        // Place the block on the server
        GameObject placedBlock = null;

        if(playerScript.isPlayer){
            if (selectedTile == 0 && !blockPlaced)
            {
                blockPlaced = true;
                placedBlock = Instantiate(blockTileObjects[selectedBlockIndex], tilePosition, rotation);
                roundControl.playersPlacedBlocks += 1;
            }
            else
            {
                return;
            }
        }

        if(playerScript.isKing){
            if (selectedTile == 0 && !autoTrapPlaced)
            {
                autoTrapPlaced = true;
                placedBlock = Instantiate(autoTrapTileObjects[selectedAutoTrapIndex], tilePosition, rotation);
                roundControl.playersPlacedBlocks +=1;
            }
            else if (selectedTile == 1 && !manualTrapPlaced)
            {
                manualTrapPlaced = true;
                placedBlock = Instantiate(manualTrapTileObjects[selectedManualTrapIndex], tilePosition, rotation);
                roundControl.playersPlacedBlocks +=1;
            }
            else if (selectedTile == 2 && !manualTrapPlaced2)
            {
                manualTrapPlaced2 = true;
                placedBlock = Instantiate(manualTrap2TileObjects[selectedManualTrap2Index], tilePosition, rotation);
                roundControl.playersPlacedBlocks +=1;
            }
            else
            {
                return;
            }
        }


        // Add NetworkIdentity component to the placed block object
        var networkIdentity = placedBlock.GetComponent<NetworkIdentity>();
        if (networkIdentity == null)
        {
            networkIdentity = placedBlock.AddComponent<NetworkIdentity>();
        }

        // Spawn the placed block on the server so that it's visible to all clients
        NetworkServer.Spawn(placedBlock);
        placedBlock.layer = kingLayerValue;

        // Change the layer of the placed block's children to the "King" layer as well
        foreach (Transform child in placedBlock.transform)
        {
            child.gameObject.layer = kingLayerValue;
        }

        RpcChangeBlockLayer(placedBlock, kingLayerValue);

        // Move to the next selected object
        selectedTile = (selectedTile + 1) % 4;
        RpcChangeSelectedTile(selectedTile);
    }

    [ClientRpc]
    private void RpcChangeSelectedTile(int newTile)
    {
        // Update the selectedTile on all clients
        selectedTile = newTile;
    }

    [ClientRpc]
    private void RpcChangeBlockLayer(GameObject blockObject, int newLayer)
    {
        // Change the layer of the placed block and its children on the client side
        blockObject.layer = newLayer;
        foreach (Transform child in blockObject.transform)
        {
            child.gameObject.layer = newLayer;
        }
    }

    [Command]
    private void CmdInitializeSelectedIndexes()
    {
        // Initialize the selected indexes on the server for this player
        selectedBlockIndex = Random.Range(0, blockTileObjects.Length);
    }

    [Command]
    private void CmdSyncSelectedIndexes(int blockIndex)
    {
        // Sync the selected indexes with the server
        selectedBlockIndex = blockIndex;
    }

    [Command]
    private void CmdInitializeSelectedKingIndexes()
    {
        // Initialize the selected indexes on the server for this player
        selectedAutoTrapIndex = Random.Range(0, autoTrapTileObjects.Length);
        selectedManualTrapIndex = Random.Range(0, manualTrapTileObjects.Length);
        selectedManualTrap2Index = Random.Range(0, manualTrap2TileObjects.Length);
    }

    [Command]
    private void CmdSyncKingSelectedIndexes(int autoTrapIndex, int manualTrapIndex, int manualTrap2Index)
    {
        // Sync the selected indexes with the server
        selectedAutoTrapIndex = autoTrapIndex;
        selectedManualTrapIndex = manualTrapIndex;
        selectedManualTrap2Index = manualTrap2Index;
    }

    private void ResetMovementInput()
    {
        // Reset the movement input
        movementInput = Vector2.zero;
    }

    private void CreatePreviewObject(GameObject[] tileObjects, int selectedIndex)
    {
        if (previewObject != null)
            DestroyPreviewObject();

        int selectedObjectIndex = Mathf.Clamp(selectedIndex, 0, tileObjects.Length - 1);
        GameObject tileObject = tileObjects[selectedObjectIndex];
        previewObject = Instantiate(tileObject, transform.position, Quaternion.Euler(0f, 0f, rotationAngle)); // Set rotation

        // Set the initial opacity
        OnPreviewOpacityChanged(0f, previewOpacity);

        // Add NetworkIdentity component to the preview object
        var networkIdentity = previewObject.GetComponent<NetworkIdentity>();
        if (networkIdentity == null)
        {
            networkIdentity = previewObject.AddComponent<NetworkIdentity>();
        }

        // Network-instantiate the preview object
        NetworkServer.Spawn(previewObject);
    }

    private void DestroyPreviewObject()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);

            // Unspawn the preview object on the server
            NetworkServer.UnSpawn(previewObject);

            // Set previewObject to null
            previewObject = null;
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

    private void RotatePreviewObject(float scrollInput)
    {
        if (selectedTile != 3)
        {
            rotationAngle += scrollInput * 90f;
            rotationAngle %= 360f;

            if (previewObject != null)
                previewObject.transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);
        }
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

    private void OnSelectedTileChanged(int oldValue, int newValue)
    {
        // When the selectedTile changes on the server, update it on the clients
        selectedTile = newValue;
    }

    private void OnSelectedBlockIndexChanged(int oldValue, int newValue)
    {
        selectedBlockIndex = newValue;
    }

    private void OnSelectedAutoTrapIndexChanged(int oldValue, int newValue)
    {
        selectedAutoTrapIndex = newValue;
    }

    private void OnSelectedManualTrapIndexChanged(int oldValue, int newValue)
    {
        selectedManualTrapIndex = newValue;
    }

    private void OnSelectedManualTrap2IndexChanged(int oldValue, int newValue)
    {
        selectedManualTrap2Index = newValue;
    }

    private void OnPreviewOpacityChanged(float oldValue, float newValue)
    {
        // When the previewOpacity changes on the server, update it on the clients
        previewOpacity = newValue;

        if (previewObject != null)
        {
            // Update the opacity of all children's SpriteRenderers in the preview object
            SpriteRenderer[] childSpriteRenderers = previewObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer spriteRenderer in childSpriteRenderers)
            {
                Color newColor = spriteRenderer.color;
                newColor.a = previewOpacity;
                spriteRenderer.color = newColor;
            }
        }
    }

    [ClientRpc]
    private void RpcUpdatePreviewOpacity(GameObject previewObject, float newOpacity)
    {
        if (previewObject != null)
        {
            // Update the opacity of all children's SpriteRenderers in the preview object on the client-side
            SpriteRenderer[] childSpriteRenderers = previewObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer spriteRenderer in childSpriteRenderers)
            {
                Color newColor = spriteRenderer.color;
                newColor.a = newOpacity;
                spriteRenderer.color = newColor;
            }
        }
    }

    private void OnRotationAngleChanged(float oldValue, float newValue)
    {
        // Apply the synchronized rotation angle to the preview object on the client
        rotationAngle = newValue;
        if (previewObject != null)
            previewObject.transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);
    }
}
