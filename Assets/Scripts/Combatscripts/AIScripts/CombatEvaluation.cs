using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Combatscripts.AIScripts
{
    public class CombatEvaluation : MonoBehaviour
    {
        public AnimationCurve attackDistanceCurve;
        public PlayerController playerController;
        public CombatEvaluation(PlayerController playerController)
        {
            if (playerController)
            {
                this.playerController = playerController;
            }
            else
            {
                Debug.LogError("PlayerController passed to CombatEvaluation is not initialized!");
            }
        }
        
        public Tuple<GameObject,string> FindBestCell()
        {
            return Evaluate();
        }

        private Tuple<GameObject, string> Evaluate()
        {
            Dictionary<GameObject, float> gridMap = new Dictionary<GameObject, float>();
            List<GameObject> playerPieces = GameObject.FindGameObjectsWithTag("Player").ToList();
            List<GameObject> attackablePieces = new List<GameObject>();
            List<GameObject> laserTiles = playerController.GetAttackableTiles(playerController.RetrievePilotInfo().GetLaserRange());
            List<GameObject> ballisticTiles = playerController.GetAttackableTiles(playerController.RetrievePilotInfo().GetBallisticRange());

            if (playerPieces.Count <= 0)
            {
                return null;
            }
            else
            {
                //All attackable tiles
                List<GameObject> allAttackableTiles = 
                    playerController.GetAttackableTiles(playerController.RetrievePilotInfo().GetLaserRange());
                foreach (var ballisticTile in playerController.GetAttackableTiles(playerController.RetrievePilotInfo()
                             .GetBallisticRange()))
                {
                    if (!allAttackableTiles.Contains(ballisticTile))
                    {
                        allAttackableTiles.Add(ballisticTile);
                    }
                }
                foreach (var player in playerPieces)
                {
                    if (player.GetComponent<AIPlayerController>()) continue; //the FindGameObjectsWithTag("Player") method also returns all AI agents, by using this line we will ignore it
                    
                    GameObject playerTile = playerController.FindClosestTile(player.transform.position);

                    if (allAttackableTiles.Contains(playerTile))
                    {
                        attackablePieces.Add(playerTile);
                    }
                }
                
                foreach (var attackablePlayer in attackablePieces)
                {
                    float attackDistance = Vector3.Distance(attackablePlayer.transform.position,
                        playerController.transform.position);
                    float attackPriority = attackDistanceCurve.Evaluate(attackDistance);

                    gridMap.TryAdd(attackablePlayer, attackPriority);
                }
            }

            GameObject mostValuableAttackCell;
            if (attackablePieces.Count != 0)
            {
                mostValuableAttackCell = gridMap.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            }
            else
            {
                return new Tuple<GameObject, string>(null, null);
            }
            
            if (!laserTiles.Contains(mostValuableAttackCell))
            {
                return new Tuple<GameObject, string>(mostValuableAttackCell, "ballistic");
            } else if (!ballisticTiles.Contains(mostValuableAttackCell))
            {
                return new Tuple<GameObject, string>(mostValuableAttackCell, "laser");
            }
            else //both lists contain the most promising tile
            {
                if (playerController.RetrievePilotInfo().GetBallisticRange() <
                    playerController.RetrievePilotInfo().GetLaserRange())
                {
                    return new Tuple<GameObject, string>(mostValuableAttackCell, "ballistic");
                }
                else
                {
                    return new Tuple<GameObject, string>(mostValuableAttackCell, "laser");
                }
            }
        }
    }
}