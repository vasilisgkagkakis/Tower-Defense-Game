using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Audio Sources")]
    public AudioSource backgroundMusicSource;
    public AudioSource soundEffectsSource;
    
    [Header("Missile Audio")]
    private AudioSource missileThrustSource;  // Created dynamically for missile thrust
    
    [Header("Audio Clips")]
    public AudioClip[] backgroundMusicTracks;
    
    [Header("Missile Sound Effects")]
    public AudioClip missileLaunch;    // Used for missile launch (one-shot)
    public AudioClip missileThrust;    // Used for ongoing thrust (looping)
    public AudioClip explosionSound;  // Used when missiles explode
    
    [Header("Turret Sound Effects")]
    public AudioClip turret1Shoot;    // Turret 1 shooting sound
    public AudioClip turret3Shoot;    // Turret 3 shooting sound
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAudioSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        PlayBackgroundMusic();
        CreateMissileThrustSource();
        
        // Register our audio sources with the GlobalAudioController MANUALLY
        // This prevents auto-registration from causing conflicts
        if (GlobalAudioController.Instance != null)
        {
            if (backgroundMusicSource != null)
            {
                GlobalAudioController.Instance.RegisterBackgroundMusic(backgroundMusicSource);
                Debug.Log("🎵 AudioManager registered background music source");
            }
            if (soundEffectsSource != null)
            {
                GlobalAudioController.Instance.RegisterSoundEffect(soundEffectsSource);
                Debug.Log("🔊 AudioManager registered sound effects source");
            }
            if (missileThrustSource != null)
            {
                GlobalAudioController.Instance.RegisterSoundEffect(missileThrustSource);
                Debug.Log("🚀 AudioManager registered missile thrust source");
            }
        }
    }
    
    private void LoadAudioSettings()
    {
        // Settings loading is now handled by GlobalAudioController
        // This is kept for compatibility but does nothing
        Debug.Log("📋 AudioManager settings loading is handled by GlobalAudioController");
    }
    
    // Volume control is now handled by GlobalAudioController
    // These methods are kept for potential compatibility but do nothing
    public void SetBackgroundMusicVolume(float volume)
    {
        // Deprecated: Use GlobalAudioController.Instance.SetBackgroundMusicVolume() instead
        Debug.LogWarning("AudioManager.SetBackgroundMusicVolume is deprecated. Use GlobalAudioController instead.");
    }
    
    public void SetSoundEffectsVolume(float volume)
    {
        // Deprecated: Use GlobalAudioController.Instance.SetSoundEffectsVolume() instead  
        Debug.LogWarning("AudioManager.SetSoundEffectsVolume is deprecated. Use GlobalAudioController instead.");
    }
    
    public void PlayBackgroundMusic(int trackIndex = 0)
    {
        if (backgroundMusicSource != null && backgroundMusicTracks != null && trackIndex < backgroundMusicTracks.Length)
        {
            backgroundMusicSource.clip = backgroundMusicTracks[trackIndex];
            backgroundMusicSource.loop = true;
            backgroundMusicSource.Play();
        }
    }
    
    public void PlaySoundEffect(AudioClip clip)
    {
        if (soundEffectsSource != null)
        {
            if (clip != null)
            {
                soundEffectsSource.PlayOneShot(clip);
            }
            else
            {
                // If no specific clip provided, use default explosion sound
                if (explosionSound != null)
                {
                    soundEffectsSource.PlayOneShot(explosionSound);
                }
            }
        }
    }
    
    private void CreateMissileThrustSource()
    {
        if (missileThrustSource == null)
        {
            // Create a dedicated AudioSource for missile thrust
            missileThrustSource = gameObject.AddComponent<AudioSource>();
            missileThrustSource.playOnAwake = false;
            missileThrustSource.loop = true;
        }
    }
    
    // Missile sound methods - all controlled by sliders via GlobalAudioController
    public void PlayMissileLaunch() => PlaySoundEffect(missileLaunch);
    
    public void StartMissileThrust()
    {
        if (missileThrustSource != null && missileThrust != null && !missileThrustSource.isPlaying)
        {
            missileThrustSource.clip = missileThrust;
            missileThrustSource.Play();
            Debug.Log("🚀 Started missile thrust sound");
        }
    }
    
    public void StopMissileThrust()
    {
        if (missileThrustSource != null && missileThrustSource.isPlaying)
        {
            missileThrustSource.Stop();
            Debug.Log("🛑 Stopped missile thrust sound");
        }
    }
    
    public void PlayExplosion() => PlaySoundEffect(explosionSound);
    
    // Turret sound methods - all controlled by sliders via GlobalAudioController
    public void PlayTurret1Shoot() => PlaySoundEffect(turret1Shoot);
    public void PlayTurret3Shoot() => PlaySoundEffect(turret3Shoot);
}