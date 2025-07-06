using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HomingMissile
{
    public class homing_missile_pointer : MonoBehaviour
    {
        public GameObject target;
        private Vector3 aimPoint;

        void Awake()
        {
            if (target != null)
            {
                Transform aimTransform = target.transform.Find("AimPoint");
                if (aimTransform != null)
                    aimPoint = aimTransform.position;
                else
                    aimPoint = target.transform.position;
            }
        }

        private void FixedUpdate()
        {
            if (target != null)
            {
                Transform aimTransform = target.transform.Find("AimPoint");
                if (aimTransform != null)
                    aimPoint = aimTransform.position;
                else
                    aimPoint = target.transform.position;

                transform.LookAt(aimPoint);
            }
        }
    }
}