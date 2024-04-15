using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/ResourceUpdateEventChannel")]
public class ResourceUpdateEventChannel : ScriptableObject
{
    public UnityAction<float, float> OnEventRaised;

    public void RaiseEvent(float knowledge, float totalLifeForce)
    {
        OnEventRaised.Invoke(knowledge, totalLifeForce);
    }
}
