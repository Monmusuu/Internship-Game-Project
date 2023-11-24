using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Mirror;
using System.Linq;

public class PlayerBlockPlacement : NetworkBehaviour
{
    // Variables
    public Tilemap kingTilemap;
    public GameObject[] blockTileObjects;

    public GameObject[] autoTrapTileObjects;
    public GameObject[] manualTrapTileObjects;
    public GameObject[] manualTrap2TileObjects;

    public GameObject teleporterReciever;
    public GameObject boundingObject;
    private GameObject previewObject;
    public GameObject trapHolder;
    public GameObject blockHolder;

    [SerializeField] private RoundControl roundControl;

    private SpriteRenderer previewSpriteRenderer;

    private Vector2 initialPosition;
    private Vector2 movementInput;

    private Collider2D cursorCollider;
    private int kingLayerValue;

    [SerializeField] private Player playerScript;

    [SerializeField][SyncVar(hook = nameof(OnSelectedTileChanged))]
    private int selectedTile = 0;
    [SerializeField][SyncVar(hook = nameof(OnSelectedBlockIndexChanged))]
    private int selectedBlockIndex = -2;
    [SerializeField][SyncVar(hook = nameof(OnSelectedAutoTrapIndexChanged))]
    private int selectedAutoTrapIndex = -2;
    [SerializeField][SyncVar(hook = nameof(OnSelectedManualTrapIndexChanged))]
    private int selectedManualTrapIndex = -2;
    [SerializeField][SyncVar(hook = nameof(OnSelectedManualTrap2IndexChanged))]
    private int selectedManualTrap2Index = -2;

    [SerializeField][SyncVar]
    private int previousBlockIndex = -1;
    [SerializeField][SyncVar]
    private int previousAutoTrapIndex = -1;
    [SerializeField][SyncVar]
    private int previousManualTrapIndex = -1;
    [SerializeField][SyncVar]
    private int previousManualTrap2Index = -1;
    private int teleporterNumber = 0;


    [SyncVar(hook = nameof(OnPreviewOpacityChanged))]
    private float previewOpacity = 0.5f;

    [SyncVar(hook = nameof(OnRotationAngleChanged))]
    private float rotationAngle = 0f;

    [SerializeField][SyncVar]
    private bool blockPlaced = false;
    [SerializeField][SyncVar]
    private bool allPlayerBlocksPlaced = false;
    [SerializeField][SyncVar]
    private bool autoTrapPlaced = false;
    [SerializeField][SyncVar]
    private bool manualTrapPlaced = false;
    [SerializeField][SyncVar]
    private bool manualTrapPlaced2 = false;
    [SerializeField][SyncVar]
    private bool allKingBlocksPlaced = false;
    private bool isGameFocused = true;

    [SerializeField][SyncVar]
    private bool initializeSelectedIndexes = false;

    // Place the block on the server
    GameObject placedBlock = null;

    private void Awake()
    {
        cursorCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        StartCoroutine(WaitForRoundControl());
        boundingObject = GameObject.Find("MapArea");
        kingLayerValue = LayerMask.NameToLayer("King");
        selectedTile = 0;
        kingTilemap = GameObject.Find("KingTilemap").GetComponent<Tilemap>();

        Application.focusChanged += OnApplicationFocus;

        playerScript = gameObject.transform.parent.GetComponent<Player>();
        trapHolder = GameObject.Find("TrapHolder");
        blockHolder = GameObject.Find("BlockHolder");
    }

    private void OnEnable()
    {
        Debug.Log("Script has been enabled.");

        placedBlock = null;
        
        if(isOwned){
            CmdInitializeSelectedIndexes();
            CmdInitializeSelectedKingIndexes();
        }

        if(isServer)
        {
            autoTrapPlaced = false;
            manualTrapPlaced = false;
            manualTrapPlaced2 = false;
            allKingBlocksPlaced = false;
            blockPlaced = false;
            allPlayerBlocksPlaced = false;

            selectedTile = 0;
            
            StartCoroutine(InitializeSelectedIndexes());
        }
    }

