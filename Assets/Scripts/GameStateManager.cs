using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;


// this is a game state manager. More accurately, it will give the appropriate manager to 
// relavent scripts depending on the scene. So if this is a combat scene, this will give 
// it a combat state controller. It centralizes which scripts to pause as well.
public class GameStateManager : MonoBehaviour
{
    private CombatStateController combatStateController;
    private CameraController cameraController;

    // called by PlayerController to tell CombatStateController to keep track of entity count
    // so that it can end the game if all entities are dead
    public CombatStateController GetCombatStateController() {
        combatStateController = FindObjectOfType<CombatStateController>();
        if (combatStateController == null) {
            combatStateController = gameObject.AddComponent<CombatStateController>();
        }
        return combatStateController;
    }

    public void GetLevelTeamStateController() {

    }

    // called by pause menu controller
    public void PauseResumeControllers(bool shouldPause) {
        if (combatStateController != null) {
            // there may not always be a combat controller in scene
            combatStateController.PauseOrResumeCombat(shouldPause);
        }

        if (cameraController == null) {
            // there should always be a camera controller in scene
            cameraController = FindObjectOfType<CameraController>();
        }
        cameraController.SetPauseState(shouldPause);

        PauseResumeLevelTeamControllers(shouldPause);

    }

    // this is where we handle pausing team/level choosing
    private void PauseResumeLevelTeamControllers(bool shouldPause) {
        MapSelectorController mapSelectorControllerSingleton = FindObjectOfType<MapSelectorController>();
        TeamChooserUI teamChooserUISingleton = FindObjectOfType<TeamChooserUI>();
        List<TeamSpotOptionController> optionControllerList = FindObjectsOfType<TeamSpotOptionController>().ToList();

        if (mapSelectorControllerSingleton != null) {

        }

        if (teamChooserUISingleton != null) {
            teamChooserUISingleton.SetPauseState(shouldPause);
        }

        if (mapSelectorControllerSingleton != null) {
            mapSelectorControllerSingleton.SetPauseState(shouldPause);
        }

        foreach (TeamSpotOptionController optionController in optionControllerList) {
            if (optionController != null) {
                optionController.SetPauseState(shouldPause);
            }
        }
    }


}
