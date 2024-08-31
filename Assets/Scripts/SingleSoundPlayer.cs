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
    
    // Constructor
    public void Initialize(Sound soundScriptableObject)
    {
        this.MaxVolume = soundScriptableObject.GetMaxVolume();
        this.enumSoundType = soundScriptableObject.GetSoundType();
        InitializeAudioSource();
        audioSource.clip = soundScriptableObject.GetAudioClip();
        audioSource.loop = soundScriptableObject.GetLoopStatus();
        audioSource.volume = soundScriptableObject.GetMaxVolume();
    }

    public void PlayFromForeignTrigger() {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void PauseFromForeignTrigger() {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    private void InitializeAudioSource() {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

}
