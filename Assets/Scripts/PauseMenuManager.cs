using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager Instance;
    
    [Header("Pause Menu UI")]
    public GameObject pauseMenuPanel;      // The main container panel
    public GameObject pauseGameObject;     // Child: pause menu (buttons + sliders)  
    public GameObject instructionsGameObject; // Child: instructions screen
    
    [Header("Buttons")]
    public Button resumeButton;
    public Button instructionsButton;
    public Button mainMenuButton;
    public Button exitGameButton;
    public Button closeInstructionsButton;
    
    [Header("Audio Sliders")]
    public Slider backgroundMusicSlider;
    public Slider soundEffectsSlider;
    public TextMeshProUGUI backgroundMusicLabel;
    public TextMeshProUGUI soundEffectsLabel;
    
    [Header("Instructions")]
    public TextMeshProUGUI instructionsText;
    
    private bool isPaused = false;
    private float originalTimeScale = 1f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        SetupUI();
        LoadAudioSettings();
        SetupInstructions();
    }
    
    void Update()
    {
        HandlePauseInput();
    }
    
    private void HandlePauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Don't allow pause/unpause if game is over
            if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver())
            {
                return; // Block pause functionality during game over
            }
            
            if (isPaused)
            {
                // If instructions GameObject is active, go back to pause GameObject
                if (instructionsGameObject != null && instructionsGameObject.activeInHierarchy)
                {
                    CloseInstructions();
                }
                else
                {
                    // If pause GameObject is active, resume game
                    ResumeGame();
                }
            }
            else
            {
                // Game is not paused, so pause it
                PauseGame();
            }
        }
    }
    
    private void SetupUI()
    {
        // Hide main container initially (game starts unpaused)
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        
        // Set initial state: show pause, hide instructions
        if (pauseGameObject != null) pauseGameObject.SetActive(true);
        if (instructionsGameObject != null) instructionsGameObject.SetActive(false);
        
        // Setup button listeners
        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (instructionsButton != null) instructionsButton.onClick.AddListener(ShowInstructions);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(GoToMainMenu);
        if (exitGameButton != null) exitGameButton.onClick.AddListener(ExitGame);
        if (closeInstructionsButton != null) closeInstructionsButton.onClick.AddListener(CloseInstructions);
        
        // Setup slider listeners
        if (backgroundMusicSlider != null) backgroundMusicSlider.onValueChanged.AddListener(OnBackgroundMusicVolumeChanged);
        if (soundEffectsSlider != null) soundEffectsSlider.onValueChanged.AddListener(OnSoundEffectsVolumeChanged);
    }
    
    private void SetupInstructions()
    {
        if (instructionsText != null)
        {
            instructionsText.text = @"<size=24><b>TOWER DEFENSE CONTROLS</b></size>

<b>MOVEMENT:</b>
• WASD - Move camera around the map
• Mouse - Look around
• Left Shift - Sprint (faster movement)

<b>TOWER PLACEMENT:</b>
• 1, 2, 3 - Select tower type to place
• Left Click - Place selected tower
• ESC - Cancel tower placement

<b>TOWER MANAGEMENT:</b>
• Left Click - Select placed tower
• T - Change tower targeting mode
• U - Upgrade selected tower
• X - Sell selected tower

<b>WAVE SYSTEM:</b>
• Click ""Start Wave"" button to begin
• ESC - Pause/Resume game
• Survive increasingly difficult waves
• Boss waves every 5th wave (red button)

<b>ECONOMY:</b>
• Kill enemies to earn currency
• Complete waves for bonus rewards
• Spend currency on towers and upgrades
• Manage your resources wisely!

<b>GOAL:</b>
Defend your base from waves of enemies!
Don't let them reach the end zone!";
        }
    }
    
    public void PauseGame()
    {
        // Don't allow pausing if game is over
        if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver())
        {
            Debug.Log("Cannot pause - game is over");
            return;
        }
        
        isPaused = true;
        originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        
        // Show the main container panel
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        
        // Show pause GameObject, hide instructions GameObject
        if (pauseGameObject != null) pauseGameObject.SetActive(true);
        if (instructionsGameObject != null) instructionsGameObject.SetActive(false);
        
        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        Debug.Log("Game Paused - Showing Pause Menu");
    }
    
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = originalTimeScale;
        
        // Hide the entire pause menu panel (which hides both child objects)
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        
        // Hide cursor if needed (depends on your game's cursor behavior)
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
        
        Debug.Log("Game Resumed");
    }
    
    public void ShowInstructions()
    {
        // Hide pause GameObject, show instructions GameObject
        if (pauseGameObject != null) pauseGameObject.SetActive(false);
        if (instructionsGameObject != null) instructionsGameObject.SetActive(true);
        
        Debug.Log("Instructions Opened");
    }
    
    public void CloseInstructions()
    {
        // Hide instructions GameObject, show pause GameObject
        if (instructionsGameObject != null) instructionsGameObject.SetActive(false);
        if (pauseGameObject != null) pauseGameObject.SetActive(true);
        
        Debug.Log("Instructions Closed - Back to Pause Menu");
    }
    
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Reset time scale before loading scene
        // Replace "MainMenu" with your actual main menu scene name
        SceneManager.LoadScene("MainMenu");
    }
    
    public void ExitGame()
    {
        Debug.Log("Exiting Game...");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    private void OnBackgroundMusicVolumeChanged(float value)
    {
        // Use GlobalAudioController - it manages all audio sources automatically
        if (GlobalAudioController.Instance != null)
        {
            GlobalAudioController.Instance.SetBackgroundMusicVolume(value);
        }
        
        if (backgroundMusicLabel != null)
        {
            backgroundMusicLabel.text = $"Background Music: {Mathf.RoundToInt(value * 100)}%";
        }
    }
    
    private void OnSoundEffectsVolumeChanged(float value)
    {
        // Use GlobalAudioController - it manages all audio sources automatically
        if (GlobalAudioController.Instance != null)
        {
            GlobalAudioController.Instance.SetSoundEffectsVolume(value);
        }
        
        if (soundEffectsLabel != null)
        {
            soundEffectsLabel.text = $"Sound Effects: {Mathf.RoundToInt(value * 100)}%";
        }
    }
    
    private void LoadAudioSettings()
    {
        // Load saved audio settings
        float bgMusicVolume = PlayerPrefs.GetFloat("BackgroundMusicVolume", 0.7f);
        float sfxVolume = PlayerPrefs.GetFloat("SoundEffectsVolume", 0.8f);
        
        if (backgroundMusicSlider != null)
        {
            backgroundMusicSlider.value = bgMusicVolume;
        }
        
        if (soundEffectsSlider != null)
        {
            soundEffectsSlider.value = sfxVolume;
        }
        
        // Apply the volumes
        OnBackgroundMusicVolumeChanged(bgMusicVolume);
        OnSoundEffectsVolumeChanged(sfxVolume);
        
        // Apply volumes using GlobalAudioController
        if (GlobalAudioController.Instance != null)
        {
            GlobalAudioController.Instance.SetBackgroundMusicVolume(bgMusicVolume);
            GlobalAudioController.Instance.SetSoundEffectsVolume(sfxVolume);
        }
    }
    
    // Public method to check if game is paused
    public bool IsGamePaused()
    {
        return isPaused;
    }
    
    // Method for other scripts to pause/unpause
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
}