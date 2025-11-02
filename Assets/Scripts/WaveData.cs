using UnityEngine;

[System.Serializable]
public class EnemySpawnData
{
    public int enemyID;
    public int count;
    public float health;
    public float spawnDelay; // Delay between spawning this enemy type
}

[CreateAssetMenu(fileName = "WaveData", menuName = "Tower Defense/Wave Data")]
public class WaveData : ScriptableObject
{
    [Header("Wave Information")]
    public int waveNumber;
    public string waveName;
    
    [Header("Enemy Spawning")]
    public EnemySpawnData[] enemies;
    public float timeBetweenEnemyGroups = 1f; // Time between different enemy types
    
    [Header("Rewards")]
    public int completionReward = 50; // Base reward for completing wave
    
    [Header("Special Properties")]
    public bool isBossWave = false;
    public string waveDescription;
}