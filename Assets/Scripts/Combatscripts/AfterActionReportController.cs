using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AfterActionReportController : MonoBehaviour
{
    [SerializeField] private GameObject backgroundObject; // Used for actual background
    [SerializeField] private Text casualtyReport; 
    [SerializeField] private Text indivCasualtyList;
    private CombatStateController combatStateController; 
    
    // Contain a CharacterDisplay
    [SerializeField] private GameObject characterDisplay;
    private CharacterDisplayController characterDisplayController;
    
	private void Start() {
		backgroundObject.SetActive(false);
		characterDisplay.SetActive(false);
	}

    public void AARStart()
    {
        combatStateController = FindObjectOfType<CombatStateController>();
        if (combatStateController == null) {
            Debug.Log("Start method called from unit After Action Report controller");
            combatStateController = new GameObject("CombatStateController").AddComponent<CombatStateController>();
            // combatStateController.PauseOrResumeCombat(shouldPause, gameObject);
        }
		
		backgroundObject.SetActive(true);
        
        PauseThroughCombatManager();
		FindObjectOfType<TurnManager>().gameObject.SetActive(false);
        Debug.LogWarning("AfterActionReportConotroller has set the end turn manager to inactive");
        List<CharacterStats> deceased = combatStateController.GetDeceased();
        
        
        
        // Total casualties
        int totalCasualties = deceased.Count;
        
        casualtyReport.text = "Total Casualties: " + totalCasualties.ToString();
        
        // Display all individual casualties
        // Use same CSC to report list of deceased friendlys.
        string namesText = "None";
        if (totalCasualties != 0)
        {
            List<string> casualtyNames = new List<string>();
            foreach (CharacterStats name in deceased)
            {
                casualtyNames.Add(name.GetPilotFaction() + ": " + NameDisplay(name.GetPilotName()));
            }
            namesText = string.Join(", \n", casualtyNames); 
        }
        // Test: namesText = NameDisplay("Madeline Engle");
        indivCasualtyList.text = "The deceased: \n" + namesText;
        
        
        // Character Display
        characterDisplayController = characterDisplay.GetComponent<CharacterDisplayController>();
	    characterDisplay.SetActive(true);
	    if (deceased.Count > 0)
	    {
		    characterDisplayController.DisplayDeceased(deceased.FirstOrDefault()); // deceased.FirstOrDefault()
	    }
                
	    
        
    }

    // Formatting of Character Names (Last, First)
    private string NameDisplay(string pilotName)
    {
        string[] sections = pilotName.Split(" ");

		// Use this line if all names are inputted as First Last
        // string formatted = sections[1] + ", " + sections[0];
		if (sections.Count() > 2) { Debug.Log("More than 2 names provided instead of First Last"); }

		// If names are first last
		string testone = sections[1].ToCharArray()[0].ToString();
		string formatted = testone.ToUpper() + sections[1].Substring(1); 
		string test = sections[0].ToCharArray()[0].ToString();
		formatted += ", " + test.ToUpper() + sections[0].Substring(1);
        return formatted;
    }
    
    // TellGameManagerPause: Tell game manager to pause in case of (starting) AAR (or something else?)
    private void PauseThroughCombatManager() {
        combatStateController.PauseOrResumeCombat(true, gameObject);
    }

	public void ContinueToLevel() {
		Debug.Log("Clicking Level Menu");
		FindObjectOfType<PauseMenuController>().GoToLevelChooser();
	}

    
}