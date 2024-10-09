using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

namespace Combatscripts.AIScripts
{
    
    //This class is used to store all our vital information for a given spatial curve.
    [System.Serializable]
    public class SpatialFunction
    {
        public Input input;
        public Operation operation;
        public AnimationCurve curve;
    }

    //This enum is used for determining what parameter we want to analyze.
    public enum Input
    {
        LineOfSight,
        TargetRange,
        PathDistance
    }

    //This enum is used for determining how we want to modify our gridMap.
    public enum Operation
    {
        Multiply,
        Add
    }
    public class SpatialEvaluation : MonoBehaviour

    {
        public PlayerController playerController;
        public SpatialFunction[] spatialCurves;

        //A constructor for SpatialEvaluation, currently not used but could be helpful in the future.
        public SpatialEvaluation(PlayerController playerController)
        {
            if (playerController)
            {
                this.playerController = playerController;
            }
            else
            {
                Debug.LogError("PlayerController passed to SpatialEvaluate is not initialized!");
            }
        }

        //This method is called in the AIPlayerController script. The purpose of this method is to return the
        //tile the enemy wants to move to as a GameObject.
        public GameObject FindBestCell()
        {
            //The reason I use "out" here is so that we don't need to store unnecessary data.
            Dijkstra(out Dictionary<GameObject, float> distanceMap);
            //The ref key word allows us to pass our distanceMap by reference.
            return Evaluate(ref distanceMap);
        }

        //The purpose of this method is to determine what parameter we want to evaluate for our spatial curve and
        //call the method associated with that parameter. After the gridMap is populated by the parameter method
        //the Evaluate method will return the GameObject/tile with the highest score.
        private GameObject Evaluate(ref Dictionary<GameObject, float> distanceDictionary)
        {
            Dictionary<GameObject, float> gridMap = new Dictionary<GameObject, float>();
            foreach (var spatialFunction in spatialCurves)
            {
                switch (spatialFunction.input)
                {
                    case(Input.LineOfSight):
                        LineOfSightCalculation(spatialFunction, ref gridMap, ref distanceDictionary);
                        break;
                    case(Input.PathDistance):
                        PathDistanceCalculation(spatialFunction, ref gridMap, ref distanceDictionary);
                        break;
                    case(Input.TargetRange):
                        TargetRangeCalculation(spatialFunction, ref gridMap, ref distanceDictionary);
                        break;
                }
            }
            return gridMap.Aggregate((x, y) => x.Value > y.Value ? x : y).Key; //returns the most promising cell from the grid
        }

        
        //DISCLAIMER: This code (LineOfSightCalculation) is untested as I am not aware of the utility it would provide to the gameplay of Habit
        //of Force. That being said, I will leave some comments that will hopefully illuminate how it works.
        
        //This method will populate our gridMap with data regarding whether a given tile on the board is within the line
        //of sight of the current enemy. It does this by ray casting to every tile on the board and returning if the ray
        //registers a hit.
        //NOTE: This method can be optimized by getting the farthest unobstructed tile in a given direction and assigning
        //all tiles that come before it to not being hit. The opposite applies for any tiles behind a tile that has been
        //hit. I.e. if there is a rock in front of the given enemy it will cast a ray to that rock, register a hit, and
        //assign any tiles behind that rock to being hit. This saves us from making a ton of ray casts.
        private void LineOfSightCalculation(SpatialFunction spatialFunction, ref Dictionary<GameObject, float> gridMap,
            ref Dictionary<GameObject, float> distanceDictionary)
        {
            LayerMask obstacleMask = LayerMask.NameToLayer("Obstacle");

            float curveValue = -1f; //want a default value to check with
            GameObject startTile = playerController.FindClosestTile(playerController.transform.position);
            Vector3 offset = new Vector3(0f, .6f, 0f); // want to make sure our raycast is not colliding with the ground

            foreach (KeyValuePair<GameObject, float> pair in distanceDictionary)
            {
                GameObject endTile = playerController.FindClosestTile(pair.Key.transform.position);
                Vector3 direction = (endTile.transform.position) - (startTile.transform.position);
                float distance = direction.magnitude;

                direction.Normalize();

                if (Physics.Raycast(startTile.transform.position + offset, direction, out RaycastHit hit, distance,
                        ~obstacleMask))
                {
                    curveValue = spatialFunction.curve.Evaluate(0f); //hit
                }
                else
                {
                    curveValue = spatialFunction.curve.Evaluate(1f); //clear LOS
                }

                gridMap.TryAdd(pair.Key, 0f);

                switch (spatialFunction.operation)
                {
                    case Operation.Add:
                        gridMap[pair.Key] += curveValue;
                        break;
                    case Operation.Multiply:
                        gridMap[pair.Key] *= curveValue;
                        break;
                    default:
                        gridMap[pair.Key] = gridMap[pair.Key];
                        break;
                }
            }
        }

