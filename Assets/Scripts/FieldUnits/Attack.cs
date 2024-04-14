using UnityEngine;

namespace FieldUnits
{
    public abstract class Attack : MonoBehaviour
    {
        public float damage;
        public float cooldown;
        public float range;
        public bool canTargetFlying;
        public abstract void PerformAttack(FieldUnit origin, FieldUnit target);
    }
}