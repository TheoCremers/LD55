using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/GameOverEventChannel")]
public class GameOverEventChannel : ScriptableObject
{
    public UnityAction<bool, GameModeType, float> OnEventRaised;

    public void RaiseEvent(bool isVictorious, GameModeType gameModeType, float clearTime = 0f)
    {
        OnEventRaised.Invoke(isVictorious, gameModeType, clearTime);
    }
}
