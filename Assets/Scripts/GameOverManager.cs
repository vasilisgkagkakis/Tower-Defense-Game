using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public Button restartButton;
    public Button mainMenuButton;

    [Header("Scene Names")]
    public string currentSceneName = "SampleScene"; // The gameplay scene name
    public string mainMenuSceneName = "MainMenu";   // Main menu scene name (create if needed)

    private void Awake()
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

    private void Start()
    {
        // Hide game over panel at start
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Set up button listeners
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }
    }

    public void ShowGameOver(int currentWave)
    {
        // Get best wave from PlayerPrefs
        int bestWave = PlayerPrefs.GetInt("BestWave", 1);
        
        // Update best wave if current is better
        if (currentWave > bestWave)
        {
            bestWave = currentWave;
            PlayerPrefs.SetInt("BestWave", bestWave);
            PlayerPrefs.Save();
        }

        // Update game over text
        if (gameOverText != null)
        {
            gameOverText.text = $"GAME OVER\nYou made it to wave {currentWave}\n(Best: {bestWave})";
        }

        // Show game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Pause the game
        Time.timeScale = 0f;

        // Play game over sound if audio manager exists
        if (AudioManager.Instance != null)
        {
            // You can add a game over sound effect here if you have one
            // AudioManager.Instance.PlayGameOverSound();
        }

        Debug.Log($"Game Over! Current wave: {currentWave}, Best wave: {bestWave}");
    }

    public void RestartGame()
    {
        Debug.Log("Restarting game...");
        
        // Resume time scale
        Time.timeScale = 1f;
        
        // Reload current scene (all managers will reset naturally)
        SceneManager.LoadScene(currentSceneName);
    }

    public void GoToMainMenu()
    {
        Debug.Log("Going to main menu...");
        
        // Resume time scale
        Time.timeScale = 1f;
        
        // Load main menu scene
        // For now, just restart the current scene (you can create a proper main menu later)
        SceneManager.LoadScene(currentSceneName);
        
        // If you have a dedicated main menu scene, uncomment this:
        // SceneManager.LoadScene(mainMenuSceneName);
    }

    // Method to hide game over panel (if needed for other purposes)
    public void HideGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        Time.timeScale = 1f;
    }

    // Check if game is currently in game over state
    public bool IsGameOver()
    {
        return gameOverPanel != null && gameOverPanel.activeSelf;
    }

    // Get the current wave for game over display
    private int GetCurrentWave()
    {
        if (WaveManager.Instance != null)
        {
            return WaveManager.Instance.GetCurrentWave();
        }
        return 1; // Default fallback
    }
}