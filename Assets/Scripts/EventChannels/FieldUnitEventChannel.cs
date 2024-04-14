using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Field Unit EventChannel")]
public class FieldUnitEventChannel : ScriptableObject
{
    public UnityAction<FieldUnit> OnEventRaised;

    public void RaiseEvent(FieldUnit fieldUnit)
    {
        OnEventRaised.Invoke(fieldUnit);
    }
}
