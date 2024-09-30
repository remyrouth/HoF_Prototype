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
        Vector3 offset = transform.position - originalPosition;
        float newX = Mathf.Clamp(offset.x, minXZ.x, maxXZ.x);
        float newZ = Mathf.Clamp(offset.z, minXZ.y, maxXZ.y);
        transform.position = originalPosition + new Vector3(newX, 0f, newZ);
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

        Vector3 center = originalPosition;
        Vector3 size = new Vector3(maxXZ.x - minXZ.x, 0f, maxXZ.y - minXZ.y);
        Gizmos.DrawWireCube(center, size);
    }
}
