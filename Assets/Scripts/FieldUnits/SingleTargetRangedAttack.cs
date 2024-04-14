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
        private float _launchForce = 10f;
        public bool arcingShot = true;
        private float _projectileSpawnOffset = 0.2f;
        
        public float maxLaunchAngle = 45.0f; // Maximum launch angle
        public float minLaunchAngle = 35.0f; // Minimum launch angle

        public override void PerformAttack(FieldUnit origin, FieldUnit target)
        {
            // Shoot projectile 
            FireProjectile(origin, target.collider.transform);
        }
        
        private void FireProjectile(FieldUnit origin, Transform targetPosition)
        {
            var startPosition = new Vector2(origin.transform.position.x,
                origin.transform.position.y + _projectileSpawnOffset);
            var projectile = Instantiate(projectilePrefab, startPosition, quaternion.identity);
            projectile.damage = damage * origin.damageModifier;
            projectile.isPlayerFaction = origin.isPlayerFaction;
            var rb = projectile.GetComponent<Rigidbody2D>();

            if (arcingShot)
            {
                rb.velocity = CalculateLaunchVelocity(targetPosition, Random.Range(minLaunchAngle, maxLaunchAngle));
                float launchAngle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
                projectile.transform.rotation = Quaternion.Euler(0, 0, launchAngle);
            }
            else
            {
                var direction = ((Vector2)targetPosition.position - startPosition).normalized;
                var launchVelocity = direction * _launchForce * projectileSpeed * 0.1f;
                rb.velocity = launchVelocity;
                rb.gravityScale = 0f;
            }
        }

        private Vector2 CalculateLaunchVelocity(Vector2 direction, float distance)
        {
            var radians = 45f * Mathf.Deg2Rad;
            var xVelocity = Mathf.Cos(radians) * distance * direction.x;
            var yVelocity = Mathf.Sin(radians) * distance;

            return new Vector2(xVelocity, yVelocity) * Mathf.Sqrt(_launchForce * projectileSpeed / (distance * Mathf.Sqrt(projectileSpeed)));
        }

        private Vector2 CalculateLaunchVelocity(Transform target, float angle)
        {
            var dir = target.position - transform.position;  // get target direction
            var h = dir.y;  // get height difference
            dir.y = 0;  // retain only the horizontal direction
            var dist = dir.magnitude ;  // get horizontal distance
            var a = angle * Mathf.Deg2Rad;  // convert angle to radians
            dir.y = dist * Mathf.Tan(a);  // set dir to the elevation angle
            dist += h / Mathf.Tan(a);  // correct for small height differences
            // calculate the velocity magnitude
            var vel = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
            return vel * dir.normalized;
        }
        
        // private Vector2 calcBallisticVelocityVector(Vector2 source, Transform target, float angle)
        // {
        //     Vector3 direction = (Vector2)target.position -  source;			// get target direction
        //     float h = direction.y;											// get height difference
        //     direction.y = 0;												// remove height
        //     float distance = direction.magnitude;							// get horizontal distance
        //     float a = angle * Mathf.Deg2Rad;								// Convert angle to radians
        //     direction.y = distance * Mathf.Tan(a);							// Set direction to elevation angle
        //     distance += h/Mathf.Tan(a);										// Correction for small height differences
		      //
        //     // calculate velocity
        //     float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2*a));
        //     return velocity * direction.normalized;
        // }
    }
}