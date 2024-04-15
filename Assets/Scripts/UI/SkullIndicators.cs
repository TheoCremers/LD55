using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkullIndicators : MonoBehaviour
{
    public GameStartEventChannel GameStartEvent;
    public UnitWaveEventChannel UnitWaveEventChannel;
    public GameObject SkullPrefab;
    private Stack<GameObject> _indicators;
    
    private void Awake()
    {
        _indicators = new Stack<GameObject>();
        GameStartEvent.OnEventRaised += DisplaySkulls;
        UnitWaveEventChannel.OnEventRaised += RemoveSkull;
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
            _indicators.Push(skull);
        }
    }

    private void OnDestroy()
    {
        GameStartEvent.OnEventRaised -= DisplaySkulls;
    }

    private void RemoveSkull(UnitWave wave)
    {
        var skull = _indicators.Pop();
        Destroy(skull.gameObject);
    }
}
