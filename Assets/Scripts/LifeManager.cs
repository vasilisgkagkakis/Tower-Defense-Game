using UnityEngine;
using TMPro;

public class LifeManager : MonoBehaviour
{
    [SerializeField] private int currentLives = 3;
    [SerializeField] private TMP_Text livesText;

    public static LifeManager Instance;

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
        UpdateLivesUI();
    }

    public int GetLives()
    {
        return currentLives;
    }
    
    public int GetCurrentLives()
    {
        return currentLives;
    }

    public bool LoseLife()
    {
        currentLives--;
        UpdateLivesUI();
        
        Debug.Log($"Life lost! Remaining lives: {currentLives}");
        
        if (currentLives <= 0)
        {
            Debug.Log("Game Over! No lives remaining!");
            OnGameOver();
            return true; // Game over
        }
        
        return false; // Game continues
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = currentLives.ToString();
        }
    }

    private void OnGameOver()
    {
        Debug.Log("🎮 GAME OVER - No lives remaining!");
        
        // Get current wave number
        int currentWave = 1; // Default fallback
        if (WaveManager.Instance != null)
        {
            currentWave = WaveManager.Instance.GetCurrentWave();
        }
        
        // Show game over UI
        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.ShowGameOver(currentWave);
        }
        else
        {
            Debug.LogError("GameOverManager.Instance is null! Make sure GameOverManager is in the scene.");
        }
    }
}