        //The PathDistanceCalculation method will run all the values calculated in Dijkstra's algorithm through our curve
        //and store the score returned from the curve in our gridMap.
        private void PathDistanceCalculation(SpatialFunction spatialFunction, ref Dictionary<GameObject, float> gridMap,
            ref Dictionary<GameObject, float> distanceDictionary)
        {
            float curveValue = -1f; //want a default value to check with

            foreach (KeyValuePair<GameObject, float> pair in distanceDictionary)
            {
                curveValue = spatialFunction.curve.Evaluate(pair.Value);
                gridMap.TryAdd(pair.Key, 0f);

                switch (spatialFunction.operation)
                {
                    case Operation.Add:
                        gridMap[pair.Key] += curveValue;
                        break;
                    case Operation.Multiply:
                        gridMap[pair.Key] *= curveValue;
                        break;
                    default:
                        gridMap[pair.Key] = gridMap[pair.Key];
                        break;
                }
            }
        }

        //The TargetRangeCalculation method will assign our target as the closest player entity and based on the distance
        //between the enemy and the closest player will rank the tiles on the board based on our target range curve.
        private void TargetRangeCalculation(SpatialFunction spatialFunction, ref Dictionary<GameObject, float> gridMap,
            ref Dictionary<GameObject, float> distanceDictionary)
        {
            GameObject target = null;
            GameObject[] playerObjectPiecesArray = GameObject.FindGameObjectsWithTag("Player");
            float distance = float.MaxValue;
            foreach (var player in playerObjectPiecesArray)
            {
                if (!player.GetComponent<AIPlayerController>()) //not an AI
                {
                    float tempDistance = Vector3.Distance(player.transform.position, playerController.transform.position);
                    if (tempDistance < distance)
                    {
                        distance = tempDistance;
                        target = player;
                    }
                }
            }

            if (target)
            {
                float curveValue = -1f; //want a default value to check with

                foreach (KeyValuePair<GameObject, float> pair in distanceDictionary)
                {
                    //the reason I am taking the floor of this value is b/c each tile is 1.5 units apart, by taking the floor we can just think of our curve as 1 unit per 1 tile
                    float distanceToTarget = Mathf.Floor(Vector3.Distance(pair.Key.transform.position, target.transform.position));
                    curveValue = spatialFunction.curve.Evaluate(distanceToTarget);
                    gridMap.TryAdd(pair.Key, 0f);

                    switch (spatialFunction.operation)
                    {
                        case Operation.Add:
                            gridMap[pair.Key] += curveValue;
                            break;
                        case Operation.Multiply:
                            gridMap[pair.Key] *= curveValue;
                            break;
                        default:
                            gridMap[pair.Key] = gridMap[pair.Key];
                            break;
                    }
                }
            }
            else
            {
                Debug.LogError("No Target Found in SpatialEvaluation.cs");
            }
        }

        //This is a standard implementation of Dijkstra's algorithm. This method returns a Dictionary that maps
        //a tile to a float value that represents how far away from the player the given tile is.
        //Here are some good resources for understanding Dijkstra's:
        // https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm
        // https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm
        private void Dijkstra(out Dictionary<GameObject, float> distanceMap)
        {
            distanceMap = new Dictionary<GameObject, float>();
            List<(GameObject, float, GameObject)> openSet = new List<(GameObject, float, GameObject)>();
            List<(GameObject, float, GameObject)> closedSet = new List<(GameObject, float, GameObject)>();
            GameObject start = playerController.FindClosestTile(playerController.transform.position);

            openSet.Add((start, 0, start));

            while (openSet.Count > 0)
            {
                openSet.Sort((a, b) => (a.Item2).CompareTo(b.Item2)); //treating the openSet like a priority queue

                //from our priority queue we pop off the most promising cell
                var mostPromisingCell = openSet[0];
                openSet.RemoveAt(0);

                //If we have already iterated over this cell, skip it
                if (closedSet.Contains(mostPromisingCell) || distanceMap.ContainsKey(mostPromisingCell.Item1))
                {
                    continue;
                }

                //If our distance map already contains this cell, replace it with the value of the cell we are currently iterated on
                //if it doesn't have this cell, add it to the distance map
                if (distanceMap.ContainsKey(mostPromisingCell.Item1))
                {
                    distanceMap[mostPromisingCell.Item1] = mostPromisingCell.Item2;
                }
                else
                {
                    distanceMap.Add(mostPromisingCell.Item1, mostPromisingCell.Item2);
                }

                closedSet.Add(mostPromisingCell);

                foreach (var neighbor in playerController.GetNeighbors(mostPromisingCell.Item1))
                {
                    if (playerController.IsTileOccupied(neighbor) || mostPromisingCell.Item1 == neighbor) continue; //eliminates possibility of including the tile itself

                    float distance = mostPromisingCell.Item2 + Vector3.Distance(neighbor.transform.position,
                        mostPromisingCell.Item1.transform.position);

                    foreach (var node in openSet)
                    {
                        if (node.Item1.transform.position == neighbor.transform.position && node.Item2 < distance)
                        {
                            continue;
                        }
                    }

                    foreach (var node in closedSet)
                    {
                        if (node.Item1.transform.position == neighbor.transform.position && node.Item2 < distance)
                        {
                            continue;
                        }
                    }

                    openSet.Add((neighbor, distance, mostPromisingCell.Item1));
                }
            }
        }
    }
}