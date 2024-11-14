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
    private CombatStateController combatStateController;

    private List<CharacterStats> deceased;
    
    // Contain a CharacterDisplay
    [SerializeField] private Button refreshCharacterDisplays;
    
    [SerializeField] private GameObject characterDisplayOne;
    private CharacterDisplayController characterDisplayControllerOne;
    [SerializeField] private GameObject characterDisplayTwo;
    private CharacterDisplayController characterDisplayControllerTwo;
    [SerializeField] private GameObject characterDisplayThree;
    private CharacterDisplayController characterDisplayControllerThree;
    [SerializeField] private GameObject characterDisplayFour;
    private CharacterDisplayController characterDisplayControllerFour;
    
    private int start;
    
	private void Start() {
		backgroundObject.SetActive(false);
		characterDisplayOne.SetActive(false);
		characterDisplayTwo.SetActive(false);
		characterDisplayThree.SetActive(false);
		characterDisplayFour.SetActive(false);
	}

    public void AARStart()
    {
        combatStateController = FindObjectOfType<CombatStateController>();
        if (combatStateController == null) {
            Debug.Log("Start method called from unit After Action Report controller");
            combatStateController = new GameObject("CombatStateController").AddComponent<CombatStateController>();
        }
		
		backgroundObject.SetActive(true);
        
		// Pause
        PauseThroughCombatManager();
		FindObjectOfType<TurnManager>().gameObject.SetActive(false);
        Debug.LogWarning("AfterActionReportConotroller has set the end turn manager to inactive");
        
        deceased = combatStateController.GetDeceased();
        
        // Total casualties
        int totalCasualties = deceased.Count;
        casualtyReport.text = "Total Casualties: " + totalCasualties.ToString();
        
        // Character Display
        
        characterDisplayControllerOne = characterDisplayOne.GetComponent<CharacterDisplayController>();
	    characterDisplayOne.SetActive(true);
	    characterDisplayControllerTwo = characterDisplayTwo.GetComponent<CharacterDisplayController>();
	    characterDisplayTwo.SetActive(true);
	    characterDisplayControllerThree = characterDisplayThree.GetComponent<CharacterDisplayController>();
	    characterDisplayThree.SetActive(true);
	    characterDisplayControllerFour = characterDisplayFour.GetComponent<CharacterDisplayController>();
	    characterDisplayFour.SetActive(true);
	    start = 0;
	    
	    DisplayUpdate();
    }
    
    // TellGameManagerPause: Tell game manager to pause in case of (starting) AAR (or something else?)
    private void PauseThroughCombatManager() {
        combatStateController.PauseOrResumeCombat(true, gameObject);
    }

	public void ContinueToLevel() {
		Debug.Log("Clicking Level Menu");
		FindObjectOfType<PauseMenuController>().GoToLevelChooser();
	}

	// Display format helpers.
	private CharacterStats DisplayEnsure(int ind)
	{
		if (ind >= deceased.Count)
		{
			return null;
		}
		return deceased[ind];
	}

	public void DisplayUpdate()
	{
		// update displays
		characterDisplayControllerOne.DisplayDeceased(DisplayEnsure(start));
		characterDisplayControllerTwo.DisplayDeceased(DisplayEnsure(start + 1));
		characterDisplayControllerThree.DisplayDeceased(DisplayEnsure(start + 2));
		characterDisplayControllerFour.DisplayDeceased(DisplayEnsure(start + 3));
		
		// if start still valid, add.
		if (deceased.Count > start + 4)
		{
			start = start + 4;
		}
		// if start no longer valid, disable button. 
		else
		{
			refreshCharacterDisplays.gameObject.SetActive(false);
		}
	}
    
}