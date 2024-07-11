using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    // List to store all the GameObjects with the "Tile" tag
    public List<GameObject> tiles;
    public float minSelectDistance;
    public GameObject currentHoverableTile;
    public GameObject currentSelectedTile;
    public GameObject currentSelectCharacter;
    public GameObject currentSelectAbility; // UPDATE THIS TO MECHSTAT ABILITY CLASS
    public List<GameObject> tileRangeList;
    private CharacterCanvasController ccc;
    private TurnManager tm;

    private EventSystem eventSystem;

    public enum CurrentCharacterSelectionStatus {
        Viewing,
        Moving,
        Attacking,
        UsingAbility
    }

    public CurrentCharacterSelectionStatus selectionState = CurrentCharacterSelectionStatus.Viewing;

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = EventSystem.current;

        // Initialize the list
        tiles = new List<GameObject>();
        ccc = FindObjectOfType<CharacterCanvasController>();
        tm = FindObjectOfType<TurnManager>();

        // Find all GameObjects with the tag "Tile" and add them to the list
        GameObject[] tilesArray = GameObject.FindGameObjectsWithTag("Tile");
        foreach (GameObject tile in tilesArray)
        {
            tiles.Add(tile);
        }

        // Optional: Output the number of tiles found
        // Debug.Log("Number of tiles found: " + tiles.Count);
    }


    void Update()
    {
        // Change to MouseClick instead, more descriptive method name
        MouseSelect();
        HoverSelect();

        EscapeButtonControls();
    }

    private void EscapeButtonControls() {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cleanup();
            ccc.MenuCleanup();
        }
    }

    // Interaction Methods
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


                if (selectionState == CurrentCharacterSelectionStatus.Viewing) {
                    SelectNewTile(hit.point);
                } else if (selectionState == CurrentCharacterSelectionStatus.Moving) {
                    if (currentSelectCharacter.GetComponent<PlayerController>().isPlayerEntity) {
                        MoveSelectedCharacter(hit.point);
                    } else {
                        currentSelectedTile.GetComponent<TileGraphicsController>().ChangeToDefaultState();
                        Cleanup();
                    }
                    ccc.MenuCleanup();
                } else if (selectionState == CurrentCharacterSelectionStatus.Attacking) {

                    HavePlayerAttack(hit.point);
                }
                

            }
        }
    }

    private void SelectNewTile(Vector3 selectionPoint) {
        // SelectTile(hit.point);
        if (currentSelectedTile != null) {
            currentSelectedTile.GetComponent<TileGraphicsController>().ChangeToDefaultState();
        }
        currentSelectedTile = FindClosestTile(selectionPoint);
        if (currentSelectedTile == null) {
            return;
        }
        currentSelectedTile.GetComponent<TileGraphicsController>().ChangeToSelectedState();

        if (currentSelectedTile != null) {
            currentSelectCharacter = FindMatchingObjectToTile();
            if (currentSelectCharacter != null) {
                DisplayerInfoToUI();
            } else {
                ccc.MenuCleanup();
            }
        }
    }


    // Action Type Methods
    private void HavePlayerAttack(Vector3 targetPosition) {

        GameObject newTile = FindClosestTile(targetPosition);
        if (newTile == null) {
            return;
        } else {
            // pre clean up
            currentSelectedTile.GetComponent<TileGraphicsController>().ChangeToDefaultState();

            bool AttackSuccessful = currentSelectCharacter.GetComponent<PlayerController>().AttackTile(newTile);

            if (AttackSuccessful) {
                Cleanup();
                ccc.MenuCleanup();
            }

            
        }


    }

    private void MoveSelectedCharacter(Vector3 selectionPoint) {
        // pre clean up
        currentSelectedTile.GetComponent<TileGraphicsController>().ChangeToDefaultState();

        // new selection
        currentSelectedTile = FindClosestTile(selectionPoint);
        if (currentSelectedTile == null) {
            return;
        }

        bool shouldUseTurnManager = false;

        GameObject possibleTileOccupyingObject = FindMatchingObjectToTile();
        if (possibleTileOccupyingObject == null) {
            // move player
            PlayerController pc = currentSelectCharacter.GetComponent<PlayerController>();
            pc.MoveToNewTile(currentSelectedTile);
            shouldUseTurnManager = true;
        }

        if (shouldUseTurnManager) {
            tm.BeginWait(currentSelectCharacter, currentSelectedTile);

        }

        // clean up even if we just chose an occupied tile
        // if you want to change this, move it into the if statement above
        Cleanup();

    }


    // State Machine Altering Methods
    public void ChangeToMovingState() {
        selectionState = CurrentCharacterSelectionStatus.Moving;
        PlayerController playerScript = currentSelectCharacter.GetComponent<PlayerController>();
        List<GameObject> reachableTiles = playerScript.GetReachableTiles(playerScript.RetrievePilotInfo().speed);
        ClearTileRange();
        tileRangeList = reachableTiles;
        foreach (GameObject tile in reachableTiles) {
            tile.GetComponent<TileGraphicsController>().ChangeToWalkableState();
        }
    }

    public void ChangeToViewingState() {
        selectionState = CurrentCharacterSelectionStatus.Viewing;
    }


    public void ChangeToAttackingState() {
        selectionState = CurrentCharacterSelectionStatus.Attacking;
        PlayerController playerScript = currentSelectCharacter.GetComponent<PlayerController>();
        List<GameObject> reachableTiles = playerScript.GetAttackableTiles(playerScript.RetrievePilotInfo().attackRange);
        ClearTileRange();
        tileRangeList = reachableTiles;
        foreach (GameObject tile in reachableTiles) {
            tile.GetComponent<TileGraphicsController>().ChangeToAttackableState();
        }
    }

    public void ChangeToAbilityState(int abilitySlot) {
        selectionState = CurrentCharacterSelectionStatus.UsingAbility;
        PlayerController playerScript = currentSelectCharacter.GetComponent<PlayerController>();
        List<GameObject> reachableTiles = new List<GameObject>();
        ClearTileRange();
        if (abilitySlot == 1) {
            reachableTiles = playerScript.GetReachableTiles(playerScript.RetrieveMechInfo().AbilitySlot1.maximumRange);
            tileRangeList = reachableTiles;
        } else if (abilitySlot == 2) {
            reachableTiles = playerScript.GetReachableTiles(playerScript.RetrieveMechInfo().AbilitySlot2.maximumRange);
            tileRangeList = reachableTiles;
        } else if (abilitySlot == 3) {
            reachableTiles = playerScript.GetReachableTiles(playerScript.RetrieveMechInfo().AbilitySlot3.maximumRange);
            tileRangeList = reachableTiles;
        } else {
            Debug.LogError("ChangeToAbilityState method given a number not matchable to an ability slot");
        }

        foreach (GameObject tile in reachableTiles) {
            tile.GetComponent<TileGraphicsController>().ChangeToAbilityState();
        }
    }




    private void DisplayerInfoToUI() {
        if (currentSelectCharacter != null) {
            PlayerController characterScript = currentSelectCharacter.GetComponent<PlayerController>();
            ccc.DisplayCharacter(characterScript);
        }
    }

    private GameObject FindMatchingObjectToTile() {
        GameObject[] playerObjectPiecesArray = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject character in playerObjectPiecesArray)
        {
            bool matchingX = (character.transform.position.x == currentSelectedTile.transform.position.x);
            bool matchingZ = (character.transform.position.z == currentSelectedTile.transform.position.z);
            if (matchingX && matchingZ) {
                return character;
            }
        }

        return null;
    }

    // Cleanup Methods
    private void Cleanup () {
        if (currentSelectedTile != null) {
            currentSelectedTile.GetComponent<TileGraphicsController>().ChangeToDefaultState();
        }
        selectionState = CurrentCharacterSelectionStatus.Viewing;
        currentSelectCharacter = null;
        currentSelectedTile = null;

        currentHoverableTile.GetComponent<TileGraphicsController>().ChangeToDefaultState();
        currentHoverableTile = null;

        ClearTileRange();
    }

    private void ClearTileRange() {
        if (tileRangeList != null) {
            if (tileRangeList.Count != 0) {

                foreach (GameObject tile in tileRangeList) {
                    tile.GetComponent<TileGraphicsController>().ChangeToDefaultState();
                }
            }
        }

        tileRangeList = new List<GameObject>();
    }



    // Helper Methods
    private GameObject FindClosestTile(Vector3 point)
    {
        GameObject closestTile = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (GameObject tile in tiles)
        {
            Vector3 directionToTile = tile.transform.position - point;
            float dSqrToTile = directionToTile.sqrMagnitude; // Use sqrMagnitude to avoid the cost of a square root calculation

            if (dSqrToTile < closestDistanceSqr)
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

}
