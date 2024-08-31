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
    private bool playerTurnCheck = true; // this will tell us when we're resuming,
    // if we go back to AI turn, or back to player controls
    private TurnManager turnManager;
    private CharacterCanvasController characterCanvasController;
    private SelectionManager selectionManager;


    public void PauseCombat() {
        FindManagers();
        if (selectionManager.enabled == true) { // this means its currently the player turn
            playerTurnCheck = true;
            selectionManager.enabled = false;
            characterCanvasController.MenuCleanup();
        } else { // this means its currently the AI turn

        }
    }

    public void ResumeCombat() {
        FindManagers();

    }

    private void FindManagers() {
        if (turnManager == null) {
            turnManager = FindObjectOfType<TurnManager>();
        }
        if (turnManager == null) {
            turnManager = FindObjectOfType<TurnManager>();
        }
        if (turnManager == null) {
            turnManager = FindObjectOfType<TurnManager>();
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
