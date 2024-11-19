using System;
using System.Collections.Generic;
using UnityEngine;

public class ScrapController : MonoBehaviour
{
    [SerializeField] private ScrapManager scrapManager;
    [SerializeField] private List<PlayerController> enemyMecha;
    [SerializeField] private List<PlayerController> playableMecha;
    [SerializeField] private float scrapThreshold = 0.5f;
    [SerializeField] private float scrapConversion = 0.25f;
    private int scrapAvailable;
    
    // Start is called before the first frame update
    void Start()
    {
        CollectEnemyMecha();
    }

    private void CollectEnemyMecha()
    {
        GameObject[] boardEntities = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject entity in boardEntities)
        {
            PlayerController playerController = entity.GetComponent<PlayerController>();
            if (playerController.isPlayerEntity == false)
            {
                enemyMecha.Add(playerController);
            }
        }
    }
    
    // Updates the dictionary with a new mech and its corresponding scrap value
    private void AddScrap(int currentScrapAvailable, int scrapToAdd)
    {
        scrapManager.SetScrapAvailable(currentScrapAvailable += scrapToAdd);
    }
    
    // Removes a mech from the scrap available
    public void SubtractScrap(int currentScrapAvailable, int scrapToSubtract)
    {
        if (currentScrapAvailable - scrapToSubtract < 0)
        {
            Debug.LogError("Not enough scrap available for this purchase.");
        }
        else
        {
            scrapManager.SetScrapAvailable(currentScrapAvailable += scrapToSubtract);
        }
    }

    // Calculates scrap based on the mech's current health 
    private int CalculateScrapValue(PlayerController mech)
    {
        return (int)Math.Floor(mech.GetMechHealth() * scrapConversion);
    }

    // Once Combat ends, this method is called in order to calculate the scrap available
    public void SetScrapPostCombat()
    {
        foreach (PlayerController enemyMech in enemyMecha)
        {
            int mechHealth = enemyMech.GetMechHealth();

            if (mechHealth == 0) // mech is dead 
            {
                Debug.LogError("This mech cannot be used as scrap or as a part of the team");
            } else if (mechHealth < enemyMech.GetMechMaxHealth() * scrapThreshold) // mech has low health
            {
                Debug.LogError("This mech can only be used as scrap.");
                AddScrap(scrapManager.GetScrapAvailable(), CalculateScrapValue(enemyMech));
                // TODO: should also update "units available to use during combat" list as well
            } else if (mechHealth > enemyMech.GetMechMaxHealth() * scrapThreshold) // mech has decent health
            {
                Debug.LogError("This mech can be used as scrap or on the team.");
                playableMecha.Add(enemyMech);
            }
        }
    }

    // Allows you to use a mech as scrap instead of using it during Combat later on
    public void UseAsScrap(PlayerController mech)
    {
        SubtractScrap(scrapManager.GetScrapAvailable(), CalculateScrapValue(mech));
        // TODO: should also update "units available to use during combat" list as well
    }
} 
