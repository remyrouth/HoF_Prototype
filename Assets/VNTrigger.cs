using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNTrigger : MonoBehaviour
{
    [SerializeField] private GameObject VisualNovelToTrigger;
    [SerializeField] private bool startsOnLoad = false;
    [Header("If starts new scene after, put scene name here")]
    [SerializeField] private string sceneToTransitionTo = ""; // NOTE : This requires a TransitionController prefab to be in the scene

    private bool isVNActive = false;

    private void Start() {
        VisualNovelToTrigger.SetActive(false);
        if (startsOnLoad) {
            Destroy(GetComponent<Collider>());
            RunController runScript = FindObjectOfType<RunController>();
            if (runScript != null) {
                runScript.PauseControls(true);
            }
            VisualNovelToTrigger.SetActive(true);
            isVNActive = true;
        }
    }
    private void OnTriggerEnter(Collider other) {
        // if you dont want a trigger, then just remove the collider
        if (VisualNovelToTrigger != null) {
            RunController runScript = FindObjectOfType<RunController>();
            if (runScript != null) {
                runScript.PauseControls(true);
            }

            VisualNovelToTrigger.SetActive(true);
            isVNActive = true;
            Collider collider = GetComponent<Collider>();
            Destroy(collider);
        }
    }

    private void Update() {
        if (isVNActive && VisualNovelToTrigger == null) {

            RunController runScript = FindObjectOfType<RunController>();
            if (runScript != null) {
                runScript.PauseControls(false);
            }
            StartTransition();

            Debug.Log("Visual Novel object has been destroyed.");
            isVNActive = false; // Disable further checks
            
        }
    }

    private void StartTransition() {
        Debug.Log("StartTransition method called");
        TransitionController transitioner = FindObjectOfType<TransitionController>();
        if (transitioner != null) {
            if (sceneToTransitionTo != "" && sceneToTransitionTo != null) {
                transitioner.TransitionToNewScene(sceneToTransitionTo);
            }
        } else {
            Debug.LogWarning("There is no TransitionController prefab in scene, cannot transition");
        }
    }
}
