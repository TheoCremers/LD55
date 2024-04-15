using System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FieldUnits
{
    public class SingleTargetRangedAttack : Attack
    {
        public Projectile projectilePrefab;
        public float projectileSpeed = 1f;
        public bool arcingShot = true;
        private float _projectileSpawnOffset = 0.2f;
        
        public float maxLaunchAngle = 45.0f; // Maximum launch angle
        public float minLaunchAngle = 35.0f; // Minimum launch angle

        public float meleeDistance = 0.5f;

        public override void PerformAttack(FieldUnit origin, FieldUnit target)
        {
            // Shoot projectile 
            FireProjectile(origin, target);
        }
        
        private void FireProjectile(FieldUnit origin, FieldUnit target)
        {
            var startPosition = origin.transform.position + Vector3.up * _projectileSpawnOffset;
            var projectile = Instantiate(projectilePrefab, startPosition, quaternion.identity);
            projectile.damage = damage * origin.damageModifier;
            projectile.isPlayerFaction = origin.isPlayerFaction;
            var rb = projectile.GetComponent<Rigidbody2D>();

            if (arcingShot && (startPosition - target.transform.position).magnitude > meleeDistance)
            {
                if (target.flying)
                {
                    rb.velocity = CalculateVelocityVectorFromSpeed(startPosition, target.transform.position, projectileSpeed);
                }
                else
                {
                    rb.velocity = CalculateLaunchVelocity(startPosition, target.transform.position, Random.Range(minLaunchAngle, maxLaunchAngle));
                }
                float launchAngle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
                projectile.transform.rotation = Quaternion.Euler(0, 0, launchAngle);
            }
            else
            {
                var direction = (target.transform.position - startPosition).normalized;
                var launchVelocity = direction * projectileSpeed;
                rb.velocity = launchVelocity;
                rb.gravityScale = 0f;
            }
        }

        private Vector2 CalculateLaunchVelocity(Vector3 startPosition, Vector3 targetPosition, float angle)
        {
            var dir = targetPosition - startPosition;  // get target direction
            var h = dir.y;  // get height difference
            dir.y = 0;  // retain only the horizontal direction
            var dist = dir.magnitude ;  // get horizontal distance
            var a = angle * Mathf.Deg2Rad;  // convert angle to radians
            dir.y = dist * Mathf.Tan(a);  // set dir to the elevation angle
            dist += h / Mathf.Tan(a);  // correct for small height differences
            // calculate the velocity
            var vel2 = dist * Physics.gravity.magnitude / Mathf.Sin(2 * a);
            if (vel2 > 0) return Mathf.Sqrt(vel2) * dir.normalized;
            else return Vector2.zero;
        }

        private Vector2 CalculateVelocityVectorFromSpeed(Vector3 startPosition, Vector3 targetPosition, float projectileSpeed)
        {
            var x = targetPosition.x - startPosition.x;
            var y = targetPosition.y - startPosition.y;
            var v2 = projectileSpeed * projectileSpeed;
            var g = Physics.gravity.magnitude;
            var d = v2 * v2 - g * (g * x * x + 2f * y * v2);
            var dir = Mathf.Sign(x);
            x = dir * x;

            if (d <= 0)
            {
                if (projectileSpeed > 0)
                {
                    return CalculateVelocityVectorFromSpeed(startPosition, targetPosition, projectileSpeed * 1.5f);
                }
                else
                {
                    throw new Exception("Projectile speed is zero");
                }
            }
            else
            {
                var sign = Mathf.Sign(Mathf.Sqrt(d) - v2);
                var a = Mathf.Atan((v2 + sign * Mathf.Sqrt(d)) / (g * x));
                var deg = a * Mathf.Rad2Deg;
                return new Vector2(dir * Mathf.Cos(a), Mathf.Sin(a)) * projectileSpeed;
            }
        }
    }
}