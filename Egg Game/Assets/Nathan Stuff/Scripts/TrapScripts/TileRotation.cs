using UnityEngine;
using UnityEngine.Tilemaps;

public class TileRotation : MonoBehaviour
{
    [System.Serializable]
    public class ChildRotationInfo
    {
        public Transform childTransform; // Reference to the child transform
        public float rotationSpeed = -90f; // Rotation speed for the child
    }

    public float baseRotationSpeed = 90f; // Rotation speed for the base object
    public ChildRotationInfo[] childRotationInfos; // Array of child rotation information
    public RoundControl roundControl;

    void StartRotation()
    {
        RotateObject(transform, baseRotationSpeed);

        foreach (ChildRotationInfo childInfo in childRotationInfos)
        {
            StartChildRotation(childInfo);
        }
    }

    void StartChildRotation(ChildRotationInfo childInfo)
    {
        RotateObject(childInfo.childTransform, childInfo.rotationSpeed);
    }

    private void Start() {
        roundControl = GameObject.Find("RoundControl").GetComponent<RoundControl>();
    }

    void Update()
    {
        if(roundControl.timerOn){
            StartRotation();

            RotateObject(transform, baseRotationSpeed);

            foreach (ChildRotationInfo childInfo in childRotationInfos)
            {
                RotateObject(childInfo.childTransform, childInfo.rotationSpeed);
            }
        }
    }

    void RotateObject(Transform objectTransform, float rotationSpeed)
    {
        objectTransform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}