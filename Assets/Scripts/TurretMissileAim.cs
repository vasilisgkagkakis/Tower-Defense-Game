using UnityEngine;
using HomingMissile;

public class TurretMissileAim : MonoBehaviour
{
    public Transform turretMount;            // Rotating part of the turret
    public float turnSpeed = 5f;
    public float detectionRange = 20f;
    public float yRotationOffset = 0f;   
    public float zRotationOffset = 0f;     
    private Transform target;
    private TowerBehaviour towerBehaviour;

    [Header("Missile Settings")]
    [SerializeField] Transform[] StartPoint;
    [SerializeField] GameObject missilePrefab;

    private float fireTimer = 0f;
    private float fireInterval = 2f; // Will be set from TowerData

    void Awake()
    {
        towerBehaviour = GetComponent<TowerBehaviour>();
        
        // Set fire interval from tower data
        if (towerBehaviour != null && towerBehaviour.towerData != null)
        {
            fireInterval = towerBehaviour.towerData.fireInterval;
        }
        
        // Initialize fireTimer to fireInterval so turret can fire immediately when placed
        fireTimer = fireInterval;
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

            // Fire timer - only when tower is placed
            fireTimer += Time.deltaTime;
            if (fireTimer >= fireInterval)
            {
                ShootMissile();
                fireTimer = 0f;
            }
        }
        else
        {
            fireTimer = fireInterval; // Ready to fire immediately when a target appears
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

    private void ShootMissile()
    {
        if (target == null || missilePrefab == null) return;
        
        // Double-check target is still valid (enemy could have died between Update and this call)
        if (target.gameObject == null || !target.gameObject.activeInHierarchy)
        {
            // Debug.Log("Target became invalid before missile launch");
            return;
        }
        
        // Use first spawn point (can be extended to use multiple like bullets)
        Transform spawnPoint = StartPoint.Length > 0 ? StartPoint[0] : transform;
        
        // Instantiate missile
        GameObject missile = Instantiate(missilePrefab, spawnPoint.position, spawnPoint.rotation);
        var missileScript = missile.GetComponent<homing_missile>();
        
        if (missileScript != null)
        {
            // Set originating turret reference for consistent targeting
            missileScript.originatingTurret = towerBehaviour;
            
            // Set target
            missileScript.target = target.gameObject;
            
            // Set damage from tower
            missileScript.explosionDamage = towerBehaviour.currentDamage;
            
            // Set target pointer
            if (missileScript.targetpointer != null)
            {
                var pointerScript = missileScript.targetpointer.GetComponent<homing_missile_pointer>();
                if (pointerScript != null)
                {
                    pointerScript.target = target.gameObject;
                }
            }
            
            // Activate missile
            missileScript.usemissile();
            
            // Debug.Log($"Missile launched at {target.name} with {towerBehaviour.currentDamage} damage (targeting: {towerBehaviour.targetType})");
        }
    }
}