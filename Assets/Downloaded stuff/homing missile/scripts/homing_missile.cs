using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HomingMissile
{
    public class homing_missile : MonoBehaviour
    {
        public int speed = 60;
        public int downspeed = 30;
        public int damage = 35;
        public bool fully_active = false;
        public int timebeforeactivition = 1;
        public int timebeforebursting = 1;
        public int timebeforedestruction = 1;
        public int timealive;
        public GameObject target;
        public GameObject shooter;
        public Rigidbody projectilerb;
        public bool isactive = false;
        public Vector3 sleepposition;
        public GameObject targetpointer;
        public float turnSpeed = 0.035f;
        public AudioSource launch_sound;
        public AudioSource thrust_sound;
        public GameObject smoke_obj;
        public ParticleSystem smoke;
        public GameObject smoke_position;
        public GameObject destroy_effect;
        private void Start()
        {
            projectilerb = this.GetComponent<Rigidbody>();
        }
        public void call_destroy_effects()
        {
            Instantiate(destroy_effect, transform.position, transform.rotation);
        }
        public void setmissile()
        {
            timealive = 0;
            transform.rotation = shooter.transform.rotation;
            transform.Rotate(0, 90, 0);
            transform.position = shooter.transform.position;
        }

        [System.Obsolete]
        public void DestroyMe()
        {
            isactive = false;
            fully_active = false;
            timealive = 0;
            smoke.transform.SetParent(null);
            smoke.Pause();
            smoke.transform.position = sleepposition;
            smoke.Play();
            projectilerb.velocity = Vector3.zero;
            thrust_sound.Pause();
            call_destroy_effects();
            transform.position = sleepposition;
            Destroy(smoke.gameObject, 3);
            Destroy(this.gameObject);
        }
        public void usemissile()
        {
            launch_sound.Play();
            isactive = true;
            // setmissile();

        }

        [System.Obsolete]
        private void OnCollisionEnter(Collision other)
        {
            if (isactive)
            {
                if (other.gameObject.CompareTag("Enemy"))
                {
                    DestroyMe();
                }
            }
        }

        [System.Obsolete]
        void FixedUpdate()
        {
            if (isactive)
            {
                // if (!target.activeInHierarchy)
                // {
                //     DestroyMe();
                // }
                if (timealive == timebeforeactivition)
                {
                    fully_active = true;
                    thrust_sound.Play();
                }
                timealive++;
                // if (timealive < timebeforebursting)
                // {
                //     projectilerb.velocity = transform.up * -1 * downspeed;
                // }
                if (timealive == timebeforebursting)
                {
                    smoke = (Instantiate(smoke_obj, smoke_position.transform.position, smoke_position.transform.rotation)).GetComponent<ParticleSystem>();
                    smoke.Play();
                    smoke.transform.SetParent(this.transform);
                }
                // if (timealive == timebeforedestruction)
                // {
                //     DestroyMe();
                // }
                if (timealive >= timebeforebursting && timealive < timebeforedestruction)
                {
                    Debug.Log("Missile is active");
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetpointer.transform.rotation, turnSpeed);
                    projectilerb.velocity = transform.forward * speed;
                }
            }
        }
    }
}