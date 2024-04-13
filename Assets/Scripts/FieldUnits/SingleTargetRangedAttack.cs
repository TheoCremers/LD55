using System;
using UnityEngine;

namespace FieldUnits
{
    public class SingleTargetRangedAttack : Attack
    {
        public Projectile projectilePrefab;
        public float projectileSpeed = 1f;
        private float _launchForce = 10f;
        private FieldUnit _parent;
        public bool arcingShot = true;

        private void Awake()
        {
            _parent = gameObject.GetComponent<FieldUnit>();
        }

        public override void PerformAttack(FieldUnit target)
        {
            // Shoot projectile 
            FireProjectile(transform.position, target.transform.position);
        }
        
        private void FireProjectile(Vector2 startPosition, Vector2 targetPosition)
        {
            var projectile = Instantiate(projectilePrefab, startPosition, Quaternion.identity);
            projectile.damage = damage;
            projectile.isPlayerFaction = _parent.isPlayerFaction;
            var rb = projectile.GetComponent<Rigidbody2D>();

            if (arcingShot)
            {
                rb.gravityScale = Mathf.Sqrt(projectileSpeed);
                var direction = (targetPosition - startPosition).normalized;
                var distance = Vector2.Distance(startPosition, targetPosition);
                var launchVelocity = CalculateLaunchVelocity(direction, distance);

                rb.velocity = launchVelocity;
            }
            else
            {
                var direction = (targetPosition - startPosition).normalized;
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
    }
}