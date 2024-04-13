using UnityEngine;

namespace FieldUnits
{
    public abstract class Attack : MonoBehaviour
    {
        public int damage;
        public float cooldown;
        public float range;
        public abstract void PerformAttack(FieldUnit target);
    }
}