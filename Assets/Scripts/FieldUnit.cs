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

    public float attackRange = 0.02f;

    public float attackCooldown = 1.0f;

    private float _lastAttackTime = 0f;

    public bool isPlayerFaction;    
    private void Start()
    {
        FieldUnitManager.FieldUnits.Add(this);
    }

    // Update is called once per frame
    private void Update()
    {
        if (target)
        {
            if (Vector3.Distance(transform.position, target.transform.position) <= attackRange)
            {
                if (Time.time > _lastAttackTime + attackCooldown)
                {
                    EngageCombat();
                    _lastAttackTime = Time.time;
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
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
    }
    
    void EngageCombat()
    {
        Debug.Log("Engaging in combat with enemy!");
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
        }
        else
        {
            target = null;
        }
    }
}
