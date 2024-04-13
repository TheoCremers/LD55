namespace FieldUnits
{
    public class SingleTargetMeleeAttack : Attack
    {
        public override void PerformAttack(FieldUnit target)
        {
            // TODO swing animation, possible delay
            target.TakeDamage(damage);
        }
    }
}