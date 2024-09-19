using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private bool isCurrentlyPaused = false;
    public float moveSpeed = 10f;

    public Vector2 maxBounds;
    public Vector2 minBounds;


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

        Vector3 clampedPos = transform.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, minBounds.x, maxBounds.x);
        clampedPos.z = Mathf.Clamp(clampedPos.z, minBounds.y, maxBounds.y);
        transform.position = clampedPos;
    }

    private void OnDrawGizmos()
    {
        //draws a gizmos reprensenting the bounds of the camera. 
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(minBounds.x, transform.position.y, minBounds.y),
            new Vector3(maxBounds.x, transform.position.y, minBounds.y)); // Bottom line
        Gizmos.DrawLine(new Vector3(minBounds.x, transform.position.y, minBounds.y),
            new Vector3(minBounds.x, transform.position.y, maxBounds.y)); // Left line
        Gizmos.DrawLine(new Vector3(maxBounds.x, transform.position.y, minBounds.y),
            new Vector3(maxBounds.x, transform.position.y, maxBounds.y)); // Right line
        Gizmos.DrawLine(new Vector3(minBounds.x, transform.position.y, maxBounds.y),
            new Vector3(maxBounds.x, transform.position.y, maxBounds.y)); // Top line
    }
}
