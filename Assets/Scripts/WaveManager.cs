using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("Wave Configuration")]
    public WaveData[] predefinedWaves; // For manually designed waves
    public bool useProceduralWaves = true; // Generate waves algorithmically

    [Header("Wave State")]
    public int currentWave = 0;
    public bool waveInProgress = false;
    public bool gameStarted = false;

    [Header("Enemy Spawning")]
    public Transform[] spawnPoints;
    public float baseSpawnDelay = 1.5f; // Increased from 1f for more breathing room
    public float spawnDelayVariation = 2f; // Random variation in spawn timing
    public float minSpawnDelay = 0.8f; // Minimum time between spawns

    [Header("Wave Progression")]
    public float difficultyMultiplier = 1.1f; // Health/count multiplier per wave
    public int enemiesPerWave = 5; // Base enemies per wav
    public float bossHealthMultiplier = 2f; // Boss has 2x regular enemy health

    [Header("UI References")]
    public Button startWaveButton;
    public TextMeshProUGUI waveInfoText;
    public TextMeshProUGUI enemiesRemainingText;

    [Header("Rewards")]
    public int baseWaveReward = 50;
    public int bossWaveReward = 200;

    // Private variables
    private int enemiesRemaining = 0;
    private bool waitingForWaveStart = true;

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
        UpdateAllUI();
        if (startWaveButton != null)
        {
            startWaveButton.onClick.AddListener(StartNextWave);
        }
    }

    void Update()
    {
        if (waveInProgress || waitingForWaveStart)
        {
            UpdateButtonUI(); // Update button state as needed
        }
    }

    public void StartNextWave()
    {
        if (waveInProgress || !waitingForWaveStart) return;
        
        // Don't start wave if game is paused
        if (PauseMenuManager.Instance != null && PauseMenuManager.Instance.IsGamePaused()) return;

        currentWave++;
        waveInProgress = true;
        waitingForWaveStart = false;
        gameStarted = true;

        if (startWaveButton != null)
            startWaveButton.interactable = false;
        
    

        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        bool isBossWave = (currentWave % 5 == 0); // Every 5th wave is boss wave

        UpdateAllUI();

        if (useProceduralWaves)
        {
            yield return StartCoroutine(SpawnProceduralWave(isBossWave));
        }
        else if (currentWave <= predefinedWaves.Length)
        {
            yield return StartCoroutine(SpawnPredefinedWave(predefinedWaves[currentWave - 1]));
        }
        else
        {
            // Fallback to procedural if we run out of predefined waves
            yield return StartCoroutine(SpawnProceduralWave(isBossWave));
        }

        // Wait for all enemies to be defeated or reach the end
        yield return new WaitUntil(() => enemiesRemaining <= 0);

        CompleteWave(isBossWave);
    }

    IEnumerator SpawnProceduralWave(bool isBossWave)
    {
        if (isBossWave)
        {
            yield return StartCoroutine(SpawnBossWave());
        }
        else
        {
            yield return StartCoroutine(SpawnRegularWave());
        }
    }

    IEnumerator SpawnRegularWave()
    {
        // Calculate wave difficulty
        int totalEnemies = Mathf.RoundToInt(enemiesPerWave + (currentWave * 0.75f)); // More enemies each wave
        float healthMultiplier = Mathf.Pow(difficultyMultiplier, currentWave - 1);

        // Mix of enemy types (IDs 1, 2, 3 for regular enemies)
        int[] enemyTypes = { 1, 2, 3 };

        Debug.Log($"Starting Wave {currentWave}: {totalEnemies} enemies, {healthMultiplier:F1}x health");

        for (int i = 0; i < totalEnemies; i++)
        {
            // Random enemy type (1, 2, or 3)
            int enemyID = enemyTypes[Random.Range(0, enemyTypes.Length)];

            // Each enemy type gets consistent health based on their type + wave multiplier
            float baseHealth = GetBaseEnemyHealth(enemyID);
            float waveHealth = baseHealth * healthMultiplier;

            SpawnEnemy(enemyID, waveHealth);

            // Random spawn delay with variation - gives player more breathing room
            float waveSpeedMultiplier = Mathf.Max(0.6f, 1f - (currentWave * 0.03f)); // Slower reduction per wave
            float randomVariation = Random.Range(-spawnDelayVariation, spawnDelayVariation);
            float spawnDelay = Mathf.Max(minSpawnDelay, (baseSpawnDelay + randomVariation) * waveSpeedMultiplier);
            
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    IEnumerator SpawnBossWave()
    {
        Debug.Log($"Starting Boss Wave {currentWave}!");

        // Spawn a few regular enemies first
        int regularEnemies = Mathf.Max(3, currentWave / 2);
        float healthMultiplier = Mathf.Pow(difficultyMultiplier, currentWave - 1);

        for (int i = 0; i < regularEnemies; i++)
        {
            // Only spawn regular enemies (1, 2, 3) - not boss (4)
            int[] regularEnemyTypes = { 1, 2, 3 };
            int enemyID = regularEnemyTypes[Random.Range(0, regularEnemyTypes.Length)];
            float baseHealth = GetBaseEnemyHealth(enemyID);
            SpawnEnemy(enemyID, baseHealth * healthMultiplier);
            
            // Random delay between regular enemies in boss wave
            float randomDelay = Random.Range(0.8f, 1.5f);
            yield return new WaitForSeconds(randomDelay);
        }

        yield return new WaitForSeconds(3f); // Longer pause before boss (increased from 2f)

        // Spawn the boss (ID 4)
        float bossHealth = GetBaseEnemyHealth(4) * healthMultiplier * bossHealthMultiplier;
        SpawnEnemy(4, bossHealth);

        Debug.Log($"Boss spawned with {bossHealth} health!");
    }

    IEnumerator SpawnPredefinedWave(WaveData waveData)
    {
        Debug.Log($"Starting predefined wave: {waveData.waveName}");

        foreach (EnemySpawnData enemyGroup in waveData.enemies)
        {
            for (int i = 0; i < enemyGroup.count; i++)
            {
                SpawnEnemy(enemyGroup.enemyID, enemyGroup.health);
                yield return new WaitForSeconds(enemyGroup.spawnDelay);
            }
            yield return new WaitForSeconds(waveData.timeBetweenEnemyGroups);
        }
    }

    void SpawnEnemy(int enemyID, float health)
    {
        if (EntitySummoner.Instance != null && spawnPoints.Length > 0)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemy = EntitySummoner.Instance.SummonEnemy(enemyID, spawnPoint.position);

            if (enemy != null)
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.Health = health;
                    enemyScript.ID = enemyID;
                }

                enemiesRemaining++;
                UpdateEnemiesRemainingUI();
            }
        }
    }

    float GetBaseEnemyHealth(int enemyID)
    {
        // Base health values for each enemy type
        switch (enemyID)
        {
            case 1: return 50f;  // Regular enemy 1
            case 2: return 55f;  // Regular enemy 2  
            case 3: return 60f;  // Regular enemy 3
            case 4: return 300f; // Boss enemy
            default: return 50f;
        }
    }

    public void OnEnemyDefeated()
    {
        enemiesRemaining--;
        UpdateEnemiesRemainingUI();

        if (enemiesRemaining <= 0 && waveInProgress)
        {
            // Wave completed - this will be handled by the coroutine
        }
    }

    public void OnEnemyReachedEnd()
    {
        enemiesRemaining--;
        UpdateEnemiesRemainingUI();
    }

    void CompleteWave(bool wasBossWave)
    {
        waveInProgress = false;
        waitingForWaveStart = true;

        // Give wave completion reward
        int reward = wasBossWave ? bossWaveReward : baseWaveReward;
        reward += currentWave * 10; // Bonus based on wave number

        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCurrency(reward);
        }

        Debug.Log($"Wave {currentWave} completed! Reward: {reward} currency");

        UpdateAllUI();
    }

    public void UpdateAllUI()
    {
        UpdateWaveUI();
        UpdateEnemiesRemainingUI();
        UpdateButtonUI();
    }

    public void UpdateWaveUI()
    {
        if (waveInfoText != null)
        {
            if (waitingForWaveStart)
            {
                bool nextIsBoss = ((currentWave + 1) % 5 == 0);
                string waveType = nextIsBoss ? "BOSS WAVE" : "Wave";
                waveInfoText.text = $"Ready for {waveType} {currentWave + 1}";
            }
            else
            {
                bool isBoss = (currentWave % 5 == 0);
                string waveType = isBoss ? "BOSS WAVE" : "Wave";
                waveInfoText.text = $"{waveType} {currentWave} in progress";
            }
        }
    }

    public void UpdateEnemiesRemainingUI()
    {
        if (enemiesRemainingText != null)
        {
            enemiesRemainingText.text = $"Enemies: {enemiesRemaining}";
        }
    }

    public void UpdateButtonUI()
    {
        if (startWaveButton != null)
        {
            bool nextIsBossWave = ((currentWave + 1) % 5 == 0);

            // Update button interactability
            startWaveButton.interactable = !waveInProgress && waitingForWaveStart;

            // Update button text
            TextMeshProUGUI buttonText = startWaveButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                if (waveInProgress)
                {
                    buttonText.text = "Wave in Progress...";
                }
                else
                {
                    string waveType = nextIsBossWave ? "Start Boss Wave" : "Start Wave";
                    buttonText.text = $"{waveType} {currentWave + 1}";
                }
            }

            // Change button color for boss waves
            ColorBlock colors = startWaveButton.colors;
            if (nextIsBossWave)
            {
                colors.normalColor = new Color(0f, 1f, 1f, 1f);
                colors.highlightedColor = new Color(0f, 1f, 1f, 0.8f);
            }
            else
            {
                colors.normalColor = Color.white;
                colors.highlightedColor = new Color(0.96f, 0.96f, 0.96f, 1f);
            }
            startWaveButton.colors = colors;
        }
    }

    public bool IsWaveInProgress()
    {
        return waveInProgress;
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }
}