using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public int maxHealth = 1;
    public int currentHealth = 0;
    public float yOffset = 1f;
    private List<GameObject> tiles;
    [SerializeField]
    private ObstacleType obstacleType = ObstacleType.Destructable;
    [SerializeField]
    private bool occupiesTiles = true;
    [SerializeField]
    private int damageOnTouch = 2;

    void Start()
    {
        if (occupiesTiles) {
            SetTagToObstacle();
        } else {
            
        }

        currentHealth = maxHealth;

        if (!IsTargetable()) {
            GameObject closestTile = FindClosestTile(transform.position);
            closestTile.GetComponent<TileGraphicsController>().ShutDown();
        } else {
            PositionalCorrectionSetup();
        }
    }

    public void TakeDamage(int damageToTake) {
        currentHealth -= damageToTake;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        currentHealth = Mathf.Max(0, currentHealth);

        if (currentHealth == 0) {
            Destroy(gameObject);
        }

    }

    public bool IsTargetable() {
        return obstacleType == ObstacleType.Destructable;
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

    public enum ObstacleType {
        Destructable,
        Indestructable,
        SpaceFiller
    }

    public void TriggerInteraction(PlayerController pc) {
        if (occupiesTiles || damageOnTouch == 0) {
            return;
        }
        pc.TakeDamage(damageOnTouch, true);
        Destroy(gameObject);
    }

    public bool OccupiesTilesCheck() {
        return occupiesTiles;
    }
 
}
