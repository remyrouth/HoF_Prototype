using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AfterActionReportController : MonoBehaviour
{
    [SerializeField] private GameObject gameObject;
    [SerializeField] private Text casualtyReport; 
    
    void Start()
    {
        CombatStateController combatStateController = FindObjectOfType<CombatStateController>();
        if (combatStateController == null) {
            Debug.Log("Start method called from unit After Action Report controller");
            combatStateController = new GameObject("CombatStateController").AddComponent<CombatStateController>();
            // combatStateController.PauseOrResumeCombat(shouldPause, gameObject);
        }
        // Currently represents total Friendly casualties.
        int casualties = combatStateController.FinalFriendlyCasualtyCount();
        casualtyReport.text = "Casualties: " + casualties.ToString();
    }
        

    // Unsure if this method is needed.
    // Currently displayed through AARCasualty Text and Start (replica of DisplayCasualties).
    public void DisplayCasualties()
    {
        CombatStateController combatStateController = FindObjectOfType<CombatStateController>();
        if (combatStateController == null) {
            Debug.Log("DisplayCasualties method called from unit After Action Report controller");
            combatStateController = new GameObject("CombatStateController").AddComponent<CombatStateController>();
            // combatStateController.PauseOrResumeCombat(shouldPause, gameObject);
        }
        int casualties = combatStateController.FinalFriendlyCasualtyCount();
        casualtyReport.text = "Casualties: " + casualties.ToString();
    }
    
    // TellGameManagerPause: Tell game manager to pause in case of (starting) AAR (or something else?)
    private void PauseThroughCombatManager(bool shouldPause) {
        CombatStateController combatStateController = FindObjectOfType<CombatStateController>();
        if (combatStateController == null) {
            Debug.Log("PauseThroughCombatManager method called from After Action Report controller");
            combatStateController = new GameObject("CombatStateController").AddComponent<CombatStateController>();
            // combatStateController.PauseOrResumeCombat(shouldPause, gameObject);
        }
        combatStateController.PauseOrResumeCombat(shouldPause, gameObject);
    }
    
}