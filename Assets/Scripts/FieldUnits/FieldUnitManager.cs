using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FieldUnitManager : MonoBehaviour
{
    public Transform playerFieldUnitParent;
    public Transform enemyFieldUnitParent;
    public UnitSummonedEventChannel unitSummonedEventChannel;
    public FieldUnitEventChannel fieldUnitSlainEventChannel;
    public VoidEventChannel hostageGainedEventChannel;

    public static List<FieldUnit> FieldUnits = new List<FieldUnit>();

    public const float FlightHeight = 1.35f;
    public const float MaxRandomHorizontalOffset = 0.1f;
    public const float MaxRandomVerticalOffset = 0.15f;
    public int hostagePerKills = 5;
    private static int _killCount;

    private void Awake()
    {
        _killCount = 0;
        unitSummonedEventChannel.OnEventRaised += OnUnitSummonedEvent;
        fieldUnitSlainEventChannel.OnEventRaised += OnFieldUnitSlainEvent;
    }

    private void OnDestroy()
    {
        unitSummonedEventChannel.OnEventRaised -= OnUnitSummonedEvent;
    }

    public void OnUnitSummonedEvent(GameObject unitPrefab, float lifeForce)
    {
        SpawnFieldUnitPrefab(unitPrefab, true);
    }

    private void OnFieldUnitSlainEvent(FieldUnit fieldUnit)
    {
        if (fieldUnit.IsCastle)
        {
            // TODO: Victory / Loss screen
            SceneManager.LoadScene( SceneManager.GetActiveScene().name );
        }
        else
        {
            // TODO: Death effect / animation
            Destroy(fieldUnit.gameObject);
            if (!fieldUnit.isPlayerFaction)
            {
                _killCount++;
                if (_killCount % hostagePerKills == 0)
                {
                    hostageGainedEventChannel.RaiseEvent();
                }
            }
        }
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
