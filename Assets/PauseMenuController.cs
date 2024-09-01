using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject MainPauseMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject goToLevelChooserSceneButton;
    [SerializeField] private string LevelChoosingSceneName = "Map";



    // Scripts to control pause with
    private GameStateManager gameStateManager;

    private bool isPaused = false;

    private void Start() {
        MainPauseMenu.SetActive(isPaused);
        settingsMenu.SetActive(false);
        gameStateManager = FindObjectOfType<GameStateManager>();
        if (gameStateManager == null) {
            gameStateManager = gameObject.AddComponent<GameStateManager>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    // can be called by main menu script
    public void Pause() {
        isPaused = !isPaused;
        // if (gameStateManager == null) {
        //     gameObject.AddComponent<GameStateManager>();
        // }
        gameStateManager.PauseResumeControllers(isPaused);
        MainPauseMenu.SetActive(isPaused);
        LevelChoosingButtonCheck();
    }

    // called by button
    public void GoToLevelChooser() {
        SceneManager.LoadScene(LevelChoosingSceneName);
    }

    public void OpenSettingsMenu() {
        SceneManager.LoadScene(LevelChoosingSceneName);
    }
    

    // checks if we can use the level chooser scene button, if so
    // then allow said button to appear on the pause menu
    private void LevelChoosingButtonCheck() {
        GameStateManager.GameSceneType sceneType = gameStateManager.GetSceneType();
        if (sceneType == GameStateManager.GameSceneType.CombatScene) {
            goToLevelChooserSceneButton.SetActive(true);
        } else {
            goToLevelChooserSceneButton.SetActive(false);
        }
    }
}
