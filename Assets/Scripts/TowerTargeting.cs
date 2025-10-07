using UnityEngine;

public class TowerTargeting
{
    public enum TargetType
    {
        First,
        Last,
        Close
    }

    public static Enemy GetTarget(TowerBehaviour CurrentTower, TargetType TargetMethod)
    {
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemyObjects.Length == 0)
            return null;

        Enemy result = null;

        switch (TargetMethod)
        {
            case TargetType.First:
                float maxDistance = float.MinValue;
                foreach (var obj in enemyObjects)
                {
                    Enemy enemy = obj.GetComponent<Enemy>();
                    if (enemy == null) continue;
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
                    Enemy enemy = obj.GetComponent<Enemy>();
                    if (enemy == null) continue;
                    if (enemy.DistanceTravelled < minDistance)
                    {
                        minDistance = enemy.DistanceTravelled;
                        result = enemy;
                    }
                }
                break;

            case TargetType.Close:
                float closest = float.MaxValue;
                Vector3 towerPos = CurrentTower.transform.position;
                foreach (var obj in enemyObjects)
                {
                    Enemy enemy = obj.GetComponent<Enemy>();
                    if (enemy == null) continue;
                    float dist = Vector3.Distance(towerPos, enemy.transform.position);
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
