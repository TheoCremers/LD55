using System;
using UnityEngine;

namespace FieldUnits
{
    public class Aura : MonoBehaviour
    {
        public AuraEffect appliedEffect;
        private bool _isBuff => appliedEffect is AuraEffect.Healing or AuraEffect.Frenzy or AuraEffect.Empower or AuraEffect.Protect;
        public FieldUnit parent;

        private void Awake()
        {
            // Aura also effects the unit with the aura (?)
            parent.ApplyAuraEffect(appliedEffect);
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