using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public enum GameState {
        StillPlaying,
        LoseState,
        WinState
    }

    public enum WinCondition {
        EnemyAnnihilation
    }

    [SerializeField]
    private int currentEnemyCount = 0;
    private int currentFriendlyCount = 0;

    public void IncreaseFriendlyCount(bool increase) {
        if (increase) {
            currentFriendlyCount++;
        } else {
            currentFriendlyCount--;
            EndLevel();
        }
        Debug.Log("currentFriendlyCount: " + currentFriendlyCount);
    }

    public void IncreaseEnemyCount(bool increase) {
        if (increase) {
            currentEnemyCount++;
        } else {
            currentEnemyCount--;
            EndLevel();
        }

        Debug.Log("currentEnemyCount: " + currentEnemyCount);
    }

    private void EndLevel() {
        if (currentEnemyCount <= 0 || currentFriendlyCount <= 0) {
            Debug.Log("currentEnemyCount: " + currentEnemyCount + "  currentFriendlyCount: " + currentFriendlyCount);
            Debug.Log("GAME HAS ENEDED NOW");
            // Map
            SceneManager.LoadScene("Map");
        }
    }
}
