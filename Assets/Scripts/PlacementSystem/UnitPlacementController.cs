using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class UnitPlacementController : MonoBehaviour
{
    [SerializeField] private GameObject placementFramePrefab;
    [SerializeField] private GameObject frameParent;

    [SerializeField] private GameObject currentHoverableTile;
    [SerializeField] private PlacementOptionFrame currentOptionScript;


    [SerializeField] private float minSelectDistance = 0.25f;







    // we dont have to serialize these parts in the inspector
    [SerializeField] private List<TeamChooserController.TeamSpot> teamSpots = new List<TeamChooserController.TeamSpot>();
    
    
    


    private List<PlacementOptionFrame> frameOptionList = new List<PlacementOptionFrame>();
    private List<GameObject> tiles = new List<GameObject>();
    private List<GameObject> preCombatTileList = new List<GameObject>();
    private EventSystem eventSystem;

    public void InitializeFromTeamRosterPersistor(List<TeamChooserController.TeamSpot> newTeam)  {
        PauseThroughCombatManager(true);
        // delete children of the array parrent GameObject
        // so we can make new ones
        foreach (Transform child in frameParent.transform)
        {
            Destroy(child.gameObject);
        }

        // teamSpots
        foreach(TeamChooserController.TeamSpot spot in newTeam) {
            if (spot.chosenPilot == null || spot.chosenMech == null) {
                return;
            }

            // make the object
            GameObject newFrameObject = Instantiate(placementFramePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            newFrameObject.transform.SetParent(frameParent.transform);
            // make the script initialize
            PlacementOptionFrame frameScript = newFrameObject.GetComponent<PlacementOptionFrame>();
            frameScript.InitializeFromPlacementController(spot.chosenPilot, spot.chosenMech);
            frameOptionList.Add(frameScript);
        }
    }

    void Start()
    {
        eventSystem = EventSystem.current;

        // Find all GameObjects with the tag "Tile" and add them to the list
        GameObject[] tilesArray = GameObject.FindGameObjectsWithTag("Tile");
        foreach (GameObject tile in tilesArray)
        {
            tiles.Add(tile);
        }

        List<GameObject> markerList = GameObject.FindGameObjectsWithTag("PreCombatTileMarker").ToList();

        foreach(GameObject marker in markerList) {
            GameObject preCombatTile = FindClosestTile(marker.transform.position);
            if (preCombatTile != null) {
                preCombatTileList.Add(preCombatTile);
            }
        }


        foreach (GameObject tile in tiles)
        {
            if (!preCombatTileList.Contains(tile))
            {
                tile.SetActive(false);
            }
        }
    }

    // called by UI PlacementOptionFrame.cs
    public void SelectNewPlacementOption(PlacementOptionFrame newOption) {
        currentOptionScript = newOption;
    }

    private void HoverSelect() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {   
            // Debug.Log("hitpoint: " + hit.point);
            if (currentHoverableTile != null) {
               currentHoverableTile.GetComponent<TileGraphicsController>().UnHoverState();
            //    Debug.Log("called unhover from main");
            }
            currentHoverableTile = FindClosestTile(hit.point);
            if (currentHoverableTile != null) {
                currentHoverableTile.GetComponent<TileGraphicsController>().ChangeToHoveringState();
            }

    
        }
    }

    private void MouseSelect() {
        // Check if the left mouse button was pressed
        if (Input.GetMouseButtonDown(0))
        {
            // Create a ray from the camera going through the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Perform the raycast
            if (Physics.Raycast(ray, out hit))
            {

                // Check if the mouse is over a UI element
                if (eventSystem.IsPointerOverGameObject())
                {
                    // Mouse is over UI, don't process tile selection
                    return;
                }

                // if we hit and have a selected placement option frame, 
                // then we place the item there automatically and then clean up  
                if (currentHoverableTile != null && currentOptionScript != null) {
                    


                    currentOptionScript.PlaceTilePiece(currentHoverableTile);
                    currentOptionScript.Deselect();
                    currentOptionScript = null;
                }

                StartGameCheck();
            }
        }
    }

    private void StartGameCheck()  {
        Debug.Log("StartGameCheck method called");

        bool canstartGame = HaveAllFramesBeenUsedCheck();
        // Debug.Log("canstartGame method return is: " + canstartGame);
        if (canstartGame) {
            foreach (GameObject tile in tiles)
            {
                tile.SetActive(true);
            }
            Debug.Log("canstartGame method return is true");
            PauseThroughCombatManager(false);
            gameObject.SetActive(false);
        }


    }

    private bool HaveAllFramesBeenUsedCheck() {
        foreach(PlacementOptionFrame frame in frameOptionList) {
            if (!frame.UseCheck()) {
                return false;
            }
        }

        return true;
    }

    private void PauseThroughCombatManager(bool shouldPause) {
        CombatStateController combatStateController = FindObjectOfType<CombatStateController>();
        if (combatStateController == null) {
            Debug.Log("PauseThroughCombatManager method called from unit placement controller");
            combatStateController = new GameObject("CombatStateController").AddComponent<CombatStateController>();
            // combatStateController.PauseOrResumeCombat(shouldPause, gameObject);
        }
        combatStateController.PauseOrResumeCombat(shouldPause, gameObject);
    }
    

    public void Update() {
        HoverSelect();
        MouseSelect();

    }













    // helper methods, exactly like in selection manager. This is duplicate code
    private GameObject FindClosestTile(Vector3 point)
    {
        ExistenceCheck();
        GameObject closestTile = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (GameObject tile in tiles)
        {
            Vector3 directionToTile = tile.transform.position - point;
            float dSqrToTile = directionToTile.sqrMagnitude; // Use sqrMagnitude to avoid the cost of a square root calculation

            if (dSqrToTile < closestDistanceSqr && ContainsTargetableTile(tile))
            {
                closestDistanceSqr = dSqrToTile;
                closestTile = tile;
            }
        }

        if (Vector3.Distance(point, closestTile.transform.position) > minSelectDistance) {
            return null;
        }
        return closestTile;
    }

    private void ExistenceCheck() {
        // this script checks if this is a combat scene
        // if not, delete. I made this when I'm pretty tired. instead just find the gamestatemanager, 
        // its not a combat scene, delete this shit. And the teampersistor

        GameObject tileObject = GameObject.FindGameObjectWithTag("Tile");
        if (tileObject == null) {
            Destroy(gameObject);
        }
    }

    private bool ContainsTargetableTile(GameObject tileObject) {
        GameObject[] obstacleObjectArray = GameObject.FindGameObjectsWithTag("Obstacle");

        Vector3 tilePos = tileObject.transform.position;
        foreach(GameObject obstacle in obstacleObjectArray) {
            Vector3 obstPos = obstacle.transform.position;
            if (obstPos.x == tilePos.x && obstPos.z == tilePos.z) {
                ObstacleController obstController = obstacle.GetComponent<ObstacleController>();
                if (!obstController.IsTargetable()) {
                    return false;
                }
            }
        }

        return true;
    }
}