    private void Update() {
        if (!isGameFocused) return; // Only process input if the game is focused

        if(!initializeSelectedIndexes) return;
        
        if(selectedTile == 3 && allKingBlocksPlaced) return;

        if(selectedBlockIndex != 9 && selectedTile == 1 && allPlayerBlocksPlaced) return;

        if(selectedBlockIndex == 9 && selectedTile == 2 && allPlayerBlocksPlaced) return;

        if (isLocalPlayer){

            // Process movement input based on the mouse position
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 newPosition = new Vector3(mousePosition.x, mousePosition.y, 0);

            // Limit the cursor's movement within the bounds of the boundingObject
            Vector3 clampedPosition = LimitPositionWithinBounds(newPosition);

            // Update the cursor's position
            transform.position = clampedPosition;
            
            CmdCreateAllPreviews();
     
            // Check for scroll wheel input to rotate the trap preview
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            if (scrollInput != 0f)
            {
                CheckRotationInput();
            }

            if (Input.GetMouseButtonDown(0))
            {

                if(playerScript.isPlayer){
                    Debug.Log("Placing Block");
                    Vector3 cursorPosition = transform.position;
                    Vector3Int cellPosition = kingTilemap.WorldToCell(cursorPosition);
                    Vector3 tilePosition = kingTilemap.CellToWorld(cellPosition) + kingTilemap.cellSize / 2f;
                    CmdPlaceBlock(tilePosition, Quaternion.Euler(0f, 0f, rotationAngle));
                }else if(playerScript.isKing){
                    Debug.Log("Placing Trap");
                    Vector3 cursorPosition = transform.position;
                    Vector3Int cellPosition = kingTilemap.WorldToCell(cursorPosition);
                    Vector3 tilePosition = kingTilemap.CellToWorld(cellPosition) + kingTilemap.cellSize / 2f;
                    CmdPlaceBlock(tilePosition, Quaternion.Euler(0f, 0f, rotationAngle));
                }
            }
        }
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

        if(playerScript.isPlayer){

            if(selectedBlockIndex == 9){

                if (selectedTile == 0 && !blockPlaced)
                {
                    blockPlaced = true;
                    placedBlock = Instantiate(blockTileObjects[selectedBlockIndex], tilePosition, rotation);
                    
                }else if(selectedTile == 1)
                {
                    placedBlock = Instantiate(teleporterReciever, tilePosition, rotation);
                    roundControl.playersPlacedBlocks += 1;
                    allPlayerBlocksPlaced = true;
                }   
            }else{
                if (selectedTile == 0 && !blockPlaced)
                {
                    blockPlaced = true;
                    placedBlock = Instantiate(blockTileObjects[selectedBlockIndex], tilePosition, rotation);
                    roundControl.playersPlacedBlocks += 1;
                    allPlayerBlocksPlaced = true;
                }
                else
                {
                    return;
                }
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
                allKingBlocksPlaced = true;
            }
            else
            {
                return;
            }
        }
        
        //Spawn the placed block on the server so that it's visible to all clients
        NetworkServer.Spawn(placedBlock);

        placedBlock.layer = kingLayerValue;

        // Change the layer of the placed block's children to the "King" layer as well
        foreach (Transform child in placedBlock.transform)
        {
            child.gameObject.layer = kingLayerValue;
        }

        RpcChangeBlockLayer(placedBlock, kingLayerValue);

        // Set the parent based on the selected tile on all clients
        RpcSetParent(placedBlock, playerScript.isPlayer);

        if(selectedBlockIndex == 9 && selectedTile ==0 && playerScript.isPlayer){
            Teleporter teleporter = placedBlock.GetComponent<Teleporter>();
            if(teleporter != null){
                teleporter.teleporterNumber = GenerateUniqueTeleporterNumber();
                teleporterNumber = teleporter.teleporterNumber;

                // Debug statement to mark progress
                Debug.Log("Teleporter placed. Teleporter Number: " + teleporterNumber);

                // Debug statement to print the name of the teleporter object
                Debug.Log("Teleporter Object Name: " + placedBlock.name);

            }else{
                Debug.Log("Null Reference");
            }
        }else if (selectedBlockIndex == 9 && selectedTile == 1 && playerScript.isPlayer){
            // Set the teleporter number on the receiver
            TeleporterReceiver receiver = placedBlock.GetComponent<TeleporterReceiver>();
            if (receiver != null)
            {
                receiver.receiverNumber = teleporterNumber;

                // Debug statement to mark progress
                Debug.Log("Teleporter Receiver placed. Receiver Number: " + receiver.receiverNumber);

            }else{
                Debug.Log("Null Reference"); 
            }
        }

        // Move to the next selected object
        selectedTile = (selectedTile + 1) % 4;
    }
    

