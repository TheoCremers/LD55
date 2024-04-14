using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/UnitSummonedEventChannel")]
public class UnitSummonedEventChannel : ScriptableObject
{
    public UnityAction<GameObject, float> OnEventRaised;

    public void RaiseEvent(GameObject unitPrefab, float bloodCost)
    {
        Debug.Log("UnitSummonedEvent Raised!");
        OnEventRaised.Invoke(unitPrefab, bloodCost);
    }
}
