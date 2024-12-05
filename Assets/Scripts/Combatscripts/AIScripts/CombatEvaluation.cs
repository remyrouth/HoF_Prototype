using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Combatscripts.AIScripts
{
    public class CombatEvaluation : MonoBehaviour
    {
        // this is a bad class name. It should be named something that related to tile evaluation
        // combat is too vague and general. The naming convention is poor
        public AnimationCurve attackDistanceCurve;
        public PlayerController playerController;
        
        //A constructor for CombatEvaluation, currently not used but could be helpful in the future.
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
        
        //This method is called in the AIPlayerController script. Although it's method body is only 1 line, it is formatted
        //this way to match the SpatialEvaluationn.FindBestCell() method. The purpose of this method is to return the
        //tile the enemy wants to attack as a GameObject and the type of attack it wants to perform as a string.
        
        //IN THE FUTURE: Could switch the second type to either a enum or a scriptable object.
        public Tuple<GameObject,string> FindBestCell()
        {
            return Evaluate();
        }

        //This method is used to determine the most promising player to attack and what type of attack to use on that player.
        private Tuple<GameObject, string> Evaluate()
        {
            //The gripMap stores a player tile and its score (how much the enemy wants to attack it) in a dictionary.
            Dictionary<GameObject, float> gridMap = new Dictionary<GameObject, float>();
            //This is a list of all the GameObjects in the scene with the "Player" tag.
            List<GameObject> playerPieces = GameObject.FindGameObjectsWithTag("Player").ToList();
            //This list represents all the attackable tiles on the board.
            List<GameObject> attackablePieces = new List<GameObject>();
            //This list contains all tiles that are attackable with a laser attack (range is determined by pilot scriptable object).
            List<GameObject> laserTiles = playerController.GetAttackableTiles(playerController.RetrievePilotInfo().GetLaserRange());
            //This list contains all tiles that are attackable with a ballistic attack (range is determined by pilot scriptable object).
            List<GameObject> ballisticTiles = playerController.GetAttackableTiles(playerController.RetrievePilotInfo().GetBallisticRange());

            if (playerPieces.Count <= 0)
            {
                return null;
            }
            
            
            List<GameObject> allAttackableTiles = 
                playerController.GetAttackableTiles(playerController.RetrievePilotInfo().GetLaserRange());
            //This foreach loop will make sure we only have 1 instance of a given tile in our allAttackableTiles list.
            //This is because 1 tile can be attackable with multiple damage types.
            foreach (var ballisticTile in playerController.GetAttackableTiles(playerController.RetrievePilotInfo()
                         .GetBallisticRange()))
            {
                if (!allAttackableTiles.Contains(ballisticTile))
                {
                    allAttackableTiles.Add(ballisticTile);
                }
            }
            //This foreach loop is to make sure we don't add any player pieces to our attackablePieces list that are
            //controlled by AI.
            foreach (var player in playerPieces)
            {
                if (player.GetComponent<AIPlayerController>()) continue; //the FindGameObjectsWithTag("Player") method also returns all AI agents, by using this line we will ignore it
                    
                GameObject playerTile = playerController.FindClosestTile(player.transform.position);

                if (allAttackableTiles.Contains(playerTile))
                {
                    attackablePieces.Add(playerTile);
                }
            }
                
            //This foreach loop is where we are populating our gridMap dictionary (i.e. retrieving our score from our
            //animationCurve based on our distance from a given attackable piece).
            foreach (var attackablePlayer in attackablePieces)
            {
                float attackDistance = Vector3.Distance(attackablePlayer.transform.position,
                    playerController.transform.position);
                float attackPriority = attackDistanceCurve.Evaluate(attackDistance);

                gridMap.TryAdd(attackablePlayer, attackPriority);
            }
            
            GameObject mostValuableAttackCell;
            if (attackablePieces.Count != 0)
            {
                //This method will return the player piece with the highest score.
                mostValuableAttackCell = gridMap.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            }
            else
            {
                return new Tuple<GameObject, string>(null, null);
            }
            
            //These if statements are used to determine which attack type we want to use.
            //IMPORTANT: If the cell is attackable by both ballistic and laser, the method will currently
            //choose the attack type with the shorter range. Whether or not it does this in the future is a DESIGN
            //DECISION!
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