using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FieldUnit : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public new Rigidbody2D rigidbody2D;
    
    // TODO animator

    public float moveSpeed = 0.05f;

    public FieldUnit target = null;

    public int maxHealth = 100;
    public int _currentHealth;

    public float attackRange = 0.02f;

    public int attackDamage = 10;

    public float attackCooldown = 1.0f;

    private float _attackCooldownRemaining = 0f;

    private float _retargetCooldownRemaining = 1f;

    public bool isPlayerFaction;    
    private void Start()
    {
        FieldUnitManager.FieldUnits.Add(this);
        _currentHealth = maxHealth;
    }

    // Update is called once per frame
    private void Update()
    {
        _attackCooldownRemaining -= Time.deltaTime;
        _retargetCooldownRemaining -= Time.deltaTime;
        if (target && _retargetCooldownRemaining > 0f)
        {
            if (Vector3.Distance(transform.position, target.transform.position) <= attackRange)
            {
                rigidbody2D.mass = 100f;
                if (_attackCooldownRemaining <= 0f)
                {
                    Attack();
                }
            }
            else
            {
                MoveTowardsEnemy();
            }
        }
        else
        {
            FindClosestEnemy();
        }
    }

    private void MoveTowardsEnemy()
    {
        rigidbody2D.mass = 1f;
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
    }
    
    void Attack()
    {
        Debug.Log(this.name + " attacks " + target.name);
        target.TakeDamage(attackDamage);
        _attackCooldownRemaining = attackCooldown;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        FieldUnitManager.FieldUnits.Remove(this);
    }

    private void FindClosestEnemy()
    {
        var enemies = FieldUnitManager.FieldUnits.Where(x => x.isPlayerFaction != isPlayerFaction).ToList();
        if (enemies.Any())
        {
            FieldUnit closest = null;
            var closestDistance = Mathf.Infinity;

            foreach (var enemy in enemies)
            {
                var distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (!(distance < closestDistance)) continue;
                closest = enemy;
                closestDistance = distance;
            }

            if (closest) target = closest;
            _retargetCooldownRemaining = 1f;
        }
        else
        {
            target = null;
        }
    }
}
