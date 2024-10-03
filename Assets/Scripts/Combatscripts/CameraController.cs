using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private bool isCurrentlyPaused = false;
    public float moveSpeed = 10f;
    [SerializeField] private Vector2 maxXZ = new Vector2(10f, 10f);
    [SerializeField] private Vector2 minXZ = new Vector2(-10f, -10f);
    [SerializeField] private Vector3 originalPosition;

    public void SetSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }

    public void SetPauseState(bool isPaused)
    {
        isCurrentlyPaused = isPaused;
    }

    private void ClampCamera()
    {
        Vector3 currPosition = gameObject.transform.position;

        float newX = Mathf.Clamp(currPosition.x, minXZ.x + originalPosition.x, maxXZ.x + originalPosition.x);
        float newZ = Mathf.Clamp(currPosition.z, minXZ.y + originalPosition.z, maxXZ.y + originalPosition.z);

        Vector3 newPosition = new Vector3(newX, currPosition.y, newZ);
        gameObject.transform.position = newPosition;
    }

    private void Start()
    {
        originalPosition = gameObject.transform.position;
    }

    void Update()
    {
        if (isCurrentlyPaused)
        {
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

        ClampCamera();
    }

    // Draw Gizmo lines to visualize the clamping area
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // Calculate the four corners of the clamping boundary
        Vector3 bottomLeft = new Vector3(minXZ.x + originalPosition.x, originalPosition.y, minXZ.y + originalPosition.z);
        Vector3 bottomRight = new Vector3(maxXZ.x + originalPosition.x, originalPosition.y, minXZ.y + originalPosition.z);
        Vector3 topLeft = new Vector3(minXZ.x + originalPosition.x, originalPosition.y, maxXZ.y + originalPosition.z);
        Vector3 topRight = new Vector3(maxXZ.x + originalPosition.x, originalPosition.y, maxXZ.y + originalPosition.z);

        // Draw lines between the corners to form a rectangle
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
}