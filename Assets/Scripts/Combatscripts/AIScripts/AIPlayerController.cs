using System;
using System.Collections;
using System.Collections.Generic;
using Combatscripts.AIScripts;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Analytics;


public enum CostByDistance
{
    High,
    Low
}
public class AIPlayerController : MonoBehaviour
{
    PlayerController attachedPlayerController;
    [SerializeField]
    private SpatialEvaluation spatialEvaluationComponent;
    private CombatEvaluation combatEvaluationComponent;
    public AnimationCurve attackDistancePreferenceCurve;
    public SpatialFunction[] spatialFunctions;
    
    //FOR DEBUGGING
    private Vector3 targetLocation;

    private void Start() {
        attachedPlayerController = GetComponent<PlayerController>();
        gameObject.AddComponent<SpatialEvaluation>();
        gameObject.AddComponent<CombatEvaluation>();
        spatialEvaluationComponent = GetComponent<SpatialEvaluation>();
        combatEvaluationComponent = GetComponent<CombatEvaluation>();
        spatialEvaluationComponent.playerController = attachedPlayerController;
        combatEvaluationComponent.playerController = attachedPlayerController;
        spatialEvaluationComponent.spatialCurves = spatialFunctions;
        combatEvaluationComponent.attackDistanceCurve = attackDistancePreferenceCurve;
    }
    
    // This method is called by the turn manager script. This move method must return the tile this AI intends
    // to move to, so that the turn manager knows when it has successfully reached its intended tile.
    // This way, it'll know when to start moving the next enemy
    public GameObject Move()
    {
        GameObject targetTile = spatialEvaluationComponent.FindBestCell();
        GameObject bestTile = attachedPlayerController.GetBestReachableTileTowardsTarget(attachedPlayerController.FindClosestTile(targetTile.transform.position), attachedPlayerController.RetrievePilotInfo().GetPilotSpeed());
        attachedPlayerController.MoveToTile(bestTile);
        targetLocation = bestTile.transform.position;
        return bestTile;
    }

    private GameObject MoveCloserToClosestPlayer() {
        // collect all viable player targets
        GameObject[] playerObjectPiecesArray = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> playerControlled = new List<GameObject>();

        foreach (GameObject piece in playerObjectPiecesArray) {
            AIPlayerController aattachedPlayerController = piece.GetComponent<AIPlayerController>();

            if (aattachedPlayerController == null) {
                playerControlled.Add(piece);
            }
        }


        if (playerControlled.Count > 0) {


            // choose target by calculating which player is the closest
            GameObject currentPositionalTile = attachedPlayerController.FindClosestTile(transform.position);
            GameObject closestTarget = playerControlled[0];

            foreach (GameObject targetObject in playerControlled) {
                // get tile of each, new and old, compared distances to current positional tile

                GameObject oldTile = attachedPlayerController.FindClosestTile(closestTarget.transform.position);
                int oldTileDistance = attachedPlayerController.GetTileDistance(currentPositionalTile, oldTile);

                GameObject newTile = attachedPlayerController.FindClosestTile(targetObject.transform.position);
                int newTileDistance = attachedPlayerController.GetTileDistance(currentPositionalTile, newTile);

                if (oldTileDistance > newTileDistance) {
                    closestTarget = targetObject;
                }

            }


            // Find best tile AI can travel to
            GameObject bestTile = attachedPlayerController.GetBestReachableTileTowardsTarget(attachedPlayerController.FindClosestTile(closestTarget.transform.position), attachedPlayerController.RetrievePilotInfo().GetPilotSpeed());
            attachedPlayerController.MoveToTile(bestTile);

            return bestTile;

        }

        return null;
        
    }

