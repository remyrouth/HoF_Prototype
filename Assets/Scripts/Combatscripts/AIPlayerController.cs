using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerController : MonoBehaviour
{
    PlayerController pc;

    private void Start() {
        pc = GetComponent<PlayerController>();
    }

    public GameObject Move() {
        return MoveCloserToClosestPlayer();
    }

    private GameObject MoveCloserToClosestPlayer() {
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
            GameObject bestTile = pc.GetBestReachableTileTowardsTarget(pc.FindClosestTile(closestTarget.transform.position), pc.RetrievePilotInfo().GetPilotSpeed());
            pc.MoveToNewTile(bestTile);

            return bestTile;

        }

        return null;
        
    }

    public void Attack() {
        AttackClosestPlayer();
    }

    private void AttackClosestPlayer() {
        Debug.Log("Attacking closest player with method");
        // collect all vaible player targets
        GameObject[] playerObjectPiecesArray = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> playerControlled = new List<GameObject>();

        foreach (GameObject piece in playerObjectPiecesArray) {
            AIPlayerController apc = piece.GetComponent<AIPlayerController>();

            if (apc == null) {
                playerControlled.Add(piece);
            }
        }

        // Debug.Log("playerControlled.Count : " + playerControlled.Count);


        if (playerControlled.Count > 0) {
            // Debug.Log("playerControlled.Count : " + playerControlled.Count);


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


            // if player is in range, attack
            GameObject playerTile = pc.FindClosestTile(closestTarget.transform.position);
            List<GameObject> reachableTile = pc.GetAttackableTiles(pc.RetrievePilotInfo().GetLaserRange());

            if (reachableTile.Contains(playerTile)) {
                // Debug.Log("Attacked player tile");
                // pc.AttackTile(playerTile);
            } else {
                // Debug.Log("player tile not attacked");
            }

        }
    }
}
