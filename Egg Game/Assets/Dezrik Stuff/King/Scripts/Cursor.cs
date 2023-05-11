using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    public Transform mCursorVisual;
    public Vector3 mDisplacement;
    void Start()
    {

    }

    void Update()
    {
        mCursorVisual.position = Input.mousePosition + mDisplacement;
    }
}
