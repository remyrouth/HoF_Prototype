using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNTrigger : MonoBehaviour
{
    [SerializeField] private GameObject VisualNovelToTrigger;
    [SerializeField] private bool startsOnLoad = false;

    private bool isVNActive = false;

    private void Start() {
        VisualNovelToTrigger.SetActive(false);
        if (startsOnLoad) {
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

            Debug.Log("Visual Novel object has been destroyed.");
            isVNActive = false; // Disable further checks
            
        }
    }
}
