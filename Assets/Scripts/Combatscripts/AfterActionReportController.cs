using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AfterActionReportController : MonoBehaviour
{
    [SerializeField] private GameObject gameObject;
    [SerializeField] private Text casualtyReport; 
    [SerializeField] private Text indivCasualtyList;
    
    void Start()
    {
        CombatStateController combatStateController = FindObjectOfType<CombatStateController>();
        if (combatStateController == null) {
            Debug.Log("Start method called from unit After Action Report controller");
            combatStateController = new GameObject("CombatStateController").AddComponent<CombatStateController>();
            // combatStateController.PauseOrResumeCombat(shouldPause, gameObject);
        }

        // Retrieve casualties list
        List<CharacterStats> friendlyCasualties = combatStateController.DeceasedFriendlyList();
        List<CharacterStats> enemyCasualties = combatStateController.DeceasedEnemyList();
        if (friendlyCasualties == null) { friendlyCasualties = new List<CharacterStats>(); }
        if (enemyCasualties == null) { enemyCasualties = new List<CharacterStats>(); }
        
        // Total casualties
        int friendlyCasualtiesInt = friendlyCasualties.Count;
        int enemyCasualtiesInt = enemyCasualties.Count;
        int totalCasualties = friendlyCasualtiesInt + enemyCasualtiesInt;
        
        casualtyReport.text = "Total Casualties: " + totalCasualties.ToString() + "\n" + 
        "Friendly Casualties: " + friendlyCasualtiesInt.ToString() + "\n" + 
        "Enemy Casualties: " + enemyCasualtiesInt.ToString();
        
        // Display all casualties
        // Use same CSC to report list of deceased friendlys.
        
        List<CharacterStats> allCasualties = friendlyCasualties.Concat(enemyCasualties).ToList();

        string namesText = "None";
        if (allCasualties.Count != 0)
        {
            List<string> casualtyNames = new List<string>();
            foreach (CharacterStats name in allCasualties)
            {
                casualtyNames.Add(name.GetPilotFaction() + ": " + NameDisplay(name.GetPilotName()));
            }
            namesText = string.Join(", \n", casualtyNames); 
        }
        // Test: namesText = NameDisplay("Madeline Engle");
        
        indivCasualtyList.text = "The deceased: \n" + namesText;
    }

    // Official Formatting of Character Names (Last, First)
    private string NameDisplay(string pilotName)
    {
        string[] sections = pilotName.Split(" ");
        // Are there any names with more than 2 names (first + last)? ex: (first + last last)
        string formatted = sections[1] + ", " + sections[0];
        return formatted;
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