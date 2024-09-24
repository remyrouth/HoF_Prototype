using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunController : MonoBehaviour
{
    // move variables
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float fallSpeed = 9.8f; // Fall speed when in the air
    [SerializeField] private float raycastDistance = 1.1f; // Distance for ground detection
    private bool isFalling = false; // Track if the player is falling

    private bool canUsePlayerInput = true; // used for pausing controller


    // animation variables
    [SerializeField] private Animator animator;
    [SerializeField] private float transitionRate = 1f; // Rate at which the Running Layer weight changes
    private static readonly int RunningLayerIndex = 1; // Assuming Running Layer is at index 1
    private static readonly int FallingLayerIndex = 2; // Assuming Running Layer is at index 2
    private float runningTargetWeight = 0f; // Target weight for the Running Layer
    private float fallingTargetWeight = 0f; // Target weight for the Falling Layer


    // camera variables
    public Vector3 cameraOffset;
    private Camera mainCamera;
    

    void Start()
    {
        animator.SetLayerWeight(RunningLayerIndex, 0f);
        mainCamera = Camera.main;
    }

    public void PauseControls(bool shouldPause) {
        canUsePlayerInput = !shouldPause;
    }

    void Update()
    {
        HandleMovement();
        UpdateCameraPosition();
        HandleFalling();

        // Check for W key input
         if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && canUsePlayerInput)
        {
            StartRunning();
        }
        else
        {
            StopRunning();
        }

        // Smoothly transition the Running Layer weight
        float currentRunningWeight = animator.GetLayerWeight(RunningLayerIndex);
        float newRunningWeight = Mathf.MoveTowards(currentRunningWeight, runningTargetWeight, Time.deltaTime * transitionRate);
        animator.SetLayerWeight(RunningLayerIndex, newRunningWeight);

        // Smoothly transition the Falling Layer weight
        float currentFallingWeight = animator.GetLayerWeight(FallingLayerIndex);
        float newFallingWeight = Mathf.MoveTowards(currentFallingWeight, fallingTargetWeight, Time.deltaTime * transitionRate);
        animator.SetLayerWeight(FallingLayerIndex, newFallingWeight);
    }



    // Call this method to start running
    private void StartRunning()
    {
        runningTargetWeight = 1f;
    }

    // Call this method to stop running
    private void StopRunning()
    {
        runningTargetWeight = 0f;
    }

    // Method to set the transition rate
    public void SetTransitionRate(float rate)
    {
        transitionRate = Mathf.Max(0f, rate); // Ensure the rate is non-negative
    }


    private void HandleFalling()
    {
        // Raycast down from the player's position
        if (!Physics.Raycast(transform.position, Vector3.down, raycastDistance))
        {
            // No ground detected, apply falling
            isFalling = true;
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);
            fallingTargetWeight = 1f; // Set falling animation weight to 1
            runningTargetWeight = 0f; // Ensure running animation is not playing while falling
        }
        else
        {
            // Ground detected, stop falling
            isFalling = false;
            fallingTargetWeight = 0f; // Set falling animation weight back to 0
        }
    }


    void HandleMovement()
    {
        if (!canUsePlayerInput) {
            return;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Get the camera's forward and right vectors, but ignore Y component
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Calculate the movement direction relative to the camera
        Vector3 desiredMoveDirection = forward * moveZ + right * moveX;

        // Apply movement to the player object
        transform.Translate(desiredMoveDirection * moveSpeed * Time.deltaTime, Space.World);

        // Optional: Make the player face the movement direction
        if (desiredMoveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), 0.15f);
        }
    }

    void UpdateCameraPosition()
    {
        if (mainCamera != null)
        {
            mainCamera.transform.position = transform.position + cameraOffset;
            mainCamera.transform.LookAt(transform.position);
        }
    }

    void OnDrawGizmos()
    {
        // Draw a sphere at the player's position
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.1f);

        // Draw a line showing the camera offset
        Gizmos.color = Color.red;
        Vector3 cameraPosition = transform.position + cameraOffset;
        Gizmos.DrawLine(transform.position, cameraPosition);

        // Draw a sphere at the camera's offset position
        Gizmos.DrawSphere(cameraPosition, 0.3f);
    }
}
