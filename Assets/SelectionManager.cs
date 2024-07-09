using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    // List to store all the GameObjects with the "Tile" tag
    public List<GameObject> tiles;
    public float minSelectDistance;
    public GameObject currentSelectedTile;
    public GameObject currentHoverableTile;
    public GameObject currentSelectCharacter;
    private CharacterCanvasController ccc;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the list
        tiles = new List<GameObject>();
        ccc = FindObjectOfType<CharacterCanvasController>();

        // Find all GameObjects with the tag "Tile" and add them to the list
        GameObject[] tilesArray = GameObject.FindGameObjectsWithTag("Tile");
        foreach (GameObject tile in tilesArray)
        {
            tiles.Add(tile);
        }

        // Optional: Output the number of tiles found
        Debug.Log("Number of tiles found: " + tiles.Count);
    }


    void Update()
    {
        MouseSelect();
        HoverSelect();
    }

    private void HoverSelect() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {   
            // Debug.Log("hitpoint: " + hit.point);
            if (currentHoverableTile != null) {
               currentHoverableTile.GetComponent<TileGraphicsController>().UnHoverState();
               Debug.Log("called unhover from main");
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
                

                // if currentSelectCharacter is null we just do selection, because means we're not moving a character
                if (currentSelectCharacter == null) {

                    // SelectTile(hit.point);
                    SelectNewTile(hit.point);
                } else {
                    
                    if (currentSelectCharacter.GetComponent<PlayerController>().isPlayerEntity) {
                        MoveSelectedCharacter(hit.point);
                    } else {
                        currentSelectedTile.GetComponent<TileGraphicsController>().ChangeToDefaultState();
                        currentSelectCharacter = null;
                        currentSelectedTile = null;
                    }
                    ccc.MenuCleanup();
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
            DisplayerInfoToUI();
        }
    }

    private void DisplayerInfoToUI() {
        if (currentSelectCharacter != null) {
            PlayerController characterScript = currentSelectCharacter.GetComponent<PlayerController>();
            ccc.DisplayCharacter(characterScript);
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


        GameObject possibleTileOccupyingObject = FindMatchingObjectToTile();
        if (possibleTileOccupyingObject == null) {
            // move player
            PlayerController pc = currentSelectCharacter.GetComponent<PlayerController>();
            pc.MoveToNewTile(currentSelectedTile);
        }


        // clean up even if we just chose an occupied tile
        // if you want to change this, move it into the if statement above
        currentSelectCharacter = null;
        currentSelectedTile = null;
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

    private void ChangeChildrenColor(GameObject parentObject, Color newColor) {
        List<GameObject> children = GetAllChildren(parentObject);
        foreach (GameObject child in children)
        {
            // Debug.Log("Child: " + child.name);
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr != null) {
                sr.color = newColor;
            }
        }
    }

    private List<GameObject> GetAllChildren(GameObject parent)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in parent.transform)
        {
            children.Add(child.gameObject);
            children.AddRange(GetAllChildren(child.gameObject)); // Recursively add all children
        }
        return children;
    }

    GameObject FindClosestTile(Vector3 point)
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
