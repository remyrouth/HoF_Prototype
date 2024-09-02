using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlaySettingsManager : MonoBehaviour
{
    // master
    public Slider masterVSlider;

    // ambient
    public Slider soundEffectVSlider;

    // ambient
    public Slider ambientVSlider;

    // music
    public Slider musicVSlider;


    private SoundManager soundManager;

    private void Start() {
        soundManager = FindObjectOfType<SoundManager>();
        if (soundManager == null) {
            soundManager = gameObject.AddComponent<SoundManager>();
        }
    }

    // method called from button to update all sound floats
    // access/changed by sliders
    public void CallUpdateFromButton() {
        SendVolumeUpdateToSoundManager(Sound.SoundType.AmbientSoundEffect, ambientVSlider.value * masterVSlider.value);
        SendVolumeUpdateToSoundManager(Sound.SoundType.AmbientSoundEffect, soundEffectVSlider.value * masterVSlider.value);
        SendVolumeUpdateToSoundManager(Sound.SoundType.MusicTrack, musicVSlider.value * masterVSlider.value);
    }

    private void SendVolumeUpdateToSoundManager(Sound.SoundType soundType, float newVolumePercent)
    {
        

        // soundManager.
        // have sound manager when making a new sound be able to reference the current settings
        // have game play settings manager be able to tell sound manager to update all single sound players
        // of type X, with a specific float percentage input
        // and then the sound system is completely integrated
        // then have the UI pause menu update said values of game play settings manager
        soundManager.UpdateSoundList(soundType, newVolumePercent);
    }
}
