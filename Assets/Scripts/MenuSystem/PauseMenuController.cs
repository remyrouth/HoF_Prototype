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
    [SerializeField] private GameObject goToMainMenuSceneButton;
    [SerializeField] private string MainMenuSceneName = "MainMenu";

    private GamePlaySettingsManager settingsManager;

    private void Awake() {
        settingsManager = FindObjectOfType<GamePlaySettingsManager>();
        if (settingsManager == null) {
            settingsManager = gameObject.AddComponent<GamePlaySettingsManager>();
        }

    }



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
        MainMenuButtonCheck();
    }

    // called by button
    public void GoToLevelChooser() {
        SceneManager.LoadScene(LevelChoosingSceneName);
    }

    // called by button
    public void GoToMainMenu() {
        SceneManager.LoadScene(MainMenuSceneName);
    }

    public void OpenSettingsMenu() {
        MainPauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    // checks if we can use the main menu scene button, if so
    // then allow said button to appear on the pause menu
    private void MainMenuButtonCheck() {
        GameStateManager.GameSceneType sceneType = gameStateManager.GetSceneType();
        goToMainMenuSceneButton.SetActive(sceneType != GameStateManager.GameSceneType.MainMenuScene);
    }

    // checks if we can use the level chooser scene button, if so
    // then allow said button to appear on the pause menu
    private void LevelChoosingButtonCheck() {
        GameStateManager.GameSceneType sceneType = gameStateManager.GetSceneType();
        goToLevelChooserSceneButton.SetActive(sceneType == GameStateManager.GameSceneType.CombatScene);
    }
}
