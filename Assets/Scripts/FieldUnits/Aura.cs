using System;
using UnityEngine;

namespace FieldUnits
{
    public class Aura : MonoBehaviour
    {
        public AuraEffect appliedEffect;
        private bool _isBuff => appliedEffect is AuraEffect.Healing or AuraEffect.Frenzy or AuraEffect.Empower or AuraEffect.Protect;
        public FieldUnit parent;

        private void Start()
        {
            if (_isBuff)
                parent.ApplyAuraEffect(appliedEffect);
            
            this.GetComponent<Collider2D>().enabled = false;
            this.GetComponent<Collider2D>().enabled = true;
        }

        private void OnTriggerEnter2D (Collider2D collision)
        {
            if (!collision.TryGetComponent(out FieldUnit fieldUnit)) return;
            if (_isBuff != (fieldUnit.isPlayerFaction == parent.isPlayerFaction)) return;
            fieldUnit.ApplyAuraEffect(appliedEffect);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.TryGetComponent(out FieldUnit fieldUnit)) return;
            if (_isBuff != (fieldUnit.isPlayerFaction == parent.isPlayerFaction)) return;
            fieldUnit.RemoveAuraEffect(appliedEffect);
        }
    }
}