using UnityEngine;

public class TowerTargeting
{
    public enum TargetType
    {
        First,
        Last,
        Close
    }

    public static Enemy GetTarget(TowerBehaviour CurrentTower, TargetType TargetMethod, float detectionRange = float.MaxValue)
    {
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemyObjects.Length == 0)
            return null;

        Enemy result = null;
        Vector3 towerPos = CurrentTower.transform.position;

        switch (TargetMethod)
        {
            case TargetType.First:
                float maxDistance = float.MinValue;
                foreach (var obj in enemyObjects)
                {
                    Enemy enemy = obj.GetComponent<Enemy>();
                    if (enemy == null || enemy.Health <= 0) continue; // Skip dead enemies
                    
                    // Check if enemy is within detection range
                    float distanceToTower = Vector3.Distance(towerPos, enemy.transform.position);
                    if (distanceToTower > detectionRange) continue; // Skip enemies out of range
                    
                    if (enemy.DistanceTravelled > maxDistance)
                    {
                        maxDistance = enemy.DistanceTravelled;
                        result = enemy;
                    }
                }
                break;

            case TargetType.Last:
                float minDistance = float.MaxValue;
                foreach (var obj in enemyObjects)
                {
                    if (!obj.TryGetComponent<Enemy>(out var enemy)) continue;
                    if (enemy.Health <= 0) continue; // Skip dead enemies
                    
                    // Check if enemy is within detection range
                    float distanceToTower = Vector3.Distance(towerPos, enemy.transform.position);
                    if (distanceToTower > detectionRange) continue; // Skip enemies out of range
                    
                    if (enemy.DistanceTravelled < minDistance)
                    {
                        minDistance = enemy.DistanceTravelled;
                        result = enemy;
                    }
                }
                break;

            case TargetType.Close:
                float closest = float.MaxValue;
                foreach (var obj in enemyObjects)
                {
                    Enemy enemy = obj.GetComponent<Enemy>();
                    if (enemy == null || enemy.Health <= 0) continue; // Skip dead enemies
                    
                    float dist = Vector3.Distance(towerPos, enemy.transform.position);
                    if (dist > detectionRange) continue; // Skip enemies out of range
                    
                    if (dist < closest)
                    {
                        closest = dist;
                        result = enemy;
                    }
                }
                break;
        }

        return result;
    }
}
