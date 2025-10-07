using UnityEngine;
using System.Collections.Generic;

public class EntitySummoner : MonoBehaviour
{
    public static List<Enemy> EnemiesInGame;
    public static Dictionary<int, GameObject> EnemyPrefabs;
    public static Dictionary<int, Queue<Enemy>> EnemyObjectPools;

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
            EnemyObjectPools = new Dictionary<int, Queue<Enemy>>();
            EnemiesInGame = new List<Enemy>();

            EnemySummonData[] Enemies = Resources.LoadAll<EnemySummonData>("Enemies");
            //Debug.Log("Found " + Enemies.Length + " enemy prefabs in Resources/Enemies");

            foreach (EnemySummonData enemyData in Enemies)
            {
                EnemyPrefabs.Add(enemyData.EnemyID, enemyData.EnemyPrefab);
                EnemyObjectPools.Add(enemyData.EnemyID, new Queue<Enemy>());
            }

            IsInitialized = true;
        }
        else
        {
            //Debug.LogWarning("EntitySummoner is already initialized.");
        }
    }

    public static Enemy SummonEnemy(int EnemyID)
    {
        Enemy SummonedEnemy = null;

        if (EnemyPrefabs.ContainsKey(EnemyID))
        {
            Queue<Enemy> ReferencedQueue = EnemyObjectPools[EnemyID];

            if (ReferencedQueue.Count > 0)
            {
                //Dequeue existing enemy from the pool
                SummonedEnemy = ReferencedQueue.Dequeue();
                SummonedEnemy.Init();

                SummonedEnemy.gameObject.SetActive(true);
                //Debug.Log("Reusing enemy from pool with ID: " + EnemyID);
            }
            else
            {
                GameObject enemyPrefab = Instantiate(EnemyPrefabs[EnemyID], spawnPoint.position, Quaternion.identity);
                SummonedEnemy = enemyPrefab.GetComponent<Enemy>();
                SummonedEnemy.Init();
                //Debug.Log("Instantiated new enemy with ID: " + EnemyID);
            }

            SummonedEnemy.ID = EnemyID;
            SummonedEnemy.Init();
            EnemiesInGame.Add(SummonedEnemy);
        }
        else
        {
            //("Enemy ID " + EnemyID + " not found in EnemyPrefabs.");
            return null;
        }

        EnemiesInGame.Add(SummonedEnemy);
        SummonedEnemy.ID = EnemyID;
        return SummonedEnemy;
    }

    public static void RemoveEnemy(Enemy EnemyToRemove)
    {
        EnemyObjectPools[EnemyToRemove.ID].Enqueue(EnemyToRemove);
        EnemyToRemove.gameObject.SetActive(false);
        EnemiesInGame.Remove(EnemyToRemove);
    }
}