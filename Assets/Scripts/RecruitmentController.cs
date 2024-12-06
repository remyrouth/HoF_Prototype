using UnityEngine;
using System.Collections.Generic;

public class RecruitmentController : MonoBehaviour
{
    [SerializeField] private RecruitmentPoints recruitmentPointManager;
    private GenericAbilityStrategy abilityStrategy;
    // [SerializeField] private PilotSaveFileInteractor saveFileInteractor;
    
    [SerializeField] private List<PlayerController> enemyPilots;
    [SerializeField] private List<PlayerController> playablePilots;
    private int recruitmentPointsAvailable;
    private bool pilotHailed = false;
    
    // Start is called before the first frame update
    void Start()
    {
        CollectEnemyPilots();
    }
    
    private void CollectEnemyPilots()
    {
        GameObject[] boardEntities = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject entity in boardEntities)
        {
            PlayerController playerController = entity.GetComponent<PlayerController>();
            if (playerController.isPlayerEntity == false)
            {
                enemyPilots.Add(playerController);
            }
        }
    }
    
    // Increases & updates the current recruitment points value
    private void AddPoints(int totalPoints, int pointsToAdd)
    {
        recruitmentPointManager.SetRecruitmentPointsAvailable(totalPoints + pointsToAdd);
    }
    
    // Decreases & updates the current recruitment points value
    private void SubtractPoints(int totalPoints, int pointsToSubtract)
    {
        if (totalPoints - pointsToSubtract < 0)
        {
            Debug.LogError("Not enough recruitment points available for this purchase.");
        }
        else
        {
            recruitmentPointManager.SetRecruitmentPointsAvailable(totalPoints += pointsToSubtract);
        }
    }
    
    // Determines if a player will defect depending on their resistance to being hailed
    private bool CanPilotBeHailed(PlayerController unit)
    {
        return unit.pilotInfo.GetHailResistance() > 50;
    }
    
    // Once Combat ends, this method is called in order to calculate the scrap available
    public void SetRecruitmentPointsPostCombat()
    {
        // TODO: finish
    }
}
