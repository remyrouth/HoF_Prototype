using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyRobotChaserController : MonoBehaviour
{
    // animation variables
    [SerializeField] private Animator animator;
    [SerializeField] private float transitionRate = 2f; // Rate at which the Running Layer weight changes
    private static readonly int RunLayerIndex = 1; // Assuming Running Layer is at index 1
    private static readonly int IdleLayerIndex = 2; // Assuming Idle Layer is at index 2
    private float runningTargetWeight = 0f; // Target weight for the Running Layer
    private float IdleTargetWeight = 1f; // Target weight for the Idle Layer

    // moving variables
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float rotationSpeed = 5f; // New variable for rotation speed
    [SerializeField] private float delayToMove = 1f;
    [SerializeField] private GameObject playerObject; // the object this object will face and walk towards
    [SerializeField] private GameObject endChaseObject; // the object this object to trigger the end of the robo chase
    [SerializeField] private float detectDistance = 1f; // the minimum distance to identify a target as reached 

    private bool isChasing = false;
    private float chaseStartTime;

    private void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Ensure the Idle layer is active at the start
        animator.SetLayerWeight(IdleLayerIndex, 1f);
        animator.SetLayerWeight(RunLayerIndex, 0f);

        // Start the chase after the delay
        StartCoroutine(StartChaseAfterDelay());
    }

    private IEnumerator StartChaseAfterDelay()
    {
        yield return new WaitForSeconds(delayToMove);
        isChasing = true;
        chaseStartTime = Time.time;
        runningTargetWeight = 1f;
        IdleTargetWeight = 0f;
    }

    private void Update()
    {
        if (!isChasing) return;

        // Move towards the player
        if (playerObject != null)
        {
            // Calculate direction to player, ignoring Y axis
            Vector3 directionToPlayer = playerObject.transform.position - transform.position;
            directionToPlayer.y = 0;
            directionToPlayer.Normalize();

            // Move in the calculated direction
            transform.position += directionToPlayer * moveSpeed * Time.deltaTime;

            // Rotate to face the player (only on Y axis)
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Check if we've reached the end chase object
        if (endChaseObject != null && Vector3.Distance(transform.position, endChaseObject.transform.position) <= detectDistance)
        {
            isChasing = false;
            runningTargetWeight = 0f;
            IdleTargetWeight = 1f;
        }

        // Update animation layer weights
        float currentRunWeight = animator.GetLayerWeight(RunLayerIndex);
        float currentIdleWeight = animator.GetLayerWeight(IdleLayerIndex);

        animator.SetLayerWeight(RunLayerIndex, Mathf.MoveTowards(currentRunWeight, runningTargetWeight, Time.deltaTime * transitionRate));
        animator.SetLayerWeight(IdleLayerIndex, Mathf.MoveTowards(currentIdleWeight, IdleTargetWeight, Time.deltaTime * transitionRate));
    }
}