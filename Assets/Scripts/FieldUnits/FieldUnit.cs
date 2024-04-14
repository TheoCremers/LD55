using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FieldUnits;
using Unity.VisualScripting;
using UnityEngine;

public class FieldUnit : MonoBehaviour
{
    [SerializeField] private ParticleSystem damageParticles = null;
    [SerializeField] private ParticleSystem healingParticles = null;

    public SpriteRenderer spriteRenderer;

    public new Rigidbody2D rigidbody2D;

    public new Collider2D collider;

    private Attack _attack;

    public List<AuraEffect> appliedAuraEffects;
    
    // TODO animator

    public float moveSpeed = 0.05f;

    public bool flying = false;

    public FieldUnit target = null;

    public float maxHealth = 100;
    public float currentHealth;

    private float _attackCooldownRemaining = 0f;

    private float _retargetCooldownRemaining = 1f;
    private Color _originalColor;

    [HideInInspector] public float damageModifier = 1.0f;
    [HideInInspector] public float defenseModifer = 1.0f;
    [HideInInspector] public float cooldownModifier = 1.0f;
    [HideInInspector] public float movementModifier = 1.0f;

    public bool isPlayerFaction;

    private void Start()
    {
        appliedAuraEffects = new List<AuraEffect>();
        FieldUnitManager.FieldUnits.Add(this);
        _attack = this.GetComponent<Attack>();
        currentHealth = maxHealth;
        _originalColor = spriteRenderer.color;
        InvokeRepeating(nameof(ApplyHealingTick), 1f, 1f);
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

    private void ApplyHealingTick()
    {
        if (appliedAuraEffects.Contains(AuraEffect.Healing))
        {
            TakeHealing(maxHealth / 20.0f); // 5% healing per second. Maybe tweak or customize in aura
        }
    }

    public void ApplyAuraEffect(AuraEffect effect)
    {
        // Check if we already have the effect
        if (appliedAuraEffects.Contains(effect))
        {
            return;
        }
        appliedAuraEffects.Add(effect);
    }

    public void RemoveAuraEffect(AuraEffect effect)
    {
        // Check if we're no longer in any other duplicate aura
        var currentColliders = new Collider2D[]{};
        var filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.NameToLayer("Aura"));
        collider.GetContacts(filter, currentColliders);
        if (currentColliders.Any(currentCollider => currentCollider.GetComponent<Aura>().appliedEffect == effect))
        {
            return;
        }

        appliedAuraEffects.Remove(effect);
    }

    private bool IsTargetInAttackRange()
    {
        var distance = Physics2D.Distance(collider, target.collider).distance;
        return distance <= _attack.range;
    }

    private void MoveTowardsEnemy()
    {
        rigidbody2D.mass = 1f;
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 
            moveSpeed * movementModifier * Time.deltaTime);
    }
    
    private void Attack()
    {
        _attack.PerformAttack(this, target);
        _attackCooldownRemaining = _attack.cooldown * cooldownModifier;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage * defenseModifer;
        SpawnDamageParticles();
        StopCoroutine(DamageFlash());
        StartCoroutine(DamageFlash());
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeHealing(float healing)
    {
        currentHealth = Math.Max(currentHealth + healing, maxHealth);
        SpawnHealingParticles();
        // Healing flash? probably not needed
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

    private void SpawnDamageParticles()
    {
        Instantiate(damageParticles, transform.position, Quaternion.identity);
    }
    private void SpawnHealingParticles()
    {
        Instantiate(healingParticles, transform.position, Quaternion.identity);
    }
}
