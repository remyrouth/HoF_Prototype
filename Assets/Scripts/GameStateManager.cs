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
    
    private CombatStateController combatStateController;

    public CombatStateController GetCombatStateController() {
        if (combatStateController == null) {
            combatStateController = gameObject.AddComponent<CombatStateController>();
        }
        return combatStateController;
    }

    public void GetLevelTeamStateController() {

    }


}
