using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSound", menuName = "Sound")]
public class Sound : ScriptableObject
{
    [SerializeField] private bool shouldLoop;
    [SerializeField][Range(0, 1)] private float maxVolume = 1f;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private SoundType soundType = SoundType.SoundEffect;
    [SerializeField] private bool usesForeignTrigger;
    // if we use a foreign trigger it means a foreign trigger is responsible for triggering this
    // and it should therefore not play immediatly once it is created

    public bool GetLoopStatus() {
        return shouldLoop;
    }

    public float GetMaxVolume() {
        return maxVolume;
    }

    public AudioClip GetAudioClip() {
        return audioClip;
    }

    public SoundType GetSoundType() {
        return soundType;
    }

    public bool GetTriggerBoolStatus() {
        return usesForeignTrigger;
    }
    public enum SoundType
    {
        SoundEffect,
        AmbientSoundEffect,
        MusicTrack
    }
}
