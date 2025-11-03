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
    public string mainMenuSceneName = "MainMenu";   // Main menu scene name

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

        // Debug.Log($"Game Over! Current wave: {currentWave}, Best wave: {bestWave}");
    }

    public void RestartGame()
    {
        // Debug.Log("Restarting game...");

        // Resume time scale
        Time.timeScale = 1f;

        // Reload current scene
        SceneManager.LoadScene(currentSceneName);
    }

    public void GoToMainMenu()
    {
        // Debug.Log("Going to main menu...");

        // Resume time scale
        Time.timeScale = 1f;

        // Load main menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Check if game is currently in game over state
    public bool IsGameOver()
    {
        return gameOverPanel != null && gameOverPanel.activeSelf;
    }
}