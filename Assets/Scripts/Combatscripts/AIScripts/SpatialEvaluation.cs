using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

namespace Combatscripts.AIScripts
{
    [System.Serializable]
    public class SpatialFunction
    {
        public Input input;
        public Operation operation;
        public AnimationCurve curve;
    }

    public enum Input
    {
        LineOfSight,
        TargetRange,
        PathDistance
    }

    public enum Operation
    {
        Multiply,
        Add
    }
    public class SpatialEvaluation : MonoBehaviour

    {
        public PlayerController playerController;
        public SpatialFunction[] spatialCurves;

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

        public GameObject FindBestCell()
        {
            Dijkstra(out Dictionary<GameObject, float> distanceMap);
            return Evaluate(ref distanceMap);
        }

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

        //for now: just grabs the closest player character
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
                    float distanceToTarget = Vector3.Distance(pair.Key.transform.position, target.transform.position);
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