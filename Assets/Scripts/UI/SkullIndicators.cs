using System;
using System.Linq;
using UnityEngine;

public class SkullIndicators : MonoBehaviour
{
    public GameStartEventChannel GameStartEvent;
    public GameObject SkullPrefab;
    
    private void Awake()
    {
        GameStartEvent.OnEventRaised += DisplaySkulls;
    }

    private void DisplaySkulls(GameModeType gameType)
    {
        var gameSettings = GameAssets.Instance.GameModeSettingsList.First(x => x.GameModeType == gameType);

        for (var i = 0; i < gameSettings.IntervalWaves.Count; i++)
        {
            var skull = Instantiate(SkullPrefab, transform, false);
            float ratio = (float)(i + 1) / (gameSettings.IntervalWaves.Count + 1);
            skull.GetComponent<RectTransform>().anchorMax = new Vector2(0f, ratio);
            skull.GetComponent<RectTransform>().anchorMin = new Vector2(0f, ratio);
        }
    }

    private void OnDestroy()
    {
        GameStartEvent.OnEventRaised -= DisplaySkulls;
    }

    private void RemoveSkull()
    {
        
    }
}
