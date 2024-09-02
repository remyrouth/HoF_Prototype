using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    [Header("Player Sources")]
    public CharacterStats pilotInfo;
    public MechStats mechInfo;
    public bool isPlayerEntity = true;


    [Header("Current Stats")]
    public int currentPlayerHealth = 0;
    public int currentMechHealth = 0;
    public int currentClarityLevel = 0;


    [Header("Action variables")]
    public bool hasAttackedYet = false;
    public bool hasMovedYet = false;


    [Header("Sound variable")]
    [SerializeField] private Sound deathSound;




    // moving variables
    public float yOffset = 1f;
    private List<GameObject> tiles;
    private NavMeshAgent agent;
    private AbilityExecutionManager aem;

    // manager variables
    private CombatStateController combatStateController;

    private void Start()
    {
        SetupInfoToScript();
        agent = GetComponent<NavMeshAgent>();
        aem = FindObjectOfType<AbilityExecutionManager>();
        TileMapSetup();
        PositionalCorrectionSetup();
        InstantiateMechObject();

        GameStateManager gameStateManager = FindObjectOfType<GameStateManager>();
        if (gameStateManager == null) {
            Debug.LogError("GameState does not exist");
        }
        combatStateController = gameStateManager.GetCombatStateController();

        if (combatStateController == null) {
            Debug.LogError("combatState does not exist");
        }
        if (isPlayerEntity) {
            combatStateController.IncreaseFriendlyCount(true);
        } else {
            combatStateController.IncreaseEnemyCount(true);
        }

    }

    public void SetPilotAndMechFromRosterScript(CharacterStats newPilot, MechStats newMech) {
        pilotInfo = newPilot;
        mechInfo = newMech;
    }

    public bool canReplacePlayerEntity() {
        return isPlayerEntity;
    }

    // Main Action Methods
    #region Main Actions

    // is a method is move players to a specific tile
    // called by selection manager returns a bool 
    // to let the selection manager know if
    // it selected a viable tile to move, and can
    // therefore return to the viewing state
    public bool MoveToTile(GameObject newTile)
    {
        TileMapSetup();
        // is new tile in range? 
        List<GameObject> reachableTiles = GetReachableTiles(pilotInfo.GetPilotSpeed());
        if (reachableTiles.Contains(newTile)) {
            hasMovedYet = true;

            currentClarityLevel += pilotInfo.GetMoveClarity();
            currentClarityLevel = Mathf.Min(mechInfo.maximumClarity, currentClarityLevel);
            StartCoroutine(MoveAlongTiles(newTile));
            return true;
        }
        return false;
    }

    // given to use by selection manager once it has chosen a tile to use it on
    // it returns a bool to let the selection manager know if the ability was successfully used
    // and can therefore return back to its viewing state
    public bool UseAbility(MechStats.AbilityMechSlot ability, GameObject targetedTile) {
        // Debug.Log("UseAbility method used in player controller class: " + ability.GetAbilityType().ToString());

        bool abilityUsedCheck = aem.InputAbilityInformationSources(ability, this, targetedTile);
        // Debug.Log("abilityUsedCheck: " + abilityUsedCheck);
        if (abilityUsedCheck) {
            hasAttackedYet = true;
            hasMovedYet = true;
        }
        return abilityUsedCheck;
    }

    public void TakeDamage(int damage, bool isMechDamage) {
        if (isMechDamage) {
            currentMechHealth = Mathf.Clamp(currentMechHealth - damage, 0, RetrieveMechInfo().GetMechHealth());
        } else {
            // Debug.Log("Pilot took damage: Started at " + currentPlayerHealth);
            currentPlayerHealth = Mathf.Clamp(currentPlayerHealth - damage, 0, RetrievePilotInfo().GetPilotHealth());
            // Debug.Log("Pilot took damage: Ended at " + currentPlayerHealth);
        }



        if (currentPlayerHealth == 0 ||currentMechHealth == 0 ) {
            if (isPlayerEntity) {
                combatStateController.IncreaseFriendlyCount(false);
            } else {
                combatStateController.IncreaseFriendlyCount(false);
            }
            SingleSoundPlayer soundPlayer = FindObjectOfType<SoundManager>().GetOrCreateSoundPlayer(deathSound);
            soundPlayer.PlayFromForeignTrigger();
            Destroy(gameObject);
        }
    }

    // called by turn manager. This allows our pieces to
    // re-activate for our turn. 
    public void ResetMoveAndAttackStates() {
        // Debug.Log("reset");
        hasMovedYet = false;
        hasAttackedYet = false;
    }
    #endregion Main Actions
    // Getter Methods
    #region Getters

    public int GetPilotHealth() {
        return currentPlayerHealth;
    }

    public int GetMechHealth() {
        return currentMechHealth;
    }

    public CharacterStats RetrievePilotInfo() {
        return pilotInfo;
    }
    public MechStats RetrieveMechInfo() {
        return mechInfo;
    }

    public GameObject FindMatchingObjectToTile(GameObject newTile) {
        GameObject[] playerObjectPiecesArray = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] obstacleObjectPiecesArray = GameObject.FindGameObjectsWithTag("Obstacle");
        GameObject[] combinedArray = CombineArrays(playerObjectPiecesArray, obstacleObjectPiecesArray);

        foreach (GameObject character in combinedArray)
        {
            bool matchingX = (character.transform.position.x == newTile.transform.position.x);
            bool matchingZ = (character.transform.position.z == newTile.transform.position.z);
            if (matchingX && matchingZ) {
                return character;
            }
        }

        return null;
    }

    public GameObject FindClosestTile(Vector3 point)
    {
        TileMapSetup();
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

    public bool IsTileOccupied(GameObject newTile) {
        TileGraphicsController tgc = newTile.GetComponent<TileGraphicsController>();
        if (tgc.IsShutDown()) {
            return true;
        }

        GameObject[] playerObjectPiecesArray = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] obstacleObjectPiecesArray = GameObject.FindGameObjectsWithTag("Obstacle");
        GameObject[] combinedArray = CombineArrays(playerObjectPiecesArray, obstacleObjectPiecesArray);

        foreach (GameObject character in playerObjectPiecesArray)
        {
            bool matchingX = (character.transform.position.x == newTile.transform.position.x);
            bool matchingZ = (character.transform.position.z == newTile.transform.position.z);
            if (matchingX && matchingZ) {
                return true;
            }
        }


        foreach (GameObject obstacle in obstacleObjectPiecesArray)
        {
            bool matchingX = (obstacle.transform.position.x == newTile.transform.position.x);
            bool matchingZ = (obstacle.transform.position.z == newTile.transform.position.z);
            ObstacleController occupiesTile = obstacle.GetComponent<ObstacleController>();
            if (matchingX && matchingZ && occupiesTile.OccupiesTilesCheck()) {
                return true;
            }
        }

        return false;
    }
    #endregion Getters
    // Setup Methods
    #region Setup 
    private void InstantiateMechObject() {
        Vector3 selfPosition = gameObject.transform.position;
        Quaternion selfRotation = gameObject.transform.rotation;
        GameObject prefabToSummon = RetrieveMechInfo().GetMechGFXPrefab();
        if (prefabToSummon != null) {
            GameObject mech = Instantiate(prefabToSummon, selfPosition, selfRotation);
            mech.transform.parent = gameObject.transform;
            GetComponent<MeshRenderer>().enabled = false;
        }

    }
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
        currentPlayerHealth = RetrievePilotInfo().GetPilotHealth();
        currentMechHealth =  RetrieveMechInfo().mechHealth;
    }

    #endregion Setup 
    // Pathing Methods
    #region Pathing
    private IEnumerator MoveAlongTiles(GameObject destinationTile) // IF YOU MOVE AND THEN TELEPORT THIS STILL OPERATES.... 
    {
        List<GameObject> path = FindPath(FindClosestTile(transform.position), destinationTile);

        foreach (GameObject tile in path)
        {
            agent.SetDestination(tile.transform.position + Vector3.up * yOffset);
            yield return new WaitUntil(() => agent.remainingDistance < 0.1f);
        }

        //  Debug.Log("Reached the intended tile: " + destinationTile.name);
        CheckForSteppingOnObstacleTile(destinationTile);
    }

    private void CheckForSteppingOnObstacleTile(GameObject tileToCheck) {
        GameObject[] obstacleObjectPiecesArray = GameObject.FindGameObjectsWithTag("Obstacle");

        Vector3 tilePos = tileToCheck.transform.position;
        foreach(GameObject obstacle in obstacleObjectPiecesArray) {
            Vector3 obstPos = obstacle.transform.position;
            if (obstPos.x == tilePos.x && obstPos.z == tilePos.z) {
                obstacle.GetComponent<ObstacleController>().TriggerInteraction(this);
            }
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

    // AI Pathing Methods
    public int GetTileDistance(GameObject startTile, GameObject endTile)
    {
        TileMapSetup();
        Vector3 start = startTile.transform.position;
        Vector3 end = endTile.transform.position;
        
        // Calculate the Manhattan distance
        int dx = Mathf.Abs(Mathf.RoundToInt(end.x - start.x));
        int dz = Mathf.Abs(Mathf.RoundToInt(end.z - start.z));
        
        return dx + dz;
    }

    public int GetTileDistanceWithObstacles(GameObject startTile, GameObject endTile)
    {
        Queue<(GameObject tile, int distance)> queue = new Queue<(GameObject, int)>();
        HashSet<GameObject> visited = new HashSet<GameObject>();

        queue.Enqueue((startTile, 0));
        visited.Add(startTile);

        while (queue.Count > 0)
        {
            var (currentTile, currentDistance) = queue.Dequeue();

            if (currentTile == endTile)
            {
                return currentDistance;
            }

            foreach (GameObject neighbor in GetNeighbors(currentTile))
            {
                // if (!visited.Contains(neighbor) && !IsTileOccupied(neighbor))
                if (!visited.Contains(neighbor) && (!IsTileOccupied(neighbor) || neighbor == endTile))
                {
                    queue.Enqueue((neighbor, currentDistance + 1));
                    visited.Add(neighbor);
                }
            }
        }

        // If no path is found, return a very large number or -1 to indicate impossibility
        return int.MaxValue;
    }

    // Method to find the best reachable tile closest to the target
    public GameObject GetBestReachableTileTowardsTarget(GameObject targetTile, int maxDistance)
    {
        TileMapSetup();
        List<GameObject> reachableTiles = GetReachableTiles(maxDistance);
        GameObject currentTile = FindClosestTile(transform.position);
        GameObject bestTile = currentTile;
        int shortestDistance = GetTileDistanceWithObstacles(currentTile, targetTile);

        foreach (GameObject tile in reachableTiles)
        {
            // int distance = GetTileDistance(tile, targetTile);
            int distance = GetTileDistanceWithObstacles(tile, targetTile);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                bestTile = tile;
            }
        }

        return bestTile;
    }


    // Method to combine two GameObject arrays
    GameObject[] CombineArrays(GameObject[] array1, GameObject[] array2)
    {
        // Create a new array to hold the combined elements
        GameObject[] combinedArray = new GameObject[array1.Length + array2.Length];

        // Copy elements from the first array
        array1.CopyTo(combinedArray, 0);

        // Copy elements from the second array
        array2.CopyTo(combinedArray, array1.Length);

        return combinedArray;
    }
    #endregion Pathing

}