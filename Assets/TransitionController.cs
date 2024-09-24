using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TransitionController : MonoBehaviour
{
    [SerializeField] private Image blackScreenFader;
    [SerializeField] private float fadeRate = 0.05f;
    private bool isBlackScreenTarget = false;
    [SerializeField] private string sceneTransitionTarget;
    

    private void Start() {
        float currentColorAlpha = blackScreenFader.color.a;
        blackScreenFader.color = new Color(blackScreenFader.color.r, blackScreenFader.color.g, blackScreenFader.color.b, 1f);
        isBlackScreenTarget = false;
    }

    private void Update() {
        // Debug.Log("Current Target: " + isBlackScreenTarget);
        float currentColorAlpha = blackScreenFader.color.a;
        if (isBlackScreenTarget) {
            float newColorAlpha = currentColorAlpha;
            newColorAlpha += fadeRate;
            newColorAlpha = Mathf.Min(1f, newColorAlpha);
            newColorAlpha = Mathf.Max(0f, newColorAlpha);
            blackScreenFader.color = new Color(blackScreenFader.color.r, blackScreenFader.color.g, blackScreenFader.color.b, newColorAlpha);

            // Debug.Log("newColorAlpha: " + newColorAlpha);
            if ((sceneTransitionTarget != "" || sceneTransitionTarget != null) && newColorAlpha == 1f) {
                SceneManager.LoadScene(sceneTransitionTarget);
            }
        } else {
            float newColorAlpha = currentColorAlpha;
            newColorAlpha -= fadeRate;
            newColorAlpha = Mathf.Min(1f, newColorAlpha);
            newColorAlpha = Mathf.Max(0f, newColorAlpha);
            blackScreenFader.color = new Color(blackScreenFader.color.r, blackScreenFader.color.g, blackScreenFader.color.b, newColorAlpha);
        }
    }

    public void TransitionToNewScene(string newSceneName) {
        isBlackScreenTarget = true;
        blackScreenFader.color = new Color(blackScreenFader.color.r, blackScreenFader.color.g, blackScreenFader.color.b, 0.001f);
        sceneTransitionTarget = newSceneName;
    }
}
