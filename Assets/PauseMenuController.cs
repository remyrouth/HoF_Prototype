using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject MainPauseMenu;
    private GameStateManager gameStateManager;
    private CameraController cameraController;

    private bool isPaused = false;

    private void Start() {
        MainPauseMenu.SetActive(isPaused);
        gameStateManager = FindObjectOfType<GameStateManager>();
        cameraController = FindObjectOfType<CameraController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            gameStateManager.PauseResumeControllers(isPaused);
            MainPauseMenu.SetActive(isPaused);

            if (cameraController != null) {
                cameraController.SetPauseState(isPaused);
            }
        }
    }
}
