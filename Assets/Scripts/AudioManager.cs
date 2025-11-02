using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Audio Sources")]
    public AudioSource backgroundMusicSource;
    public AudioSource soundEffectsSource;
    
    [Header("Audio Clips")]
    public AudioClip[] backgroundMusicTracks;
    public AudioClip explosionSound;
    
    private float backgroundMusicVolume = 0.7f;
    private float soundEffectsVolume = 0.8f;
    
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
        }
    }
    
    private void LoadAudioSettings()
    {
        backgroundMusicVolume = PlayerPrefs.GetFloat("BackgroundMusicVolume", 0.7f);
        soundEffectsVolume = PlayerPrefs.GetFloat("SoundEffectsVolume", 0.8f);
        
        UpdateAudioSources();
    }
    
    private void UpdateAudioSources()
    {
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = backgroundMusicVolume;
        }
        
        if (soundEffectsSource != null)
        {
            soundEffectsSource.volume = soundEffectsVolume;
        }
    }
    
    public void SetBackgroundMusicVolume(float volume)
    {
        backgroundMusicVolume = volume;
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = volume;
        }
        PlayerPrefs.SetFloat("BackgroundMusicVolume", volume);
    }
    
    public void SetSoundEffectsVolume(float volume)
    {
        soundEffectsVolume = volume;
        if (soundEffectsSource != null)
        {
            soundEffectsSource.volume = volume;
        }
        PlayerPrefs.SetFloat("SoundEffectsVolume", volume);
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
    
    // Convenience method for explosion sound (used by missiles)
    public void PlayExplosion() => PlaySoundEffect(explosionSound);
    
    public float GetBackgroundMusicVolume() => backgroundMusicVolume;
    public float GetSoundEffectsVolume() => soundEffectsVolume;
}