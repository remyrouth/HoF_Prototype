using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechDisplayManager : MonoBehaviour
{
    [SerializeField] private Transform mechStartPosition;
    private GameObject currentMechObject;


    // sound variables
    private SoundManager soundManager;
    private SingleSoundPlayer currentSsingleSoundPlayer;

    public void DisplayMech(MechStats mechStats)
    {
        CleanUpCurrentMech();
        if (mechStats != null)
        {
            GameObject mechPrefab = mechStats.GetMechGFXPrefab();
            currentMechObject = Instantiate(mechPrefab, mechStartPosition.position, mechStartPosition.rotation);
        }

        PrepSoundManager();
        if (mechStats != null) {
            PlayMechIdleSound(mechStats);
        }
    }

    private void PlayMechIdleSound(MechStats mechStats) {
        Sound mechIdleSFX = mechStats.GetMechIdleSFX();
        if (mechIdleSFX != null) {
            currentSsingleSoundPlayer = soundManager.GetOrCreateSoundPlayer(mechIdleSFX);
            currentSsingleSoundPlayer.PlayFromForeignTrigger();
        } else {
            Debug.LogWarning("Current Mech does not have an idle sound in the scriptable object");
        }
    }

    private void PrepSoundManager() {
        if (soundManager == null) {
            soundManager = FindObjectOfType<SoundManager>();
        }
        if (soundManager == null) {
            soundManager = gameObject.AddComponent<SoundManager>();
        }
    }

    private void CleanUpCurrentMech()
    {
        if (currentMechObject != null)
        {
            Destroy(currentMechObject);
            currentMechObject = null;
        }
    }
}