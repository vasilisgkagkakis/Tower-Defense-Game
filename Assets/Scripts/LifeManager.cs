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
            DontDestroyOnLoad(gameObject);
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
        // You can expand this to show game over screen, pause game, etc.
        Debug.Log("🎮 GAME OVER - Implement game over logic here!");
        
        // Example: Pause the game
        // Time.timeScale = 0f;
        
        // Example: Load game over scene
        // SceneManager.LoadScene("GameOverScene");
    }
}