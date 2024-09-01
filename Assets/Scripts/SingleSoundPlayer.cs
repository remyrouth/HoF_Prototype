using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SingleSoundPlayer : MonoBehaviour
{
    private float MaxVolume;
    private Sound.SoundType enumSoundType;
    private AudioSource audioSource;
    
    // Initialization method, self-explanatory
    public void Initialize(Sound soundScriptableObject)
    {
        this.MaxVolume = soundScriptableObject.GetMaxVolume();
        this.enumSoundType = soundScriptableObject.GetSoundType();
        InitializeAudioSource();
        audioSource.clip = soundScriptableObject.GetAudioClip();
        audioSource.loop = soundScriptableObject.GetLoopStatus();
        audioSource.volume = soundScriptableObject.GetMaxVolume();
    }

    // an outside / foreign script will trigger the 
    // play audio source method
    public void PlayFromForeignTrigger() {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    // an outside / foreign script will trigger the 
    // pause audio source method
    public void PauseFromForeignTrigger() {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    private void InitializeAudioSource() {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    // the settings has sliders like master volume and sound effect volume changers
    // while a single sound player has a max volume float variable
    // of that volume maximum, we can only output a percentage of it 
    // according to the gameplaysettingmanger.cs script settings
    public void NewVolumePercentageOutput(float newPercentOutput) {
        audioSource.volume = MaxVolume * newPercentOutput;
        Debug.Log("current percent: " + newPercentOutput);
    }

}
