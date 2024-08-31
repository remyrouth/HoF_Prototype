using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatStateController : MonoBehaviour
{
    public enum CombatState {
        Paused,
        PlayerTurn,
        AITurn
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
    private TurnManager turnManager;
    private CharacterCanvasController characterCanvasController;
    private SelectionManager selectionManager;


    // called by GameStateManager
    public void PauseOrResumeCombat(bool shouldPause) {
        FindManagers();
        if (shouldPause) { // we pause
            Pause();
        } else { // we resume the AI or the player turn
           Resume();
        }
    }

    private void Pause() {
        selectionManager.enabled = false;
        characterCanvasController.MenuCleanup();
        turnManager.PauseTurnSystem();
    }

    private void Resume() {
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
            SceneManager.LoadScene("Map");
        }
    }
}
