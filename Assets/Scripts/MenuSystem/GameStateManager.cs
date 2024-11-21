using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


// this is a game state manager. More accurately, it will give the appropriate manager to 
// relavent scripts depending on the scene. So if this is a combat scene, this will give 
// it a combat state controller. It centralizes which scripts to pause as well.
public class GameStateManager : MonoBehaviour
{
    public enum GameSceneType {
        UpgradeScene,
        CombatScene,
        LevelChoosingScene,
        MainMenuScene
    }
    private CombatStateController combatStateController;
    private CameraController cameraController;

    private void Awake() {
        combatStateController = FindObjectOfType<CombatStateController>();
        if (combatStateController)
        {
            Debug.LogWarning(
                "combatStateController does not exist, and was created by a game state manager object object");
            GameObject combatObject = new GameObject("CombatStateController");
            combatStateController = combatObject.AddComponent<CombatStateController>();
        }
    }

    public GameSceneType GetSceneType() {
        MapSelectorController mapSelectorControllerSingleton = FindObjectOfType<MapSelectorController>();
        PlayerController playerController = FindObjectOfType<PlayerController>();
        MainMenuController mainMenuController = FindObjectOfType<MainMenuController>();
        // currently we just have two types 
        // mapSelectorControllerSingleton 
        // will always be present in the map choosing scenes
        // so we'll just check that for now

        if (mapSelectorControllerSingleton != null) { // we are in map level chooser 
            return GameSceneType.LevelChoosingScene;
        } else if (playerController != null) { // only exist in combat scenes
            return GameSceneType.CombatScene;
        } else { // we only have a main menu type scene left
            return GameSceneType.MainMenuScene;
        }
    }

    // called by pause menu controller
    public void PauseResumeControllers(bool shouldPause) {
        if (combatStateController != null) {
            // there may not always be a combat controller in scene
            combatStateController.PauseOrResumeCombat(shouldPause, gameObject);
        }

        if (cameraController == null) {
            // there should always be a camera controller in scene
            cameraController = FindObjectOfType<CameraController>();
        }

        if (cameraController != null) {
            cameraController.SetPauseState(shouldPause);
        }

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
