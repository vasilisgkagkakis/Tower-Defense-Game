using UnityEngine;
using System.Collections.Generic;

public class GlobalAudioController : MonoBehaviour
{
    public static GlobalAudioController Instance;

    // Lists to track all audio sources by category
    private List<AudioSource> backgroundMusicSources = new();
    private List<AudioSource> soundEffectSources = new();

    // Current volume levels
    private float backgroundMusicVolume = 0.7f;
    private float soundEffectsVolume = 0.8f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadVolumeSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Register an audio source as background music
    public void RegisterBackgroundMusic(AudioSource audioSource)
    {
        if (audioSource != null && !backgroundMusicSources.Contains(audioSource))
        {
            // Remove from sound effects list if it was there (prevent double registration)
            soundEffectSources.Remove(audioSource);

            backgroundMusicSources.Add(audioSource);
            audioSource.volume = backgroundMusicVolume;
        }
    }

    // Register an audio source as sound effect
    public void RegisterSoundEffect(AudioSource audioSource)
    {
        if (audioSource != null && !soundEffectSources.Contains(audioSource))
        {
            // Remove from background music list if it was there (prevent double registration)
            backgroundMusicSources.Remove(audioSource);

            soundEffectSources.Add(audioSource);
            audioSource.volume = soundEffectsVolume;
        }
    }

    private bool IsBackgroundMusicSource(string name, string tag)
    {
        return name.Contains("music") || name.Contains("background") ||
               tag.Contains("music") || tag.Contains("background");
    }

    // Set background music volume (0.0 to 1.0)
    public void SetBackgroundMusicVolume(float volume)
    {
        backgroundMusicVolume = Mathf.Clamp01(volume);

        // Update all registered background music sources
        foreach (AudioSource source in backgroundMusicSources)
        {
            if (source != null)
            {
                source.volume = backgroundMusicVolume;
            }
        }

        // Save setting
        PlayerPrefs.SetFloat("BackgroundMusicVolume", backgroundMusicVolume);
    }

    // Set sound effects volume (0.0 to 1.0)
    public void SetSoundEffectsVolume(float volume)
    {
        soundEffectsVolume = Mathf.Clamp01(volume);

        // Update all registered sound effect sources
        foreach (AudioSource source in soundEffectSources)
        {
            if (source != null)
            {
                source.volume = soundEffectsVolume;
            }
        }

        // Save setting
        PlayerPrefs.SetFloat("SoundEffectsVolume", soundEffectsVolume);
    }

    // Clean up null references from the lists
    public void CleanupNullReferences()
    {
        backgroundMusicSources.RemoveAll(source => source == null);
        soundEffectSources.RemoveAll(source => source == null);
    }

    // Load volume settings from PlayerPrefs
    private void LoadVolumeSettings()
    {
        backgroundMusicVolume = PlayerPrefs.GetFloat("BackgroundMusicVolume", 0.7f);
        soundEffectsVolume = PlayerPrefs.GetFloat("SoundEffectsVolume", 0.8f);
    }
}