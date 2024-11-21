using System;
using System.Collections.Generic;
using UnityEngine;

public class ScrapController : MonoBehaviour
{
    [SerializeField] private ScrapManager scrapManager;
    [SerializeField] private MechSaveFileInteractor saveFileInteractor;
    [SerializeField] private List<PlayerController> enemyMecha;
    [SerializeField] private List<PlayerController> playableMecha;
    [SerializeField] private float scrapThreshold = 0.5f;
    [SerializeField] private float scrapMultiplier = 0.25f;
    private int scrapAvailable;
    
    // Start is called before the first frame update
    void Start()
    {
        CollectEnemyMecha();
    }

    private void CollectEnemyMecha()
    {
        // There might be non-player controlled allies to be aware of later on. 
        
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
    private void AddScrap(int totalScrap, int scrapToAdd)
    {
        scrapManager.SetScrapAvailable(totalScrap += scrapToAdd);
    }
    
    // Removes a mech from the scrap available
    public void SubtractScrap(int totalScrap, int scrapToSubtract)
    {
        if (totalScrap - scrapToSubtract < 0)
        {
            Debug.LogError("Not enough scrap available for this purchase.");
        }
        else
        {
            scrapManager.SetScrapAvailable(totalScrap += scrapToSubtract);
        }
    }

    // Calculates scrap based on the mech's current health 
    private int CalculateScrapValue(PlayerController mech)
    {
        return (int)Math.Floor(mech.GetMechHealth() * scrapMultiplier);
    }

    // Once Combat ends, this method is called in order to calculate the scrap available
    public void SetScrapPostCombat()
    {
        foreach (PlayerController enemyMech in enemyMecha)
        {
            if (enemyMech.currentMechHealth == 0) // mech is dead 
            {
                Debug.LogError("This mech cannot be used as scrap or as a part of the team");
            } else if (enemyMech.currentPlayerHealth == 0 && enemyMech.currentMechHealth > 0) // pilot is dead
            {
                Debug.LogError("This mech can be used as scrap or on the team.");
                saveFileInteractor.LogMechStatsToFile(enemyMech); // mech is saved as playable on your team now
            }
        }
    }

    // Allows you to use a mech as scrap instead of using it during Combat later on
    public void UseAsScrap(PlayerController mech)
    {
        AddScrap(scrapManager.GetScrapAvailable(), CalculateScrapValue(mech));
        mech.currentMechHealth = 0; // now this mech won't be available to use during combat
    }
    
    // Allows you to make repairs to damaged mechs
    public void MakeRepairs(PlayerController mech)
    {
        SubtractScrap(scrapManager.GetScrapAvailable(), CalculateScrapValue(mech));
    }
} 
