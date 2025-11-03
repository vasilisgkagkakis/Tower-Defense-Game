using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Main Menu Buttons")]
    public Button playButton;
    public Button instructionsButton;
    public Button quitButton;

    [Header("Instructions Panel")]
    public GameObject instructionsPanel;      // Panel containing instructions
    public Button closeInstructionsButton;   // Back button in instructions

    [Header("Scene Settings")]
    public string gameplaySceneName = "SampleScene"; // The main game scene

    void Start()
    {
        SetupButtons();

        // Hide instructions panel at start
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(false);
        }
    }

    private void SetupButtons()
    {
        // Setup main menu button listeners
        if (playButton != null)
        {
            playButton.onClick.AddListener(PlayGame);
        }

        if (instructionsButton != null)
        {
            instructionsButton.onClick.AddListener(ShowInstructions);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }

        if (closeInstructionsButton != null)
        {
            closeInstructionsButton.onClick.AddListener(CloseInstructions);
        }
    }

    public void PlayGame()
    {
        // Debug.Log("Starting game...");

        // Load the gameplay scene
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void ShowInstructions()
    {
        // Debug.Log("Showing instructions...");

        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(true);
        }
    }

    public void CloseInstructions()
    {
        // Debug.Log("Closing instructions...");

        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(false);
        }
    }

    public void QuitGame()
    {
        // Debug.Log("Quitting game...");
        Application.Quit();
    }

    void Update()
    {
        // Handle ESC key to close instructions
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (instructionsPanel != null && instructionsPanel.activeInHierarchy)
            {
                CloseInstructions();
            }
        }
    }
}