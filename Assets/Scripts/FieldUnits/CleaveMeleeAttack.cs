using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FieldUnits
{
    public class CleaveMeleeAttack : Attack
    {
        public float cleaveRange = 0f;
        public override void PerformAttack(FieldUnit origin, FieldUnit target)
        {
            // TODO swing animation, possible delay
            foreach (var fieldUnit in FindNearbyTargets(target))
            {
                fieldUnit.TakeDamage(damage * origin.damageModifier);
            }
        }
        
        private List<FieldUnit> FindNearbyTargets(FieldUnit splashOrigin)
        {
            var allies = FieldUnitManager.fieldUnits.Where(x => x.isPlayerFaction == splashOrigin.isPlayerFaction).ToList();
            return allies.Where(ally => IsTargetInCleaveRange(splashOrigin.collider, ally.collider)).ToList();
        }
        
        private bool IsTargetInCleaveRange(Collider2D originCollider, Collider2D targetCollider)
        {
            if (originCollider == targetCollider) return true;
            var distance = Physics2D.Distance(originCollider, targetCollider).distance;
            return distance <= cleaveRange;
        }
    }
}