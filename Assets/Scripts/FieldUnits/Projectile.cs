using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D _rigidBody2D;

    public float damage;

    public bool isPlayerFaction;

    void Start()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        RotateArrowAccordingToVelocity();
    }

    private void RotateArrowAccordingToVelocity()
    {
        if (_rigidBody2D.velocity != Vector2.zero)
        {
            var angle = Mathf.Atan2(_rigidBody2D.velocity.y, _rigidBody2D.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
    
    private void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.CompareTag("FieldUnitHitBox"))
        {
            var fieldUnit = collision.GetComponentInParent<FieldUnit>();
            if (fieldUnit.isPlayerFaction != isPlayerFaction)
            {
                fieldUnit.TakeDamage(damage);
                Destroy(gameObject);
            }
        } 
        else if (collision.CompareTag("DestroyProjectiles"))
        {
            Destroy(gameObject);
        }
    }
}
