using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerController : MonoBehaviour
{
    PlayerController pc;

    private void Start() {
        pc = GetComponent<PlayerController>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Move();
            Debug.Log("Spacebar pressed!");
        }
    }

    public void Move() {
        MoveCloserToClosestPlayer();
    }

    private void MoveCloserToClosestPlayer() {
        // collect all vaible player targets
        GameObject[] playerObjectPiecesArray = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> playerControlled = new List<GameObject>();

        foreach (GameObject piece in playerObjectPiecesArray) {
            AIPlayerController apc = piece.GetComponent<AIPlayerController>();

            if (apc == null) {
                playerControlled.Add(piece);
            }
        }


        if (playerControlled.Count > 0) {


            // choose target by calculating which player is the closest
            GameObject currentPositionalTile = pc.FindClosestTile(transform.position);
            GameObject closestTarget = playerControlled[0];

            foreach (GameObject targetObject in playerControlled) {
                // get tile of each, new and old, compared distances to current positional tile

                GameObject oldTile = pc.FindClosestTile(closestTarget.transform.position);
                int oldTileDistance = pc.GetTileDistance(currentPositionalTile, oldTile);

                GameObject newTile = pc.FindClosestTile(targetObject.transform.position);
                int newTileDistance = pc.GetTileDistance(currentPositionalTile, newTile);

                if (oldTileDistance > newTileDistance) {
                    closestTarget = targetObject;
                }

            }


            // Find best tile AI can travel to
            GameObject bestTile = pc.GetBestReachableTileTowardsTarget(pc.FindClosestTile(closestTarget.transform.position), pc.characterInfo.speed);
            pc.MoveToNewTile(bestTile);

        }

        
    }

    public void Attack() {
        AttackClosestPlayer();
    }

    private void AttackClosestPlayer() {

    }
}
