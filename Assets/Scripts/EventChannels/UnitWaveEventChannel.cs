using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/UnitWaveEventChannel")]
public class UnitWaveEventChannel : ScriptableObject
{
    public UnityAction<UnitWave> OnEventRaised;

    public void RaiseEvent(UnitWave unitWave)
    {
        Debug.Log("spawning interval wave");
        OnEventRaised.Invoke(unitWave);
    }
}
