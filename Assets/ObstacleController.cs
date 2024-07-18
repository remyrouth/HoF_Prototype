using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public int maxHealth = 1;
    public int currentHealth = 0;
    public int yOffset = 1;
    private List<GameObject> tiles;

    void Start()
    {
        SetTagToObstacle();
        PositionalCorrectionSetup();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageToTake) {
        currentHealth -= damageToTake;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        currentHealth = Mathf.Max(0, currentHealth);

        if (currentHealth == 0) {
            Destroy(gameObject);
        }

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

    private void SetTagToObstacle() {
        gameObject.tag = "Obstacle";
    }

}
