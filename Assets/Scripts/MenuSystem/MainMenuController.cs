using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPrefab;
    private PauseMenuController pauseMenuController;

    public void PullUpPauseMenuFromButton() {
        if (pauseMenuController == null) {
            pauseMenuController = FindObjectOfType<PauseMenuController>();
        }
        pauseMenuController.Pause();
    }

    public void StartLevelChooser() {
        SceneManager.LoadScene("Map");
    }
}
