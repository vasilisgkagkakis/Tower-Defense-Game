using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float Health;
    public int ID;

    public float DistanceTravelled { get; private set; }
    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
        DistanceTravelled = 0f;
    }

    void Update()
    {
        DistanceTravelled += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        if (Health <= 0)
        {
            GetComponent<Ragdoll>().EnableRagdoll(transform.position);
            GetComponent<NavMeshAgent>().enabled = false;
        }
    }

    public void TakeDamage(float damage)
    {
        if (Health <= 0) return; // Already dead

        Health -= damage;

        if (Health <= 0)
        {
            Health = 0;
            Die();
        }
    }

    private void Die()
    {
        // Grant currency reward to player based on wave difficulty
        if (CurrencyManager.Instance != null && WaveManager.Instance != null)
        {
            // Scale kill reward with wave progression
            int baseKillReward = 5;
            int currentWave = WaveManager.Instance.GetCurrentWave();
            
            // Formula: 5 + (wave * 2) - gives reasonable scaling
            // Wave 1: 7, Wave 5: 15, Wave 10: 25, Wave 20: 45
            int reward = baseKillReward + (currentWave * 2);
            
            // Boss enemies (ID 4) give extra reward
            if (ID == 4)
            {
                reward *= 3; // Boss gives 3x normal kill reward
            }
            
            CurrencyManager.Instance.AddCurrency(reward);
        }

        // Notify wave manager
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnEnemyDefeated();
        }
    }
}