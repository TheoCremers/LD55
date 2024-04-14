using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FieldUnits;
using UnityEngine;

public class FieldUnit : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem = null;
    private ParticleSystem _particleSystemInstance;
    
    public SpriteRenderer spriteRenderer;

    public new Rigidbody2D rigidbody2D;

    public new Collider2D collider;

    private Attack _attack;
    
    // TODO animator

    public float moveSpeed = 0.05f;

    public bool flying = false;

    public FieldUnit target = null;

    public int maxHealth = 100;
    public int currentHealth;

    private float _attackCooldownRemaining = 0f;

    private float _retargetCooldownRemaining = 1f;
    private Color _originalColor;

    public bool isPlayerFaction;

    private void Start()
    {
        FieldUnitManager.FieldUnits.Add(this);
        _attack = this.GetComponent<Attack>();
        currentHealth = maxHealth;
        _originalColor = spriteRenderer.color;
    }

    // Update is called once per frame
    private void Update()
    {
        _attackCooldownRemaining -= Time.deltaTime;
        _retargetCooldownRemaining -= Time.deltaTime;
        if (target && _retargetCooldownRemaining > 0f)
        {
            if (IsTargetInAttackRange())
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

    private bool IsTargetInAttackRange()
    {
        var distance = Physics2D.Distance(collider, target.collider).distance;
        return distance <= _attack.range;
    }

    private void MoveTowardsEnemy()
    {
        rigidbody2D.mass = 1f;
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
    }
    
    private void Attack()
    {
        Debug.Log(this.name + " attacks " + target.name);
        _attack.PerformAttack(target);
        _attackCooldownRemaining = _attack.cooldown;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        SpawnParticles();
        StopCoroutine(DamageFlash());
        StartCoroutine(DamageFlash());
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        FieldUnitManager.FieldUnits.Remove(this);
    }
    
    private IEnumerator DamageFlash ()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.color = _originalColor;
    }

    private void FindClosestEnemy()
    {
        var enemies = FieldUnitManager.FieldUnits.Where(x => x.isPlayerFaction != isPlayerFaction).ToList();
        if (_attack.canTargetFlying == false)
        {
            enemies = enemies.Where(x => !x.flying).ToList();
        }
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

    private void SpawnParticles()
    {
        _particleSystemInstance = Instantiate(_particleSystem, transform.position, Quaternion.identity);
    }
}
