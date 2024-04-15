using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/ResourceUpdateEventChannel")]
public class ResourceUpdateEventChannel : ScriptableObject
{
    public UnityAction<float, float, int> OnEventRaised;

    public void RaiseEvent(float knowledge, float totalLifeForce, int numberOfAcolytes)
    {
        OnEventRaised.Invoke(knowledge, totalLifeForce, numberOfAcolytes);
    }
}