    public void Attack()
    {
        Tuple<GameObject,string> attackTargetTuple = combatEvaluationComponent.FindBestCell();
        GameObject attackTargetTile = attackTargetTuple.Item1;
        string attackTargetType = attackTargetTuple.Item2;
        targetLocation = attackTargetTile.transform.position;
        
        if (attackTargetTile == null)
        {
            return;
        }
        
        switch (attackTargetType)
        {
            case("laser"):
                int laserPower = attachedPlayerController.RetrievePilotInfo().GetLaserStrength();
                int laserRange = attachedPlayerController.RetrievePilotInfo().GetLaserRange();
                MechStats.AbilityMechSlot tempLaserSlot = CreateAttackSlotOption(laserPower, laserRange);
                attachedPlayerController.UseAbility(tempLaserSlot, attackTargetTile);
                break;
            case("ballistic"):
                int ballisticPower = attachedPlayerController.RetrievePilotInfo().GetBallisticStrength();
                int ballisticRange = attachedPlayerController.RetrievePilotInfo().GetBallisticRange();
                MechStats.AbilityMechSlot tempBallisticSlot = CreateAttackSlotOption(ballisticPower, ballisticRange);
                attachedPlayerController.UseAbility(tempBallisticSlot, attackTargetTile);
                break;
        }
        //AttackClosestPlayer();
    }

    private void AttackClosestPlayer() {
        Debug.Log("Attacking closest player with method");
        // collect all vaible player targets
        GameObject[] playerObjectPiecesArray = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> playerControlled = new List<GameObject>();

        foreach (GameObject piece in playerObjectPiecesArray) {
            AIPlayerController aattachedPlayerController = piece.GetComponent<AIPlayerController>();

            if (aattachedPlayerController == null) {
                playerControlled.Add(piece);
            }
        }
        Debug.Log("playerControlled.Count : " + playerControlled.Count);


        // find closest player to attack
        if (playerControlled.Count > 0) {
            Debug.Log("playerControlled.Count : " + playerControlled.Count);


            // choose target by calculating which player is the closest
            GameObject currentPositionalTile = attachedPlayerController.FindClosestTile(transform.position);
            GameObject closestTarget = playerControlled[0];

            foreach (GameObject targetObject in playerControlled) {
                // get tile of each, new and old, compared distances to current positional tile

                GameObject oldTile = attachedPlayerController.FindClosestTile(closestTarget.transform.position);
                int oldTileDistance = attachedPlayerController.GetTileDistance(currentPositionalTile, oldTile);

                GameObject newTile = attachedPlayerController.FindClosestTile(targetObject.transform.position);
                int newTileDistance = attachedPlayerController.GetTileDistance(currentPositionalTile, newTile);

                if (oldTileDistance > newTileDistance) {
                    closestTarget = targetObject;
                }

            }


            // if player is in range, attack
            GameObject playerTile = attachedPlayerController.FindClosestTile(closestTarget.transform.position);
            List<GameObject> reachableTile = attachedPlayerController.GetAttackableTiles(attachedPlayerController.RetrievePilotInfo().GetLaserRange());

            if (reachableTile.Contains(playerTile)) {
                // Debug.Log("Attacked player tile");
                // attachedPlayerController.AttackTile(playerTile);

                // same lines as in character canvas controller in the BeginAttackSystem method
                int laserPower = attachedPlayerController.RetrievePilotInfo().GetLaserStrength();
                int laserRange = attachedPlayerController.RetrievePilotInfo().GetLaserRange();
                MechStats.AbilityMechSlot tempLaserSlot = CreateAttackSlotOption(laserPower, laserRange);
                attachedPlayerController.UseAbility(tempLaserSlot, playerTile);
            } else {
                // Debug.Log("player tile not attacked");
            }

        }
    }

    // this exact method is currently in the character canvas controller, we can this somewhere more unified for optimization
    // we could probably move this method directly to the player, but it might create a bloat of code, but would be more clear
    // this change would be suggested. If this change is needed, desired, urgent, (anything), remind remy to do this.
    // another note, look at how character canvas controller uses it, to gather all options of laser, ballistic, and combo
    public MechStats.AbilityMechSlot CreateAttackSlotOption(int power, int attackRange) {
        MechStats.AbilityMechSlot tempSlot = new MechStats.AbilityMechSlot();
        // why are these 0? because standard attack options do not cost 
        // clarity to use/cast. And the minimum range of these abilities is
        // always zero. Other abilities may have a minimum range, 
        // standard attack options do not
        int clarityCost = 0;
        int minimumRange = 0;
        tempSlot.SetValues(power, clarityCost, minimumRange, attackRange);
        tempSlot.SetAbilityType(MechStats.AbilityType.Laser);

        return tempSlot;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(targetLocation, .5f);
    }
}
