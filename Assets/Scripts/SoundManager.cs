using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private Dictionary<string, SingleSoundPlayer> soundPlayerDictionary = new Dictionary<string, SingleSoundPlayer>();

    [Header("Music Track Variables")]
    [SerializeField] private Sound musicTrack;
    private SingleSoundPlayer musicTrackPlayer;


    private GameObject cameraAudioChildObject; // will hold sound players

    private void Awake() {
        musicTrackPlayer = GetOrCreateSoundPlayer(musicTrack);
        musicTrackPlayer.PlayFromForeignTrigger();
    }

    public SingleSoundPlayer GetOrCreateSoundPlayer(Sound soundScriptableObject)
    {
        string soundName = soundScriptableObject.name;

        if (soundPlayerDictionary.TryGetValue(soundName, out SingleSoundPlayer existingPlayer))
        {
            // If the SingleSoundPlayer already exists, return it
            return existingPlayer;
        }
        else
        {
            if (cameraAudioChildObject == null) {
                CreateSoundPlayerHolder();
            }
            SingleSoundPlayer newPlayer = cameraAudioChildObject.AddComponent<SingleSoundPlayer>();
            newPlayer.Initialize(soundScriptableObject);

            // Add the new SingleSoundPlayer to the dictionary
            soundPlayerDictionary[soundName] = newPlayer;

            return newPlayer;
        }
    }

    private void CreateSoundPlayerHolder() {
        cameraAudioChildObject = new GameObject("SoundPlayerHolder");
        cameraAudioChildObject.transform.parent = GameObject.FindObjectOfType<AudioListener>().gameObject.transform;
    }

    public void PauseMusic() {
        if (musicTrackPlayer) {
            musicTrackPlayer.PauseFromForeignTrigger();
        }
    }

    public void PlayMusic() {
        if (musicTrackPlayer) {
            musicTrackPlayer.PlayFromForeignTrigger();
        }
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }

}
