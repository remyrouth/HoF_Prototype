using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // this is used to reference the specific single sound player matched to a scriptable object
    private Dictionary<string, SingleSoundPlayer> soundPlayerDictionary = new Dictionary<string, SingleSoundPlayer>();

    // This is used to reference every single sound player of a specific sound type which will be used for game
    private Dictionary<Sound.SoundType, SoundListClass> soundTypeList = new Dictionary<Sound.SoundType, SoundListClass>();

    [Header("Music Track Variables")]
    [SerializeField] private Sound musicTrack;
    private SingleSoundPlayer musicTrackPlayer;


    private GameObject cameraAudioChildObject; // will hold sound players

    private void Awake() {
        soundTypeList[Sound.SoundType.SoundEffect] = new SoundListClass();
        soundTypeList[Sound.SoundType.AmbientSoundEffect] = new SoundListClass();
        soundTypeList[Sound.SoundType.MusicTrack] = new SoundListClass();
    }
    private void Start() {
        if (musicTrack != null) {
            musicTrackPlayer = GetOrCreateSoundPlayer(musicTrack);
            musicTrackPlayer.PlayFromForeignTrigger();
        } else {
            Debug.LogWarning("SoundManager was not given a music track to play");
        }
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
            soundTypeList[soundScriptableObject.GetSoundType()].AddSingleSoundPlayerToList(newPlayer);
            return newPlayer;
        }
    }

    private void CreateSoundPlayerHolder() {
        cameraAudioChildObject = new GameObject("SoundPlayerHolder");
        cameraAudioChildObject.transform.parent = GameObject.FindObjectOfType<AudioListener>().gameObject.transform;
    }

    public void UpdateSoundList(Sound.SoundType soundTypeToUpdate, float newPercent) {
        soundTypeList[soundTypeToUpdate].UpdateVolumePercent(newPercent);
    }

    public class SoundListClass {
        private List<SingleSoundPlayer> soundPlayerList = new List<SingleSoundPlayer>();
        public float currentSoundListVolumePercent = 1f;
        public void AddSingleSoundPlayerToList(SingleSoundPlayer newPlayer) {
            soundPlayerList.Add(newPlayer);
        }

        // gives the new volume to all single sound players in this list, they individually
        // handle that percentage to their personal max volume
        public void UpdateVolumePercent(float newPercent) {
            if (newPercent == currentSoundListVolumePercent) {
                return;
            }

            foreach(SingleSoundPlayer soundPlayer in soundPlayerList) {
                soundPlayer.NewVolumePercentageOutput(newPercent);
            }
        }
    }

}
