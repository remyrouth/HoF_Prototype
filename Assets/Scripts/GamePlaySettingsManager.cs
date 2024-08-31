using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlaySettingsManager : MonoBehaviour
{
    public delegate void VolumeChanged(float newVolume);
    public event VolumeChanged OnSoundEffectVolumeChanged;
    public event VolumeChanged OnAmbientSoundVolumeChanged;
    public event VolumeChanged OnMusicVolumeChanged;

    private float _masterVolume = 1f;
    private float _soundEffectVolume = 1f;
    private float _ambientSoundVolume = 1f;
    private float _musicVolume = 1f;

    public float MasterVolume
    {
        get => _masterVolume;
        set
        {
            _masterVolume = value;
            UpdateVolume(OnSoundEffectVolumeChanged, _soundEffectVolume);
            UpdateVolume(OnAmbientSoundVolumeChanged, _ambientSoundVolume);
            UpdateVolume(OnMusicVolumeChanged, _musicVolume);
        } 
    }

    public float SoundEffectVolume
    {
        get => _soundEffectVolume;
        set => UpdateVolume(ref _soundEffectVolume, value, OnSoundEffectVolumeChanged);
    }

    public float AmbientSoundVolume
    {
        get => _ambientSoundVolume;
        set => UpdateVolume(ref _ambientSoundVolume, value, OnAmbientSoundVolumeChanged);
    }

    public float MusicVolume
    {
        get => _musicVolume;
        set => UpdateVolume(ref _musicVolume, value, OnMusicVolumeChanged);
    }

    private void UpdateVolume(VolumeChanged volumeChanged, float volume)
    {
        volumeChanged?.Invoke(_masterVolume * volume);
    }

    private void UpdateVolume(ref float currentVolume, float newVolume, VolumeChanged volumeChanged)
    {
        if (currentVolume == newVolume) return;
        currentVolume = newVolume;
        UpdateVolume(volumeChanged, currentVolume);
    }
}
