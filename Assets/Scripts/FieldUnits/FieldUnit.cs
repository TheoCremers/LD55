using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using FieldUnits;
using UnityEngine;

public class FieldUnit : MonoBehaviour
{
    public FieldUnitEventChannel FieldUnitSlainEventChannel;
    
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
    [HideInInspector] public float defenseModifier = 1.0f;
    [HideInInspector] public float cooldownModifier = 1.0f;
    [HideInInspector] public float movementModifier = 1.0f;

    public bool isPlayerFaction;

    public bool IsCastle;

    private void Awake()
    {
        appliedAuraEffects = new List<AuraEffect>();
        _attack = this.GetComponent<Attack>();
        currentHealth = maxHealth;
        _originalColor = spriteRenderer.color;
        InvokeRepeating(nameof(ApplyHealingTick), 1f, 1f);
    }

    private void FixedUpdate()
    {
        ApplyAuraEffects();
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

    private void ApplyAuraEffects()
    {
        if (appliedAuraEffects.Contains(AuraEffect.Empower))
        {
            damageModifier = appliedAuraEffects.Contains(AuraEffect.Weaken) ? 1.0f : 1.3f;
        }
        else
        {
            damageModifier = appliedAuraEffects.Contains(AuraEffect.Weaken) ? 0.7f : 1.0f;
        }
        if (appliedAuraEffects.Contains(AuraEffect.Protect))
        {
            defenseModifier = appliedAuraEffects.Contains(AuraEffect.Expose) ? 1.0f : 1.3f;
        }
        else
        {
            defenseModifier = appliedAuraEffects.Contains(AuraEffect.Expose) ? 0.7f : 1.0f;
        }
        if (appliedAuraEffects.Contains(AuraEffect.Frenzy))
        {
            movementModifier = appliedAuraEffects.Contains(AuraEffect.Slow) ? 1.0f : 1.3f;
            cooldownModifier = appliedAuraEffects.Contains(AuraEffect.Slow) ? 1.0f : 1.3f;
        }
        else
        {
            movementModifier = appliedAuraEffects.Contains(AuraEffect.Slow) ? 0.7f : 1.0f;
            cooldownModifier = appliedAuraEffects.Contains(AuraEffect.Slow) ? 0.7f : 1.0f;
        }
    }

    private void ApplyHealingTick()
    {
        if (appliedAuraEffects.Contains(AuraEffect.Degen))
        {
            TakeDamage(maxHealth * 0.05f); // 5% max hp dmg per second. Maybe tweak or customize in aura
        }
        if (appliedAuraEffects.Contains(AuraEffect.Healing))
        {
            TakeHealing(maxHealth * 0.05f); // 5% max hp healing per second. Maybe tweak or customize in aura
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
        spriteRenderer.flipX = (target.transform.position.x - transform.position.x) < 0;
    }
    
    private void Attack()
    {
        _attack.PerformAttack(this, target);
        _attackCooldownRemaining = _attack.cooldown * cooldownModifier;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage * defenseModifier;
        SpawnDamageParticles();
        DamagePopup.Create(this.transform, (int)damage, false);
        StopCoroutine(DamageFlash());
        StartCoroutine(DamageFlash());
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        FieldUnitSlainEventChannel.RaiseEvent(this);
        _ = DeathAnimation();
    }

    public void ApplyFaction(bool playerFaction)
    {
        this.isPlayerFaction = playerFaction;
        spriteRenderer.flipX = !playerFaction;
        spriteRenderer.color = playerFaction ? new Color32(49, 86, 204, 255) : new Color32(164, 61, 61, 255);
        _originalColor = spriteRenderer.color;
    }

    private void TakeHealing(float healing)
    {
        currentHealth = Mathf.Min(currentHealth + healing, maxHealth);
        DamagePopup.Create(this.transform, (int)healing, true);
        SpawnHealingParticles();
        // Healing flash? probably not needed
    }


    
    private IEnumerator DamageFlash ()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.color = _originalColor;
    }

    public async Task DeathAnimation()
    {
        spriteRenderer.DOKill();
        await spriteRenderer.DOColor(new Color(0.25f, 0, 0), 0.2f).AsyncWaitForCompletion();
        await spriteRenderer.DOFade(0f, 0.2f).AsyncWaitForCompletion();
        Destroy(gameObject);
    }

    private void FindClosestEnemy()
    {
        var enemies = FieldUnitManager.fieldUnits.Where(x => x.isPlayerFaction != isPlayerFaction).ToList();
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
        var emitter = Instantiate(damageParticles, transform, false);
    }
    private void SpawnHealingParticles()
    {
        var emitter = Instantiate(healingParticles, transform, false);
    }
}
