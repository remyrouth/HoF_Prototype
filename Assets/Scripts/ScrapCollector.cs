using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapCollector : MonoBehaviour
{
    [SerializeField] private ScrapManager scrapManager;
    [SerializeField] private List<PlayerController> enemyMecha;
    [SerializeField] private List<PlayerController> highHealthMecha;
    private int scrapAvailable;
    
    // Start is called before the first frame update
    void Start()
    {
        CollectEnemyMecha();
    }

    private void Update()
    {
        scrapAvailable = scrapManager.GetScrapAvailable();
    }

    public void CollectEnemyMecha()
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
    private void AddScrap(int scrapValue)
    {
        scrapManager.SetScrapAvailable(scrapAvailable += scrapValue);
    }
    
    // Removes a mech from the scrap available
    public void SubtractScrap(int scrapValue)
    {
        if (scrapAvailable - scrapValue < 0)
        {
            Debug.LogError("Not enough scrap available for this purchase.");
        }
        else
        {
            scrapManager.SetScrapAvailable(scrapAvailable += scrapValue);
        }
    }

    // Calculates scrap based on the mech's current health 
    private int CalculateScrapValue(PlayerController mech)
    {
        return (int)Math.Floor(mech.GetMechHealth() * .25);
    }

    // Once Combat ends, this method is called in order to calculate the scrap available
    public void SetScrapPostCombat()
    {
        foreach (PlayerController enemyMech in enemyMecha)
        {
            int mechHealth = enemyMech.GetMechHealth();

            if (mechHealth == 0)
            {
                Debug.LogError("This mech cannot be used as scrap or as a part of the team");
            } else if (mechHealth < enemyMech.GetMechMaxHealth() * .5)
            {
                Debug.LogError("This mech can only be used as scrap.");
                AddScrap(CalculateScrapValue(enemyMech));
                // TODO: should also update "units available to use during combat" list as well
            } else if (mechHealth > enemyMech.GetMechMaxHealth() * .5)
            {
                Debug.LogError("This mech can be used as scrap or on the team.");
                highHealthMecha.Add(enemyMech);
            }
        }
    }

    // Allows you to use a mech as scrap instead of using it during Combat later on
    public void UseAsScrap(PlayerController mech)
    {
        SubtractScrap(CalculateScrapValue(mech));
        // TODO: should also update "units available to use during combat" list as well
    }
} 
