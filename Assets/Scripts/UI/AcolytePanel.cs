using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AcolytePanel : MonoBehaviour
{
    public GameObject acolytePrefab;
    public RectTransform acolyteParent;
    public int startingMaxAcolytes = 5;
    public float knowledgeLostFactor = 0.75f;

    private List<AcolyteSlot> _acolyteSlots;
    private int _maxAcolyteSlots;

    public bool HasEmptySlots => _acolyteSlots.Any(s => s.isActive && !s.hasAcolyte);

    public int NumberOfAcolytes => _acolyteSlots.Count(s => s.hasAcolyte);

    private void Awake()
    {
        _acolyteSlots = GetComponentsInChildren<AcolyteSlot>().ToList();
        _maxAcolyteSlots = startingMaxAcolytes;
        for (int i = _maxAcolyteSlots; i < _acolyteSlots.Count; i++)
        {
            _acolyteSlots[i].DisableSlot();
        }
    }

    /// <summary>
    /// Returns amount of knowledge lost
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public float DrainBloodFromAcolytes(float amount)
    {
        var occupiedAccolyteSlots = _acolyteSlots.Where(s => s.isActive && s.hasAcolyte).ToList();
        var knowledgeLost = 0f;

        while (amount > 0)
        {
            if (occupiedAccolyteSlots.Count <= 0) break;

            var sacrificeSlot = occupiedAccolyteSlots.Last();
            amount = sacrificeSlot.acolyte.ChangeCurrentHealth(-amount);
            if (sacrificeSlot.acolyte.currentHealth <= 0)
            {
                knowledgeLost += sacrificeSlot.acolyte.knowledge * knowledgeLostFactor;
                sacrificeSlot.SacrificeAcolyte();
                occupiedAccolyteSlots.Remove(sacrificeSlot);
            }
        }

        return knowledgeLost;
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

    public void AddAcolyte(float knowledge, float lifeForce)
    {
        _acolyteSlots.FirstOrDefault(s => s.isActive && !s.hasAcolyte)?.AddAcolyte(knowledge, lifeForce, acolytePrefab);
    }
}
