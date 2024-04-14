using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AcolytePanel : MonoBehaviour
{
    public GameObject acolytePrefab;
    public RectTransform acolyteParent;

    private List<Acolyte> _acolytes;

    private void Awake()
    {
        _acolytes = GetComponentsInChildren<Acolyte>().ToList();
    }

    public bool DrainBloodFromAcolytes(float amount)
    {
        while (amount > 0)
        {
            if (_acolytes.Count <= 0) { return false; }
            var sacrifice = _acolytes.Last();
            amount = sacrifice.ChangeCurrentHealth(-amount);
            if (sacrifice.currentHealth <= 0)
            { 
                _acolytes.Remove(sacrifice);
                Destroy(sacrifice.gameObject);
                // sacrificate.DeathAnimation();
            }
        }

        return true;
    }

    public float GetTotalLifeForce()
    {
        return _acolytes.Sum(a => a.currentHealth);
    }

    public void AddAcolyte(float lifeForce)
    {
        var newAcolyteObject = Instantiate(acolytePrefab);
        newAcolyteObject.transform.SetParent(acolyteParent, false);
        var newAcolyte = newAcolyteObject.GetComponent<Acolyte>();
        newAcolyte.maxHealth = lifeForce;
        _acolytes.Add(newAcolyte);
    }
}
