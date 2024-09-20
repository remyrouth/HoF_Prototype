using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AfterActionReportController : MonoBehaviour
{
    [SerializeField] private GameObject gameObject;
    [SerializeField] private Text casualtyReport; 
    [SerializeField] private Text friendlysList; 
    
    void Start()
    {
        CombatStateController combatStateController = FindObjectOfType<CombatStateController>();
        if (combatStateController == null) {
            Debug.Log("Start method called from unit After Action Report controller");
            combatStateController = new GameObject("CombatStateController").AddComponent<CombatStateController>();
            // combatStateController.PauseOrResumeCombat(shouldPause, gameObject);
        }
        int casualties = combatStateController.FinalFriendlyCasualtyCount();
        casualtyReport.text = "Casualties: " + casualties.ToString();
        
        /*
         * Currently represents total Friendly (USA) casualties.
         * Enemy (Principality of America) are not in this representation.
         * Possible things to rep for dogtag-esque death report: DOB, Residence, Marital Status, Next of Kin, Origin, etc.
         * Pay mortuary fee option: two buttons (pay, dont)
         */
        
        // Use same CSC to report list of deceased. 
        List<string> friendlyDeceasedNames = combatStateController.DeceasedFriendlyList();
        string namesText = "None";
        if (friendlyDeceasedNames != null)
        {
            namesText = string.Join(", ", friendlyDeceasedNames); 
        }
        friendlysList.text = "The deceased: " + namesText;
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