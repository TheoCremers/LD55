using System.Collections.Generic;
using UnityEngine;

public class FieldUnitManager : MonoBehaviour
{
    public Transform playerFieldUnitParent;
    public Transform enemyFieldUnitParent;
    public UnitSummonedEventChannel unitSummonedEventChannel;

    public static List<FieldUnit> FieldUnits = new List<FieldUnit>();

    public const float FlightHeight = 1.35f;
    public const float MaxRandomHorizontalOffset = 0.1f;
    public const float MaxRandomVerticalOffset = 0.15f;

    private void Awake()
    {
        unitSummonedEventChannel.OnEventRaised += OnUnitSummonedEvent;
    }

    private void OnDestroy()
    {
        unitSummonedEventChannel.OnEventRaised -= OnUnitSummonedEvent;
    }

    public void OnUnitSummonedEvent(GameObject unitPrefab, float lifeForce)
    {
        SpawnFieldUnitPrefab(unitPrefab, true);
    }

    public void SpawnFieldUnitPrefab(GameObject unitPrefab, bool isPlayerFaction)
    {
        var newUnit = Instantiate(unitPrefab);
        var fieldUnit = newUnit.GetComponent<FieldUnit>();
        fieldUnit.ApplyFaction(isPlayerFaction);
        SetFieldUnitPosition(fieldUnit);
    }

    private void SetFieldUnitPosition(FieldUnit fieldUnit)
    {
        fieldUnit.transform.SetParent(fieldUnit.isPlayerFaction ? playerFieldUnitParent : enemyFieldUnitParent, false);
        
        var randomOffset = new Vector3(Random.Range(-MaxRandomHorizontalOffset, MaxRandomHorizontalOffset), Random.Range(-MaxRandomVerticalOffset, MaxRandomVerticalOffset));
        fieldUnit.transform.position += randomOffset;
        if (fieldUnit.flying)
        {
            fieldUnit.transform.position += Vector3.up * FlightHeight;
        }
    }
}
