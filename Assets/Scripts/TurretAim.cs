using UnityEngine;

public class TurretAim : MonoBehaviour
{
    public Transform turretMount;            // Rotating part of the turret
    public float turnSpeed = 5f;
    public float detectionRange = 15f;
    public float yRotationOffset = 0f;
    public float zRotationOffset = 0f;
    private Transform target;
    private TowerBehaviour towerBehaviour;

    [SerializeField] Transform[] StartPoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] LineRenderer bulletTrail;

    private float fireInterval = 1f; // Will be set from TowerData
    private float lastShotTime = -999f; // Track when last barrel fired
    private int currentBarrel = 0; // Which barrel to fire next (0-3 for 4 barrels)
    private float barrelDelay; // Time between each barrel shot

    void Awake()
    {
        towerBehaviour = GetComponent<TowerBehaviour>();

        // Set fire interval from tower data
        if (towerBehaviour != null && towerBehaviour.towerData != null)
        {
            fireInterval = towerBehaviour.towerData.fireInterval;
        }

        // Calculate delay between barrels
        barrelDelay = StartPoint.Length > 0 ? fireInterval / StartPoint.Length : 1f;


        // Initialize so turret can fire immediately when placed
        lastShotTime = -999f;
        currentBarrel = 0;
    }

    void Update()
    {
        // Only function if tower is placed (not in preview mode)
        if (towerBehaviour == null || !towerBehaviour.isPlaced)
        {
            target = null; // Clear target when not placed
            return;
        }

        FindTargetByType();

        if (target != null)
        {
            Vector3 direction = target.position - turretMount.position;
            direction.y = 0f; // Only rotate around Y

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                Quaternion adjustedRotation = lookRotation * Quaternion.Euler(0, yRotationOffset, zRotationOffset);
                turretMount.rotation = Quaternion.Slerp(turretMount.rotation, adjustedRotation, Time.deltaTime * turnSpeed);
            }

            // Fire timing - only when tower is placed (don't require perfect aim for hitscan)
            float timeSinceLastShot = Time.time - lastShotTime;


            // Check if it's time to fire next barrel
            if (timeSinceLastShot >= barrelDelay)
            {
                // Update lastShotTime IMMEDIATELY to prevent multiple fires this frame
                lastShotTime = Time.time;
                FireSingleBarrel();
            }
        }
        else
        {
            // When no target, only reset after a longer period to avoid breaking sequences
            if (lastShotTime > 0 && Time.time - lastShotTime > barrelDelay * 3)
            {
                currentBarrel = 0;
                lastShotTime = -999f;
                // Debug.Log($"[{Time.time:F2}] No target for >3 barrel delays, resetting sequence for immediate fire");
            }
        }
    }

    void FindTargetByType()
    {
        if (towerBehaviour == null || !towerBehaviour.isPlaced)
        {
            target = null;
            return;
        }

        Enemy enemy = TowerTargeting.GetTarget(towerBehaviour, towerBehaviour.targetType, detectionRange);
        if (enemy != null)
            target = enemy.transform;
        else
            target = null;
    }

    private void FireSingleBarrel()
    {
        if (target == null || StartPoint.Length == 0) return;

        // Get the current barrel to fire
        var sp = StartPoint[currentBarrel];
        if (sp == null) return;

        Vector3 start = sp.position;
        Vector3 end;

        // Safely try to find AimPoint
        Transform aimPoint = target.Find("AimPoint");
        if (aimPoint != null)
        {
            end = aimPoint.position;
        }
        else
        {
            end = target.position;
        }

        // Create bullet trail effect
        GameObject bulletTrailEffect = Instantiate(bulletTrail.gameObject, start, Quaternion.identity);
        LineRenderer lineR = bulletTrailEffect.GetComponent<LineRenderer>();
        lineR.SetPosition(0, start);
        lineR.SetPosition(1, end);
        Destroy(bulletTrailEffect, 1f);

        // Play turret shooting sound
        PlayTurretShootSound();

        // Apply damage instantly (hitscan style)
        ApplyInstantDamage(target);

        // Fire bullet prefab
        FireBulletFromPoint(sp, end);

        // Update barrel index (lastShotTime already updated in caller)
        currentBarrel = (currentBarrel + 1) % StartPoint.Length; // Cycle through barrels
    }

    private void ApplyInstantDamage(Transform target)
    {
        if (target == null || towerBehaviour == null) return;

        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            // Apply damage instantly (hitscan style)
            enemy.TakeDamage(towerBehaviour.currentDamage);
            // Debug.Log($"Turret hit {enemy.name} for {towerBehaviour.currentDamage} damage!");
        }
    }

    private void FireBulletFromPoint(Transform sp, Vector3 end)
    {
        Vector3 start = sp.position;
        Vector3 direction = (end - start).normalized;

        float maxAngle = 30f;
        float randomAngle = Random.Range(-maxAngle, maxAngle);
        Quaternion randomRot = Quaternion.AngleAxis(randomAngle, Vector3.up);
        Vector3 spreadDirection = randomRot * direction;

        if (bulletPrefab != null)
        {
            // Make -Z face the direction
            Quaternion rot = Quaternion.LookRotation(spreadDirection) * Quaternion.Euler(0, 180f, 0);
            GameObject bullet = Instantiate(bulletPrefab, start, rot);

            // The bullet prefab is just visual decoration that falls to ground
            // Damage is applied instantly via LineRenderer in ApplyInstantDamage()
            // Give a small forward force (along -Z)
            if (bullet.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.AddForce(-bullet.transform.forward, ForceMode.Impulse);
            }

            Destroy(bullet, 2f);
        }
    }

    private void PlayTurretShootSound()
    {
        if (AudioManager.Instance == null) return;

        // Identify turret type by TowerData name or GameObject name
        if (towerBehaviour != null && towerBehaviour.towerData != null)
        {
            string turretName = towerBehaviour.towerData.turretName.ToLower();

            if (turretName.Contains("basic"))
            {
                AudioManager.Instance.PlayTurret1Shoot();
                return;
            }
            else if (turretName.Contains("advanced"))
            {
                AudioManager.Instance.PlayTurret3Shoot();
                return;
            }
        }
    }
}
