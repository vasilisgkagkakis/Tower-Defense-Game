using UnityEngine;

namespace HomingMissile
{
    public class homing_missile_pointer : MonoBehaviour
    {
        public GameObject target;

        private Vector3 previousTargetPosition;
        private Vector3 targetVelocity;
        private bool hasVelocityData = false;

        private void FixedUpdate()
        {
            if (target != null)
            {
                Vector3 currentTargetPosition;
                
                // Get the target position (prefer AimPoint if available)
                Transform aimTransform = target.transform.Find("AimPoint");
                if (aimTransform != null)
                    currentTargetPosition = aimTransform.position;
                else
                    currentTargetPosition = target.transform.position;

                if (hasVelocityData)
                {
                    targetVelocity = (currentTargetPosition - previousTargetPosition) / Time.fixedDeltaTime;
                }
                else
                {
                    targetVelocity = Vector3.zero;
                    hasVelocityData = true;
                }
                
                previousTargetPosition = currentTargetPosition;

                // Calculate predictive aim point
                Vector3 predictedAimPoint = CalculatePredictiveAimPoint(currentTargetPosition);
                
                transform.LookAt(predictedAimPoint);
            }
        }

        private Vector3 CalculatePredictiveAimPoint(Vector3 currentTargetPosition)
        {
            var missile = GetComponentInParent<homing_missile>();
            if (missile == null) return currentTargetPosition;

            float missileSpeed = missile.speed;
            if (missileSpeed <= 0 || targetVelocity.magnitude < 0.1f) 
                return currentTargetPosition;

            // Use interception calculation for better accuracy
            Vector3 predictedPosition = CalculateInterceptionPoint(
                transform.position, 
                missileSpeed, 
                currentTargetPosition, 
                targetVelocity
            );

            float predictionStrength = missile.predictionStrength;
            Vector3 finalAimPoint = Vector3.Lerp(currentTargetPosition, predictedPosition, predictionStrength);
            
            return finalAimPoint;
        }

        private Vector3 CalculateInterceptionPoint(Vector3 shooterPos, float projectileSpeed, Vector3 targetPos, Vector3 targetVel)
        {
            // Solve quadratic equation for interception
            Vector3 toTarget = targetPos - shooterPos;
            float a = Vector3.Dot(targetVel, targetVel) - (projectileSpeed * projectileSpeed);
            float b = 2 * Vector3.Dot(targetVel, toTarget);
            float c = Vector3.Dot(toTarget, toTarget);

            float discriminant = b * b - 4 * a * c;
            
            if (discriminant < 0 || Mathf.Abs(a) < 0.001f)
            {
                // No solution or target is too fast - fall back to simple prediction
                float timeEstimate = toTarget.magnitude / projectileSpeed;
                return targetPos + targetVel * timeEstimate;
            }

            float t1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
            float t2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
            
            // Use the smallest positive time
            float t = (t1 > 0 && (t2 <= 0 || t1 < t2)) ? t1 : t2;
            
            if (t <= 0)
            {
                // Fallback for edge cases
                float timeEstimate = toTarget.magnitude / projectileSpeed;
                return targetPos + targetVel * timeEstimate;
            }

            return targetPos + targetVel * t;
        }

    }
}