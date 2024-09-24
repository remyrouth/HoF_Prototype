using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraController : MonoBehaviour
{
    private bool isCurrentlyPaused = false;
    public float moveSpeed = 10f;

    public void SetSpeed(float newSpeed) {
        moveSpeed = newSpeed;
    }

    public void SetPauseState(bool isPaused) {
        isCurrentlyPaused = isPaused;
    }

    void Update()
    {
        if (isCurrentlyPaused) {
            return;
        }
        
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector3.right;
        }

        // Normalize direction to ensure consistent speed in all directions
        if (direction.magnitude > 0)
        {
            direction.Normalize();
        }

        // Move the camera in global space
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
    }
}