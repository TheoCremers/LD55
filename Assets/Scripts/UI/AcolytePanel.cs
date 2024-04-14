using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AcolytePanel : MonoBehaviour
{
    public GameObject acolytePrefab;
    public RectTransform acolyteParent;
    public int startingMaxAcolytes = 5;

    private List<AcolyteSlot> _acolyteSlots;
    private int _maxAcolyteSlots;

    public bool hasEmptySlots => _acolyteSlots.Any(s => s.isActive && !s.hasAcolyte);

    private void Awake()
    {
        _acolyteSlots = GetComponentsInChildren<AcolyteSlot>().ToList();
        _maxAcolyteSlots = startingMaxAcolytes;
        for (int i = _maxAcolyteSlots; i < _acolyteSlots.Count; i++)
        {
            _acolyteSlots[i].DisableSlot();
        }
    }

    public bool DrainBloodFromAcolytes(float amount)
    {
        var occupiedAccolyteSlots = _acolyteSlots.Where(s => s.isActive && s.hasAcolyte).ToList();
        while (amount > 0)
        {
            if (occupiedAccolyteSlots.Count <= 0) return false;

            var sacrificeSlot = occupiedAccolyteSlots.Last();
            amount = sacrificeSlot.acolyte.ChangeCurrentHealth(-amount);
            if (sacrificeSlot.acolyte.currentHealth <= 0)
            {
                sacrificeSlot.SacrificeAcolyte();
                occupiedAccolyteSlots.Remove(sacrificeSlot);
            }
        }

        return true;
    }

    public void AddAcolyteSlot()
    {
        if (_maxAcolyteSlots >= _acolyteSlots.Count) return;
        _acolyteSlots[_maxAcolyteSlots].EnableSlot();
        _maxAcolyteSlots++;
    }

    public float GetTotalLifeForce()
    {
        return _acolyteSlots.Sum(s => s.hasAcolyte? s.acolyte.currentHealth : 0f);
    }

    public void AddAcolyte(float lifeForce)
    {
        _acolyteSlots.FirstOrDefault(s => s.isActive && !s.hasAcolyte)?.AddAcolyte(lifeForce, acolytePrefab);
    }
}
