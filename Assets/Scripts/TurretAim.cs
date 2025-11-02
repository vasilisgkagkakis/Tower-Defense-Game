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

    private float fireTimer = 0f;
    public float fireInterval = 1f; // seconds between shots

    void Awake()
    {
        towerBehaviour = GetComponent<TowerBehaviour>();
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
                SpawnBulletTrail();
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

    private void SpawnBulletTrail()
    {
        if (target == null) return;
        StartCoroutine(FireAllBarrels());
    }

    private System.Collections.IEnumerator FireAllBarrels()
    {
        int count = StartPoint.Length;
        if (count == 0) yield break;
        float delay = fireInterval / count;

        for (int i = 0; i < count; i++)
        {
            // Check if target is still valid before each shot
            if (target == null)
            {
                // Debug.Log("Target became null during firing sequence - stopping barrage");
                yield break;
            }

            var sp = StartPoint[i];
            if (sp == null) continue;
            
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

            GameObject bulletTrailEffect = Instantiate(bulletTrail.gameObject, start, Quaternion.identity);
            LineRenderer lineR = bulletTrailEffect.GetComponent<LineRenderer>();
            lineR.SetPosition(0, start);
            lineR.SetPosition(1, end);
            Destroy(bulletTrailEffect, 1f);

            FireBulletFromPoint(sp, end);

            if (i < count - 1)
                yield return new WaitForSeconds(delay);
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

            // Give a small forward force (along -Z)
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(-bullet.transform.forward, ForceMode.Impulse); // -forward for -Z
            }

            Destroy(bullet, 2f);
        }
    }
}
