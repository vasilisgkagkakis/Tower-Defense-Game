using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
        public float turnSpeed = 0.035f;
        public AudioSource launch_sound;
        public AudioSource thrust_sound;
        public GameObject smoke_obj;
        public ParticleSystem smoke;
        public GameObject smoke_position;
        public GameObject destroy_effect;
        public bool rotate = true;
        public bool velocity = true;

        private void Start()
        {
            projectilerb = this.GetComponent<Rigidbody>();
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
                    GameObject effect = Instantiate(explosionEffect[randomIndex], transform.position, Quaternion.identity);
                    // Destroy(effect, 2f);
                }
            }

            // Area damage (optional, if you want splash)
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
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
                    smoke.transform.SetParent(this.transform);
                }
                if (timealive >= timebeforebursting && timealive < timebeforedestruction)
                {
                    if(rotate)transform.rotation = Quaternion.Slerp(transform.rotation, targetpointer.transform.rotation, turnSpeed);
                    if(velocity)projectilerb.velocity = transform.forward * speed;
                }
            }
        }
    }
}