    [ClientRpc]
    void RpcSetParent(GameObject block, bool isPlayer)
    {
        if (isPlayer)
        {
            block.transform.SetParent(blockHolder.transform);
        }
        else
        {
            block.transform.SetParent(trapHolder.transform);
        }
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
    private void CmdCreateAllPreviews()
    {
        if(playerScript.isPlayer){
            if (!blockPlaced && selectedTile == 0)
            {
                CreatePreviewObject(blockTileObjects, selectedBlockIndex);

                // Manually synchronize the preview opacity on the server and all clients
                RpcUpdatePreviewOpacity(previewObject, previewOpacity);
            }else if (selectedBlockIndex == 9 && selectedTile == 1){

                CreateReceiverPreviewObject(teleporterReciever);

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

    private void CreatePreviewObject(GameObject[] tileObjects, int selectedIndex)
    {
        if (previewObject != null)
            DestroyPreviewObject();

        int selectedObjectIndex = Mathf.Clamp(selectedIndex, 0, tileObjects.Length - 1);
        GameObject tileObject = tileObjects[selectedObjectIndex];
        previewObject = Instantiate(tileObject, transform.position, Quaternion.Euler(0f, 0f, rotationAngle)); // Set rotation

        // Set the initial opacity
        OnPreviewOpacityChanged(0f, previewOpacity);

        // Network-instantiate the preview object
        NetworkServer.Spawn(previewObject);
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

    private void CreateReceiverPreviewObject(GameObject receiverObject)
    {
        if (previewObject != null)
            DestroyPreviewObject();

        previewObject = Instantiate(receiverObject, transform.position, Quaternion.Euler(0f, 0f, rotationAngle));

        // Set the initial opacity
        OnPreviewOpacityChanged(0f, previewOpacity);

        // Network-instantiate the preview object
        NetworkServer.Spawn(previewObject);
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

    private void CheckRotationInput()
    {
        float rotationInput = Input.GetAxis("Mouse ScrollWheel");

        if (rotationInput != 0f)
        {
            // Call a method to handle rotation on the client side
            CmdRotatePreviewObject(rotationInput);
        }
    }

    [Command]
    private void CmdRotatePreviewObject(float rotationInput)
    {
        // Call a method to handle rotation on the server side
        RpcRotatePreviewObject(rotationInput);
    }

    [ClientRpc]
    private void RpcRotatePreviewObject(float rotationInput)
    {
        // Handle rotation on all clients
        if (previewObject != null)
        {
            // Update the rotation angle based on input
            rotationAngle += rotationInput * 90f;
            rotationAngle %= 360f;

            // Set the rotation of the preview object
            previewObject.transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);

            Debug.Log("Preview Object Rotated to: " + rotationAngle);
        }
    }

    private bool IsPlacementValid(Vector3Int position)
    {
        // Calculate the world space position of the placement
        Vector3 positionWorld = kingTilemap.GetCellCenterWorld(position);

        // Calculate the bounds of the preview object
        Bounds previewBounds = previewObject.GetComponent<Collider2D>().bounds;

        // Check for overlaps with the border layer
        Collider2D[] overlapCollidersBorder = Physics2D.OverlapBoxAll(positionWorld, previewBounds.size, 0f, LayerMask.GetMask("Border"));

        // Check for overlaps with the king layer
        Collider2D[] overlapCollidersKing = Physics2D.OverlapBoxAll(positionWorld, previewBounds.size, 0f, LayerMask.GetMask("King"));

        // If there are overlaps with either layer, the placement is invalid
        if (overlapCollidersBorder.Length > 0 || overlapCollidersKing.Length > 0)
        {
            return false;
        }

        return true;
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

    // Add this method to your script
    private int GenerateUniqueTeleporterNumber()
    {
        // Get all existing teleporters in the scene
        Teleporter[] teleporters = GameObject.FindObjectsOfType<Teleporter>();

        // Generate a new teleporter number until a unique one is found
        int newTeleporterNumber;
        do
        {
            newTeleporterNumber = Random.Range(1, 1000); // Adjust the range as needed
            // Debug statement to check if the generated number is not unique
            if (teleporters.Any(teleporter => teleporter.teleporterNumber == newTeleporterNumber))
            {
                Debug.Log("Generated teleporter number is not unique: " + newTeleporterNumber);
            }

        } while (teleporters.Any(teleporter => teleporter.teleporterNumber == newTeleporterNumber));

        return newTeleporterNumber;
    }

    private IEnumerator InitializeSelectedIndexes()
    {
        // Wait for synchronization to occur
        yield return new WaitForSeconds(0.5f); // Adjust the delay as needed

        do
        {
            selectedAutoTrapIndex = Random.Range(0, autoTrapTileObjects.Length);
        } while (selectedAutoTrapIndex == previousAutoTrapIndex);

        Debug.Log("After selection: " + selectedAutoTrapIndex + ", " + selectedManualTrapIndex + ", " + selectedManualTrap2Index);

        do
        {
            selectedManualTrapIndex = Random.Range(0, manualTrapTileObjects.Length);
        } while (selectedManualTrapIndex == previousManualTrapIndex);

        do
        {
            selectedManualTrap2Index = Random.Range(0, manualTrap2TileObjects.Length);
        } while (selectedManualTrap2Index == previousManualTrap2Index);

        previousAutoTrapIndex = selectedAutoTrapIndex;
        previousManualTrapIndex = selectedManualTrapIndex;
        previousManualTrap2Index = selectedManualTrap2Index;

        do
        {
            selectedBlockIndex = Random.Range(0, blockTileObjects.Length);
        } while (selectedBlockIndex == previousBlockIndex);

        previousBlockIndex = selectedBlockIndex;

        initializeSelectedIndexes = true;
    }

    [Command]
    private void CmdInitializeSelectedIndexes()
    {
        selectedBlockIndex = -2;
    }

    [Command]
    private void CmdInitializeSelectedKingIndexes()
    {
        // Initialize the selected indexes on the server for this player
        selectedAutoTrapIndex = -2;
        selectedManualTrapIndex = -2;
        selectedManualTrap2Index = -2;
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

    private void OnRotationAngleChanged(float oldValue, float newValue)
    {
        // Apply the synchronized rotation angle to the preview object on the client
        rotationAngle = newValue;
        if (previewObject != null)
            previewObject.transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);
    }

    

    IEnumerator WaitForRoundControl() {
        while (roundControl == null) {

            roundControl = GameObject.Find("RoundControl(Clone)").GetComponent<RoundControl>();

            yield return null; // Wait for a frame before checking again
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

        initializeSelectedIndexes = false;
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

    private void ResetMovementInput()
    {
        // Reset the movement input
        movementInput = Vector2.zero;
    }
}
