using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatStateController : MonoBehaviour
{
    public enum CombatState {
        PreCombat,
        RegularCombat,
        Paused
    }


    public enum WinCondition {
        EnemyAnnihilation,
        ZoneProtection,
        ObstacleProtection,
        FlagCapture
    }

    private int currentEnemyCount;
    private int currentFriendlyCount;


    // Controllers
    private TurnManager turnManager;
    private CharacterCanvasController characterCanvasController;
    private SelectionManager selectionManager;


    // [SerializeField] private List<string> pauseCallerNames = new List<string>();
    [SerializeField] private List<GameObject> pauseCallerNames = new List<GameObject>();
    // called by GameStateManager

    // list of deceased
    private List<CharacterStats> deceasedList;
    private AfterActionReportController afterActionReportController;
    
    // Start
    private void Start()
    {
        // Debug.Log("start aar");
        afterActionReportController = FindObjectOfType<AfterActionReportController>();
        if (afterActionReportController == null)
        {
            afterActionReportController =  new GameObject("AfterActionReportController").AddComponent<AfterActionReportController>();
        }
        // afterActionReportController.GameObject().SetActive(false);
        
        deceasedList = new List<CharacterStats>();
    }

    // we input an object to tell how many have called pause. we know we can
    // unpause from other scripts, so we have to know how many scripts
    // are currently requesting a pause. if there's no pause requests currently
    // in effect, we resume
    public void PauseOrResumeCombat(bool shouldPause, GameObject objectCaller) {
        // Debug.Log("PauseOrResumeCombat called in combat state controller");
        FindManagers();
        if (shouldPause) { // we pause
            Pause();
            // Debug.Log("pauseCallers.Count before addition: " + pauseCallerNames.Count);
            pauseCallerNames.Add(objectCaller);
            // Debug.Log("pauseCallers.Count after addition: " + pauseCallerNames.Count);
        } else { // we resume the AI or the player turn
            Debug.Log("Called Removal"); 
            // Debug.Log("pauseCallers.Count before removal: " + pauseCallerNames.Count);
            pauseCallerNames.Remove(objectCaller);
            // Debug.Log("pauseCallers.Count after removal: " + pauseCallerNames.Count);
            if (pauseCallerNames.Count == 0) {
                Resume();
            }
            Resume();
        }
    }

    private void Pause() {
        selectionManager.enabled = false;
        characterCanvasController.MenuCleanup();
        turnManager.PauseTurnSystem();
    }

    private void Resume() {
        Debug.Log("Resume called");
        
        bool playerTurnCheck = turnManager.IsPlayerTurnCheck(); // this will tell us when we're resuming,
        // if we go back to AI turn, or back to player controls

        if (playerTurnCheck) { // this means its currently the player turn
            selectionManager.enabled = true;
        } else { // this means its currently the AI turn
            selectionManager.enabled = false;
        }
        
        turnManager.ResumeTurnSystem();
        characterCanvasController.MenuCleanup();
    }

    private void FindManagers() {
        if (turnManager == null) {
            turnManager = FindObjectOfType<TurnManager>();
        }
        if (characterCanvasController == null) {
            characterCanvasController = FindObjectOfType<CharacterCanvasController>();
        }
        if (selectionManager == null) {
            selectionManager = FindObjectOfType<SelectionManager>();
        }
    }

    // Currently takes in character stats to get info to add to list of deceased names(CharacterStats). 
    public void IncreaseFriendlyCount(bool increase, CharacterStats name) {
        if (increase) {
            currentFriendlyCount++;
        } else {
            deceasedList.Add(name);
            currentFriendlyCount--;
            EndLevel();
        }
        // Debug.Log("currentFriendlyCount: " + currentFriendlyCount);
    }

    public void IncreaseEnemyCount(bool increase, CharacterStats name)
    {
        if (increase) {
            currentEnemyCount++;
        } else {
            deceasedList.Add(name);
            currentEnemyCount--;
            EndLevel();
        }
    }

    private void EndLevel() {
        Debug.Log("Checking end of level");
        if (currentEnemyCount <= 0 || currentFriendlyCount <= 0)
        {
            Debug.Log("Calling AARStart");
            afterActionReportController.AARStart();
            // afterActionReportController.GameObject().SetActive(true);

            // Map
            // SceneManager.LoadScene("Map");
        }
    }

    public List<CharacterStats> GetDeceased()
    {
        return deceasedList;
    }
}
