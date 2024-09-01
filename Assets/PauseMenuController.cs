using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject MainPauseMenu;



    // Scripts to control pause with
    private GameStateManager gameStateManager;

    private bool isPaused = false;

    private void Start() {
        MainPauseMenu.SetActive(isPaused);
        gameStateManager = FindObjectOfType<GameStateManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            if (gameStateManager != null) {
                gameStateManager.PauseResumeControllers(isPaused);
            }
            MainPauseMenu.SetActive(isPaused);
        }
    }
}
