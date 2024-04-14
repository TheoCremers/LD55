namespace FieldUnits
{
    public class SingleTargetMeleeAttack : Attack
    {
        public override void PerformAttack(FieldUnit origin, FieldUnit target)
        {
            // TODO swing animation, possible delay
            target.TakeDamage(damage * origin.damageModifier);
        }
    }
}