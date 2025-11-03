using UnityEngine;

public class EndZoneTrigger : MonoBehaviour
{

    void Start()
    {
        // Ensure this object has a collider set as trigger
        if (!TryGetComponent<Collider>(out var col))
        {
            // Debug.LogError("EndZoneTrigger: No collider found! Please add a collider component.");
            return;
        }

        if (!col.isTrigger)
        {
            // Debug.LogWarning("EndZoneTrigger: Collider is not set as trigger. Setting isTrigger = true.");
            col.isTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Debug.Log($"OnTriggerExit: {other.name} exited end zone (Tag: {other.tag})");

        // Check if the object leaving the trigger is an enemy
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<Enemy>(out var enemy))
            {
                // Only lose life if enemy is still alive (not already dead/destroyed)
                if (enemy.Health > 0)
                {
                    // Debug.Log($"Enemy {enemy.name} reached the end zone! Losing 1 life...");

                    // Lose a life
                    if (LifeManager.Instance != null)
                    {
                        LifeManager.Instance.LoseLife();
                    }
                    else
                    {
                        Debug.LogError("LifeManager.Instance is null! Make sure LifeManager is in the scene.");
                    }

                    // Notify wave manager that enemy reached the end
                    if (WaveManager.Instance != null)
                    {
                        WaveManager.Instance.OnEnemyReachedEnd();
                    }

                    // Remove the enemy that reached the end
                    if (EntitySummoner.EnemiesInGame.Contains(enemy))
                    {
                        EntitySummoner.RemoveEnemy(enemy);
                    }
                    else
                    {
                        // Fallback if enemy isn't in the summoner list
                        Destroy(enemy.gameObject);
                    }
                }
            }
        }
    }
}