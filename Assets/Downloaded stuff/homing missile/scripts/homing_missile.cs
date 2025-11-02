using UnityEngine;

namespace HomingMissile
{
    public class homing_missile : MonoBehaviour
    {
        public int speed = 60;
        public float explosionRadius = 3f;
        public float explosionDamage = 350f;
        public GameObject[] explosionEffect;
        public int timebeforeactivition = 1;
        public int timebeforebursting = 1;
        public int timebeforedestruction = 1;
        public int timealive;
        public GameObject target;
        public Rigidbody projectilerb;
        public bool isactive = false;
        public GameObject targetpointer;
        public float turnSpeed = 0.15f;
        public AudioSource launch_sound;
        public AudioSource thrust_sound;
        public GameObject smoke_obj;
        public ParticleSystem smoke;
        public GameObject smoke_position;
        public GameObject destroy_effect;
        public bool rotate = true;
        public bool velocity = true;

        [Header("Smart Targeting")]
        public float retargetRange = 20f; // Range to look for new targets
        private float retargetCheckInterval = 0.25f; // Check every 0.25 seconds
        private float retargetTimer = 0f;
        
        [Header("Predictive Targeting")]
        [Range(0f, 1f)]
        public float predictionStrength = 0.7f; // How much to lead the target (0 = no prediction, 1 = full prediction)
        
        [Header("Turret Integration")]
        [HideInInspector]
        public TowerBehaviour originatingTurret; // Reference to the turret that fired this missile

        private void Start()
        {
            projectilerb = GetComponent<Rigidbody>();
        }

        public void usemissile()
        {
            launch_sound.Play();
            isactive = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Only explode if hitting an enemy parent collider
            if (other.CompareTag("Enemy"))
            {
                Explode();
            }
        }

        private void Explode()
        {
            if (explosionEffect != null)
            {
                if (explosionEffect.Length > 0 && explosionEffect[0] != null)
                {
                    // Spawn a random explosion effect from the array
                    int randomIndex = Random.Range(0, explosionEffect.Length);
                    Instantiate(explosionEffect[randomIndex], transform.position, Quaternion.identity);
                }
            }

            // Area damage
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (var hit in hitColliders)
            {
                if (hit.CompareTag("Enemy"))
                {
                    Enemy enemy = hit.GetComponentInParent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.Health -= explosionDamage;
                    }
                }
            }

            Destroy(gameObject);
        }

        private void CheckAndRetargetIfNeeded()
        {
            retargetTimer += Time.fixedDeltaTime;

            // Only check periodically to avoid performance issues
            if (retargetTimer < retargetCheckInterval) return;
            retargetTimer = 0f;

            // Check if current target is dead or invalid
            if (target == null || !target.activeInHierarchy || IsTargetDead())
            {
                GameObject newTarget = FindTargetUsingTurretLogic();
                if (newTarget != null)
                {
                    // Retarget to new enemy
                    target = newTarget;
                    if (targetpointer != null)
                    {
                        var pointerScript = targetpointer.GetComponent<homing_missile_pointer>();
                        if (pointerScript != null)
                        {
                            pointerScript.target = newTarget;
                        }
                    }
                    Debug.Log($"🎯 Missile retargeted to {newTarget.name}");
                }
                else
                {
                    // No targets available - explode
                    Debug.Log("💥 No targets available - missile exploding");
                    Explode();
                }
            }
        }

        private bool IsTargetDead()
        {
            if (target == null) return true;

            if (target.TryGetComponent<Enemy>(out var enemy))
            {
                return enemy.Health <= 0;
            }

            return false; // If no Enemy component, assume alive
        }

        private GameObject FindTargetUsingTurretLogic()
        {
            // If we have an originating turret, use its targeting logic
            if (originatingTurret != null)
            {
                // Try to get the turret's detection range (missiles should use a larger range for retargeting)
                float turretRange = float.MaxValue; // Default to no range limit
                
                // Check if the turret has a TurretAim or TurretMissileAim component for range
                var turretAim = originatingTurret.GetComponent<TurretAim>();
                if (turretAim != null)
                {
                    turretRange = turretAim.detectionRange * 2f; // Missiles get double range for retargeting
                }
                else
                {
                    var missileAim = originatingTurret.GetComponent<TurretMissileAim>();
                    if (missileAim != null)
                    {
                        turretRange = missileAim.detectionRange * 2f; // Missiles get double range for retargeting
                    }
                }
                
                Enemy enemyTarget = TowerTargeting.GetTarget(originatingTurret, originatingTurret.targetType, turretRange);
                if (enemyTarget != null)
                {
                    Debug.Log($"🎯 Missile using turret targeting logic ({originatingTurret.targetType}) with range {turretRange}: {enemyTarget.name}");
                    return enemyTarget.gameObject;
                }
            }

            // Fallback to closest enemy if no turret reference
            Debug.Log("🎯 Missile fallback to closest targeting (no turret reference)");
            return FindClosestLivingEnemy();
        }

        private GameObject FindClosestLivingEnemy()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject closest = null;
            float closestDistance = retargetRange;

            foreach (GameObject enemy in enemies)
            {
                if (enemy == null || !enemy.activeInHierarchy) continue;

                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null && enemyScript.Health <= 0) continue; // Skip dead enemies

                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closest = enemy;
                    closestDistance = distance;
                }
            }

            return closest;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);

            // Draw retarget range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, retargetRange);
        }

        [System.Obsolete]
        void FixedUpdate()
        {
            if (isactive)
            {
                if (timealive == timebeforeactivition)
                {
                    thrust_sound.Play();
                }
                timealive++;
                if (timealive == timebeforebursting)
                {
                    smoke = Instantiate(smoke_obj, smoke_position.transform.position, smoke_position.transform.rotation).GetComponent<ParticleSystem>();
                    smoke.Play();
                    smoke.transform.SetParent(transform);
                }
                if (timealive >= timebeforebursting && timealive < timebeforedestruction)
                {
                    // Check if current target is still valid
                    CheckAndRetargetIfNeeded();

                    if (rotate)
                    {
                        // Optional: Dynamic turn speed based on distance to target
                        float dynamicTurnSpeed = turnSpeed;
                        if (target != null)
                        {
                            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
                            // Turn faster when closer to target (within 5 units)
                            if (distanceToTarget < 5f)
                            {
                                dynamicTurnSpeed = turnSpeed * 2f; // Double the turn speed when close
                            }
                        }
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetpointer.transform.rotation, dynamicTurnSpeed);
                    }
                    if (velocity) projectilerb.velocity = transform.forward * speed;
                }
            }
        }
    }
}