using UnityEngine;
using Mirror;

public class TileRotation : NetworkBehaviour
{
    public RoundControl roundControl;
    private bool rotationRequested = false;
    private float parentRotationSpeed = 35f;
    private float childRotationSpeed = -35f;

    [SyncVar(hook = nameof(OnRotationStateChanged))]
    private float currentRotation = 0f;

    private void Start()
    {
        roundControl = GameObject.Find("RoundControl").GetComponent<RoundControl>();
    }

    [ServerCallback]
    private void Update()
    {
        if (roundControl.timerOn)
        {
            rotationRequested = true;
            currentRotation = parentRotationSpeed;
        }
    }

    [ClientRpc]
    void RpcRotateOnClients(float speed)
    {
        parentRotationSpeed = speed;
        childRotationSpeed = -speed;
        rotationRequested = true;
        currentRotation = speed;
    }

    void UpdateRotation(Transform transformToRotate, float rotationSpeed)
    {
        if (rotationRequested)
        {
            transformToRotate.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnRotationStateChanged(float oldRotation, float newRotation)
    {
        parentRotationSpeed = newRotation;
        childRotationSpeed = -newRotation;
        rotationRequested = true;
        currentRotation = newRotation;

        transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);
        foreach (Transform childTransform in transform)
        {
            childTransform.rotation = Quaternion.Euler(0f, 0f, -currentRotation);
        }
    }

    private void FixedUpdate()
    {
        if (isServer)
        {
            RpcRotateOnClients(parentRotationSpeed);
        }

        UpdateRotation(transform, parentRotationSpeed);

        foreach (Transform childTransform in transform)
        {
            UpdateRotation(childTransform, childRotationSpeed);
        }
    }
}
