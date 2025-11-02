using UnityEngine;
using System.Collections.Generic;

public class GlobalAudioController : MonoBehaviour
{
    public static GlobalAudioController Instance;
    
    // Lists to track all audio sources by category
    private List<AudioSource> backgroundMusicSources = new List<AudioSource>();
    private List<AudioSource> soundEffectSources = new List<AudioSource>();
    
    // Current volume levels
    private float backgroundMusicVolume = 0.7f;
    private float soundEffectsVolume = 0.8f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolumeSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Don't auto-register - let scripts register their own audio sources manually
        // This prevents conflicts and double registration
        Debug.Log("🎧 GlobalAudioController ready - waiting for manual registrations");
    }
    
    #region Registration System
    
    /// <summary>
    /// Register an audio source as background music
    /// </summary>
    public void RegisterBackgroundMusic(AudioSource audioSource)
    {
        if (audioSource != null && !backgroundMusicSources.Contains(audioSource))
        {
            // Remove from sound effects list if it was there (prevent double registration)
            soundEffectSources.Remove(audioSource);
            
            backgroundMusicSources.Add(audioSource);
            audioSource.volume = backgroundMusicVolume;
            Debug.Log($"🎵 Registered background music: {audioSource.gameObject.name}");
        }
    }
    
    /// <summary>
    /// Register an audio source as sound effect
    /// </summary>
    public void RegisterSoundEffect(AudioSource audioSource)
    {
        if (audioSource != null && !soundEffectSources.Contains(audioSource))
        {
            // Remove from background music list if it was there (prevent double registration)
            backgroundMusicSources.Remove(audioSource);
            
            soundEffectSources.Add(audioSource);
            audioSource.volume = soundEffectsVolume;
            Debug.Log($"🔊 Registered sound effect: {audioSource.gameObject.name}");
        }
    }
    
    /// <summary>
    /// Unregister an audio source (when destroyed)
    /// </summary>
    public void UnregisterAudioSource(AudioSource audioSource)
    {
        backgroundMusicSources.Remove(audioSource);
        soundEffectSources.Remove(audioSource);
    }
    
    /// <summary>
    /// Automatically find and register all audio sources in the scene
    /// </summary>
    public void RefreshAllAudioSources()
    {
        AudioSource[] allSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        
        foreach (AudioSource source in allSources)
        {
            if (source == null) continue;
            
            // Auto-detect based on GameObject name or tag
            string objectName = source.gameObject.name.ToLower();
            string tag = source.gameObject.tag.ToLower();
            
            if (IsBackgroundMusicSource(objectName, tag))
            {
                RegisterBackgroundMusic(source);
            }
            else
            {
                // Default to sound effect if not explicitly background music
                RegisterSoundEffect(source);
            }
        }
        
        Debug.Log($"🔄 Refreshed audio sources: {backgroundMusicSources.Count} music, {soundEffectSources.Count} effects");
    }
    
    private bool IsBackgroundMusicSource(string name, string tag)
    {
        return name.Contains("music") || name.Contains("background") || 
               tag.Contains("music") || tag.Contains("background");
    }
    
    #endregion
    
    #region Volume Control
    
    /// <summary>
    /// Set background music volume (0.0 to 1.0)
    /// </summary>
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
        Debug.Log($"🎵 Background music volume set to: {(backgroundMusicVolume * 100):F0}%");
    }
    
    /// <summary>
    /// Set sound effects volume (0.0 to 1.0)
    /// </summary>
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
        Debug.Log($"🔊 Sound effects volume set to: {soundEffectsVolume * 100:F0}%");
    }
    
    /// <summary>
    /// Get current background music volume
    /// </summary>
    public float GetBackgroundMusicVolume()
    {
        return backgroundMusicVolume;
    }
    
    /// <summary>
    /// Get current sound effects volume
    /// </summary>
    public float GetSoundEffectsVolume()
    {
        return soundEffectsVolume;
    }
    
    #endregion
    
    #region Utility Methods
    
    /// <summary>
    /// Clean up null references from the lists
    /// </summary>
    public void CleanupNullReferences()
    {
        backgroundMusicSources.RemoveAll(source => source == null);
        soundEffectSources.RemoveAll(source => source == null);
    }
    
    /// <summary>
    /// Load volume settings from PlayerPrefs
    /// </summary>
    private void LoadVolumeSettings()
    {
        backgroundMusicVolume = PlayerPrefs.GetFloat("BackgroundMusicVolume", 0.7f);
        soundEffectsVolume = PlayerPrefs.GetFloat("SoundEffectsVolume", 0.8f);
    }
    
    /// <summary>
    /// Get debug info about registered audio sources
    /// </summary>
    public string GetDebugInfo()
    {
        CleanupNullReferences();
        return $"Audio Controller Status:\n" +
               $"Background Music: {backgroundMusicSources.Count} sources ({(backgroundMusicVolume * 100):F0}%)\n" +
               $"Sound Effects: {soundEffectSources.Count} sources ({(soundEffectsVolume * 100):F0}%)";
    }
    
    #endregion
    
    #region Unity Lifecycle
    
    void Update()
    {
        // Periodically clean up destroyed audio sources (every 5 seconds)
        if (Time.time % 5f < Time.deltaTime)
        {
            CleanupNullReferences();
        }
    }
    
    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    #endregion
}