using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Game Start Event Channel")]
public class GameStartEventChannel : ScriptableObject
{
    public UnityAction<GameModeType> OnEventRaised;

    public void RaiseEvent(GameModeType type)
    {
        Debug.Log("GameStartEvent Raised!");
        OnEventRaised.Invoke(type);
    }
}
