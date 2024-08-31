using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public enum LevelState {
        ChoosingTeamAndLevel,
        Combat
    }
    
    private LevelState currentSceneLevelState = LevelState.Combat;
    private CombatStateController combatStateController;

    // called by PlayerController to tell CombatStateController to keep track of entity count
    // so that it can end the game if all entities are dead
    public CombatStateController GetCombatStateController() {
        currentSceneLevelState = LevelState.Combat;
        if (combatStateController == null) {
            combatStateController = gameObject.AddComponent<CombatStateController>();
        }
        return combatStateController;
    }

    public void GetLevelTeamStateController() {
        currentSceneLevelState = LevelState.ChoosingTeamAndLevel;
    }

    // called by pause menu controller
    public void PauseResumeControllers(bool shouldPause) {
        switch (currentSceneLevelState) {
            case LevelState.Combat:
                combatStateController.PauseOrResumeCombat(shouldPause);
                break;
            case LevelState.ChoosingTeamAndLevel:
                break;
            default:
                Debug.LogWarning("Unknown LevelState type.");
                break;
        }
    }


}
