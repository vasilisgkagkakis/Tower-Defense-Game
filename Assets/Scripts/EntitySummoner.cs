using UnityEngine;
using System.Collections.Generic;

public class EntitySummoner : MonoBehaviour
{
    public static List<Enemy> EnemiesInGame;
    public static Dictionary<int, GameObject> EnemyPrefabs;

    [SerializeField]
    private Transform spawnPointInstance; // Assign in Inspector

    public static Transform spawnPoint; // Static reference for static methods

    private static bool IsInitialized;

    void Awake()
    {
        spawnPoint = spawnPointInstance;
    }

    public static void Init()
    {
        if (!IsInitialized)
        {
            EnemyPrefabs = new Dictionary<int, GameObject>();
            EnemiesInGame = new List<Enemy>();

            EnemySummonData[] Enemies = Resources.LoadAll<EnemySummonData>("Enemies");
            Debug.Log("Found " + Enemies.Length + " enemy prefabs in Resources/Enemies");

            foreach (EnemySummonData enemyData in Enemies)
            {
                EnemyPrefabs.Add(enemyData.EnemyID, enemyData.EnemyPrefab);
            }

            IsInitialized = true;
        }
    }

    public static Enemy SummonEnemy(int EnemyID)
    {
        if (!EnemyPrefabs.ContainsKey(EnemyID))
        {
            Debug.LogError("Enemy ID " + EnemyID + " not found in EnemyPrefabs.");
            return null;
        }

        // Always create a new enemy - no pooling
        GameObject enemyObject = Instantiate(EnemyPrefabs[EnemyID], spawnPoint.position, Quaternion.identity);
        Enemy newEnemy = enemyObject.GetComponent<Enemy>();
        
        if (newEnemy != null)
        {
            newEnemy.ID = EnemyID;
            EnemiesInGame.Add(newEnemy);
            
            // Debug.Log($"Spawned new enemy with ID: {EnemyID} at {spawnPoint.position}");
        }
        else
        {
            Debug.LogError($"Enemy prefab with ID {EnemyID} doesn't have an Enemy component!");
            Destroy(enemyObject);
        }

        return newEnemy;
    }

    public static void RemoveEnemy(Enemy EnemyToRemove)
    {
        if (EnemyToRemove == null) return;

        // Remove from tracking list
        if (EnemiesInGame.Contains(EnemyToRemove))
        {
            EnemiesInGame.Remove(EnemyToRemove);
        }

        // Simply destroy the enemy - no pooling
        Debug.Log($"Destroying enemy: {EnemyToRemove.name}");
        Destroy(EnemyToRemove.gameObject);
    }
}