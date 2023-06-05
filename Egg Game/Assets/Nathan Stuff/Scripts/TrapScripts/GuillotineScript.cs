using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuillotineScript : MonoBehaviour
{
    public Transform pointA; // Starting point
    public Transform pointB; // Target point
    public float moveSpeedDown = 5f; // Speed of movement down to point B
    public float moveSpeedUp = 2f; // Speed of movement back up to point A

    private bool isMovingDown = false; // Flag indicating if the object is moving down
    private bool isMovingUp = false; // Flag indicating if the object is moving up

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartMovingDown();
        }

        if (isMovingDown)
        {
            MoveDown();
        }
        else if (isMovingUp)
        {
            MoveUp();
        }
    }

    private void StartMovingDown()
    {
        isMovingDown = true;
        isMovingUp = false;
    }

    private void MoveDown()
    {
        transform.position = Vector3.MoveTowards(transform.position, pointB.position, moveSpeedDown * Time.deltaTime);

        if (transform.position == pointB.position)
        {
            isMovingDown = false;
            isMovingUp = true;
        }
    }

    private void MoveUp()
    {
        transform.position = Vector3.MoveTowards(transform.position, pointA.position, moveSpeedUp * Time.deltaTime);

        if (transform.position == pointA.position)
        {
            isMovingUp = false;
        }
    }
}
