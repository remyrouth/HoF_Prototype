using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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

    private int currentEnemyCount = 0;
    private int currentFriendlyCount = 0;


    // Controllers
    [SerializeField] public ScrapController scrapController;
    private TurnManager turnManager;
    private CharacterCanvasController characterCanvasController;
    private SelectionManager selectionManager;
    

    // [SerializeField] private List<string> pauseCallerNames = new List<string>();
    [SerializeField] private List<GameObject> pauseCallerNames = new List<GameObject>();
    // called by GameStateManager

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

    public void IncreaseFriendlyCount(bool increase) {
        if (increase) {
            currentFriendlyCount++;
        } else {
            currentFriendlyCount--;
            EndLevel();
        }
        // Debug.Log("currentFriendlyCount: " + currentFriendlyCount);
    }

    public void IncreaseEnemyCount(bool increase) {
        if (increase) {
            currentEnemyCount++;
        } else {
            currentEnemyCount--;
            EndLevel();
        }

        // Debug.Log("currentEnemyCount: " + currentEnemyCount);
    }

    private void EndLevel() {
        if (currentEnemyCount <= 0 || currentFriendlyCount <= 0) {
            // Debug.Log("currentEnemyCount: " + currentEnemyCount + "  currentFriendlyCount: " + currentFriendlyCount);
            // Debug.Log("GAME HAS ENEDED NOW");
            // Map

            // handles saving granted mechs to the team text file
            MechSaveFileInteractor saveFileScript = gameObject.AddComponent<MechSaveFileInteractor>();
            // saveFileScript.LogMechStatsToFile();
            List<GameObject> players = GameObject.FindGameObjectsWithTag("Player").ToList();
            foreach(GameObject player in players) {
                PlayerController playerScript = player.GetComponent<PlayerController>();
                if (playerScript.grantedOnBoard) {
                    saveFileScript.LogMechStatsToFile(playerScript);
                }
            }
            

            // handles adding scrap
            if (scrapController != null)
            {
                scrapController.SetScrapPostCombat();
            }
            else
            {
                Debug.LogWarning("You forgot to attach the ScrapCollector.cs component, the game will not reward players scrap without this script being on this same object. Please fix this.");
            }
            
            SceneManager.LoadScene("Map");
        }
    }
}
