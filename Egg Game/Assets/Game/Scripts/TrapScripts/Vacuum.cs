using UnityEngine;
using Mirror;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Vacuum : NetworkBehaviour
{
    public float followSpeed = 50f;
    private Rigidbody2D rb;
    [SerializeField]
    private bool followingMouse = false;
    [SerializeField]
    private bool isActivatedByLocalPlayer = false;
    public GameObject suckArea;
    private float exitTime = 2.5f;
    private Coroutine deactivateCoroutine;
    [SerializeField] private RoundControl roundControl;
    
    void Start()
    {
        StartCoroutine(WaitForRoundControl());
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        suckArea.SetActive(false);
    }

    private void Update()
    {
        if (followingMouse && isActivatedByLocalPlayer)
        {
            ActivateFunction();
        }

        if (isActivatedByLocalPlayer&& roundControl.placingItems && roundControl.Round >= 1)
        {
            TargetDeactivateVacuum();
        }
    }

    [Command]
    public void CmdActivateSuck()
    {
        RpcActivateSuck();
    }

    [ClientRpc]
    private void RpcActivateSuck()
    {
        suckArea.SetActive(true);
    }

    [Command]
    private void CmdDeactivateSuck()
    {
        RpcDeactivateSuck();
        this.GetComponentInParent<NetworkIdentity>().RemoveClientAuthority();
    }

    [ClientRpc]
    private void RpcDeactivateSuck()
    {
        suckArea.SetActive(false);
        Debug.Log("Vacuum deactivated by local player");
    }

    [TargetRpc]
    public void TargetActivateVacuum(NetworkConnection target)
    {
        // Activate the vacuum only for the client that clicked
        isActivatedByLocalPlayer = true;
        followingMouse = true;
        CmdActivateSuck();
    }

    //[TargetRpc]
    public void TargetDeactivateVacuum()
    {
        // Deactivate the vacuum only for the client that clicked
        isActivatedByLocalPlayer = false;
        followingMouse = false;
        CmdDeactivateSuck();
    }
    
    private void ActivateFunction()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetPosition = new Vector3(mousePosition.x, mousePosition.y, transform.position.z);

        // Check the distance between the current position and the target position
        float distance = Vector3.Distance(transform.position, targetPosition);

        // Only move if the distance is above a certain threshold
        if (distance > 0.1f)
        {
            // Gradual rotation towards the target
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, targetPosition - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, followSpeed * Time.deltaTime);

            // Smooth movement towards the target
            Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
            rb.MovePosition(newPosition);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("TrapCursor"))
        {
            Debug.Log("Exited from TrapCursor");

            if (isActivatedByLocalPlayer)
            {
                // If a coroutine is already running, stop it
                if (deactivateCoroutine != null)
                {
                    StopCoroutine(deactivateCoroutine);
                }

                // Start a new coroutine to wait for the specified duration before deactivating
                deactivateCoroutine = StartCoroutine(DeactivateAfterDelay());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TrapCursor"))
        {
            Debug.Log("Entered TrapCursor");

            if (isActivatedByLocalPlayer)
            {
                // If a coroutine is already running, stop it
                if (deactivateCoroutine != null)
                {
                    StopCoroutine(deactivateCoroutine);
                }
            }
        }
    }

    private IEnumerator DeactivateAfterDelay()
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(exitTime);
        TargetDeactivateVacuum();
        Debug.Log("Deactivated after delay");

        deactivateCoroutine = null; // Reset the coroutine reference
    }

    IEnumerator WaitForRoundControl() {
        while (roundControl == null) {

            roundControl = GameObject.Find("RoundControl(Clone)").GetComponent<RoundControl>();

            yield return null; // Wait for a frame before checking again
        }
    }
}
