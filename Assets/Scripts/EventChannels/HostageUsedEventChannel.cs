using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/HostageUsedEventChannel")]
public class HostageUsedEventChannel : ScriptableObject
{
    public UnityAction<HostageUseType, float, float, float> OnEventRaised;

    public void RaiseEvent(HostageUseType type, float knowledge, float lifeForce, float ransomValue)
    {
        Debug.Log("HostageUsedEvent Raised!");
        OnEventRaised.Invoke(type, knowledge, lifeForce, ransomValue);
    }
}
