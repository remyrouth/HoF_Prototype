using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    [Header("")]
    public CharacterStats characterInfo;
    public List<GameObject> tiles;
    public float yOffset = 1f;
    private NavMeshAgent agent;
    public bool isPlayerEntity = true;


    public int playerHealth = 0;



    public bool hasAttackedYet = false;
    public bool hasMovedYet = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        TileMapSetup();
        PositionalCorrectionSetup();
    }

    // Main Action Methods

    public void MoveToNewTile(GameObject newTile)
    {
        hasMovedYet = true;
        TileMapSetup();
        // is new tile in range? 
        List<GameObject> reachableTiles = GetReachableTiles(characterInfo.speed);
        if (reachableTiles.Contains(newTile)) {
            StartCoroutine(MoveAlongTiles(newTile));
        }
        // StartCoroutine(MoveAlongTiles(newTile));
    }

    public bool AttackTile(GameObject newTile) {
        hasAttackedYet = true;
        GameObject objectOnAttackedTile = FindMatchingObjectToTile(newTile);
        if (objectOnAttackedTile != null) {
            Debug.Log("Attackable entity found");

            objectOnAttackedTile.GetComponent<PlayerController>().TakeDamage(characterInfo.strength);

            return true;
        }
        return false;
    }

    public void TakeDamage(int damage) {
        playerHealth -= damage;
        playerHealth = Mathf.Max(0, playerHealth);

        if (playerHealth == 0) {
            Destroy(gameObject);
        }
    }

    // Helper Methods

    private GameObject FindMatchingObjectToTile(GameObject newTile) {
        GameObject[] playerObjectPiecesArray = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject character in playerObjectPiecesArray)
        {
            bool matchingX = (character.transform.position.x == newTile.transform.position.x);
            bool matchingZ = (character.transform.position.z == newTile.transform.position.z);
            if (matchingX && matchingZ) {
                return character;
            }
        }

        return null;
    }

    private GameObject FindClosestTile(Vector3 point)
    {
        GameObject closestTile = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (GameObject tile in tiles)
        {
            Vector3 directionToTile = tile.transform.position - point;
            float dSqrToTile = directionToTile.sqrMagnitude;

            if (dSqrToTile < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTile;
                closestTile = tile;
            }
        }

        return closestTile;
    }

    private bool IsTileOccupied(GameObject newTile) {
        GameObject[] playerObjectPiecesArray = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject character in playerObjectPiecesArray)
        {
            bool matchingX = (character.transform.position.x == newTile.transform.position.x);
            bool matchingZ = (character.transform.position.z == newTile.transform.position.z);
            if (matchingX && matchingZ) {
                return true;
            }
        }

        return false;
    }

    public void ResetMoveAndAttackStates() {
        hasMovedYet = false;
        hasAttackedYet = false;
    }
    // Setup Methods 
    private void PositionalCorrectionSetup()
    {
        Vector3 positionalCorrection = FindClosestTile(gameObject.transform.position).transform.position;
        gameObject.transform.position = new Vector3(positionalCorrection.x, positionalCorrection.y + yOffset, positionalCorrection.z);
    }

    private void TileMapSetup()
    {
        // Initialize the list
        tiles = new List<GameObject>();

        // Find all GameObjects with the tag "Tile" and add them to the list
        GameObject[] tilesArray = GameObject.FindGameObjectsWithTag("Tile");
        foreach (GameObject tile in tilesArray)
        {
            tiles.Add(tile);
        }
    }

    private void SetupInfoToScript() {
        playerHealth = characterInfo.health;
    }

    // Pathing Methods
    private IEnumerator MoveAlongTiles(GameObject destinationTile)
    {
        List<GameObject> path = FindPath(FindClosestTile(transform.position), destinationTile);

        foreach (GameObject tile in path)
        {
            agent.SetDestination(tile.transform.position + Vector3.up * yOffset);
            yield return new WaitUntil(() => agent.remainingDistance < 0.1f);
        }
    }

    private List<GameObject> FindPath(GameObject startTile, GameObject endTile)
    {
        List<GameObject> openSet = new List<GameObject> { startTile };
        Dictionary<GameObject, GameObject> cameFrom = new Dictionary<GameObject, GameObject>();
        Dictionary<GameObject, float> gScore = new Dictionary<GameObject, float>();
        Dictionary<GameObject, float> fScore = new Dictionary<GameObject, float>();

        gScore[startTile] = 0;
        fScore[startTile] = Vector3.Distance(startTile.transform.position, endTile.transform.position);

        while (openSet.Count > 0)
        {
            GameObject current = openSet[0];
            foreach (GameObject tile in openSet)
            {
                if (fScore.ContainsKey(tile) && fScore[tile] < fScore[current])
                {
                    current = tile;
                }
            }

            if (current == endTile)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            foreach (GameObject neighbor in GetNeighbors(current))
            {
                if (IsTileOccupied(neighbor) && neighbor != endTile) continue; // Skip occupied tiles

                float tentativeGScore = gScore[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Vector3.Distance(neighbor.transform.position, endTile.transform.position);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return new List<GameObject>(); // No path found
    }

    public List<GameObject> GetAttackableTiles(int maxDistance)
    {
        TileMapSetup();

        GameObject startTile = FindClosestTile(transform.position);
        List<GameObject> reachableTiles = new List<GameObject>();
        Queue<(GameObject tile, int distance)> queue = new Queue<(GameObject, int)>();
        HashSet<GameObject> visited = new HashSet<GameObject>();

        queue.Enqueue((startTile, 0));
        visited.Add(startTile);

        while (queue.Count > 0)
        {
            var (currentTile, currentDistance) = queue.Dequeue();

            if (currentDistance <= maxDistance)
            {
                reachableTiles.Add(currentTile);

                if (currentDistance < maxDistance)
                {
                    foreach (GameObject neighbor in GetNeighbors(currentTile))
                    {
                        if (!visited.Contains(neighbor))
                        {
                            queue.Enqueue((neighbor, currentDistance + 1));
                            visited.Add(neighbor);
                        }
                    }
                }
            }
        }

        return reachableTiles;
    }

    public List<GameObject> GetReachableTiles(int maxDistance)
    {
        TileMapSetup();

        GameObject startTile = FindClosestTile(transform.position);
        List<GameObject> reachableTiles = new List<GameObject>();
        Queue<(GameObject tile, int distance)> queue = new Queue<(GameObject, int)>();
        HashSet<GameObject> visited = new HashSet<GameObject>();

        queue.Enqueue((startTile, 0));
        visited.Add(startTile);

        while (queue.Count > 0)
        {
            var (currentTile, currentDistance) = queue.Dequeue();

            if (currentDistance <= maxDistance)
            {
                reachableTiles.Add(currentTile);

                if (currentDistance < maxDistance)
                {
                    foreach (GameObject neighbor in GetNeighbors(currentTile))
                    {
                        if (!visited.Contains(neighbor) && !IsTileOccupied(neighbor))
                        {
                            queue.Enqueue((neighbor, currentDistance + 1));
                            visited.Add(neighbor);
                        }
                    }
                }
            }
        }

        return reachableTiles;
    }

    private List<GameObject> ReconstructPath(Dictionary<GameObject, GameObject> cameFrom, GameObject current)
    {
        List<GameObject> path = new List<GameObject> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }
    private List<GameObject> GetNeighbors(GameObject tile)
    {
        List<GameObject> neighbors = new List<GameObject>();
        float tileSize = 1f; // Adjust this based on your tile size

        foreach (GameObject potentialNeighbor in tiles)
        {
            if (Vector3.Distance(tile.transform.position, potentialNeighbor.transform.position) <= tileSize * 1.5f)
            {
                neighbors.Add(potentialNeighbor);
            }
        }

        return neighbors;
    }
}