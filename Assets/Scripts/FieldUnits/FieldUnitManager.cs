using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class FieldUnitManager : MonoBehaviour
{
    public Transform playerFieldUnitParent;
    public Transform enemyFieldUnitParent;
    public UnitSummonedEventChannel unitSummonedEventChannel;
    public FieldUnitEventChannel fieldUnitSlainEventChannel;
    public VoidEventChannel hostageGainedEventChannel;
    public GameStartEventChannel gameStartEvent;
    public GameOverEventChannel gameOverEventChannel;
    public UnitWaveEventChannel unitWaveEventChannel;
    public VoidEventChannel gameConfiguredEvent;

    public FieldUnit playerCastle;
    public FieldUnit enemyCastle;

    public CinemachineVirtualCamera virtualCamera;

    private GameModeType _gameModeType;
    private GameModeSettings _gameSettings;
    public float _secondsElapsed;
    public float nextThresholdPercentage = 0f;
    private int intervalWavesSpawned = 0;

    private static List<FieldUnit> _fieldUnits = new List<FieldUnit>();
    public static List<FieldUnit> fieldUnits => _fieldUnits;

    public const float flightHeight = 1.35f;
    public const float maxRandomHorizontalOffset = 0.1f;
    public const float maxRandomVerticalOffset = 0.15f;
    public int hostagePerKills = 5;
    private static int _killCount;
    private float _playTime;

    private void Awake()
    {
        _killCount = 0;
        _playTime = 0f;
        nextThresholdPercentage = 0;
        
        unitSummonedEventChannel.OnEventRaised += OnUnitSummonedEvent;
        fieldUnitSlainEventChannel.OnEventRaised += OnFieldUnitSlainEvent;
        gameStartEvent.OnEventRaised += OnGameStartEvent;
        
        _fieldUnits.Clear();
        _fieldUnits.Add(playerCastle);
        _fieldUnits.Add(enemyCastle);
    }


    private void OnDestroy()
    {
        unitSummonedEventChannel.OnEventRaised -= OnUnitSummonedEvent;
        fieldUnitSlainEventChannel.OnEventRaised -= OnFieldUnitSlainEvent;
        gameStartEvent.OnEventRaised -= OnGameStartEvent;
    }

    private void OnGameStartEvent(GameModeType gameModeType)
    {
        _gameModeType = gameModeType;
        _gameSettings = GameAssets.Instance.GameModeSettingsList.First(x => x.GameModeType == gameModeType);
        _secondsElapsed = 0f;
        nextThresholdPercentage = 100f - (100f / (_gameSettings.IntervalWaves.Count + 1));
        playerCastle.maxHealth = 1000 * _gameSettings.PlayerCastleHealthMod;
        playerCastle.currentHealth = playerCastle.maxHealth;
        enemyCastle.maxHealth = 1000 * _gameSettings.EnemyCastleHealthMod;
        enemyCastle.currentHealth = enemyCastle.maxHealth;
        gameConfiguredEvent.RaiseEvent();
    }

    public void OnUnitSummonedEvent(GameObject unitPrefab, float lifeForce)
    {
        SpawnFieldUnitPrefab(unitPrefab, true, true);
    }

    private void Update()
    {
        _playTime += Time.deltaTime;

        if (virtualCamera != null)
        {
            // Set camera lookat object to right-most friendly unit
            var friendlyUnits = _fieldUnits.Where(f => f.isPlayerFaction);
            var rightMostUnit = friendlyUnits.OrderByDescending(u => u.transform.position.x).FirstOrDefault();
            virtualCamera.Follow = rightMostUnit.transform;
        }
        
        _secondsElapsed += Time.deltaTime;
        // Check castle HP thresholds
        if ((100f*(enemyCastle.currentHealth / enemyCastle.maxHealth) < nextThresholdPercentage) && enemyCastle.currentHealth > 0)
        {
            // spawn interval wave
            unitWaveEventChannel.RaiseEvent(_gameSettings.IntervalWaves[intervalWavesSpawned]);
            // set next thresholdpercentage
            nextThresholdPercentage -= (100f / (_gameSettings.IntervalWaves.Count + 1));
            intervalWavesSpawned++;
        }
    }

    private void OnFieldUnitSlainEvent(FieldUnit fieldUnit)
    {
        if (fieldUnit.IsCastle)
        {
            if (fieldUnit.isPlayerFaction)
            {
                gameOverEventChannel.RaiseEvent(false, _gameModeType);
            }
            else
            {
                DataPersistenceManager.instance.SaveGame(new GameData() {HighestClearedMode = _gameModeType});
                gameOverEventChannel.RaiseEvent(true, _gameModeType, _playTime);
            }
        }
        else
        {
            _fieldUnits.Remove(fieldUnit);
            if (!fieldUnit.isPlayerFaction)
            {
                _killCount++;
                if (_killCount % hostagePerKills == 0)
                {
                    hostageGainedEventChannel.RaiseEvent();
                }
            }
        }
    }

    public void SpawnFieldUnitPrefab(GameObject unitPrefab, bool isPlayerFaction, bool isSummon)
    {
        var newUnit = Instantiate(unitPrefab);
        var fieldUnit = newUnit.GetComponent<FieldUnit>();
        fieldUnit.ApplyFaction(isPlayerFaction);
        fieldUnit.maxHealth *= (isPlayerFaction ? _gameSettings.PlayerUnitHealthMod : _gameSettings.EnemyUnitHealthMod);
        if (!isSummon)
        {
            // stat boost based on time factor. Every 30 secs boosts enemy ATK and HP by 10%, ally by 5%.
            // enemies get an additional hp and damage mod from the game settings, based on difficulty.
            // capped at 60 minutes.
            _secondsElapsed = Mathf.Min(_secondsElapsed, 3600.0f);
            var statBoostFactor = _secondsElapsed / 300.0f;
            fieldUnit.maxHealth *= (isPlayerFaction ?
                (1.0f + (statBoostFactor * 0.5f)) :
                (1.0f + (statBoostFactor)));
            fieldUnit.timeDamageModifier *= (isPlayerFaction ? 
                (1.0f + (statBoostFactor * 0.5f)) :
                (1.0f + (statBoostFactor)) * _gameSettings.EnemyUnitDamageMod);
        }
        _fieldUnits.Add(fieldUnit);
        SetFieldUnitPosition(fieldUnit);
    }

    private void SetFieldUnitPosition(FieldUnit fieldUnit)
    {
        fieldUnit.transform.SetParent(fieldUnit.isPlayerFaction ? playerFieldUnitParent : enemyFieldUnitParent, false);
        
        var randomOffset = new Vector3(Random.Range(-maxRandomHorizontalOffset, maxRandomHorizontalOffset), Random.Range(-maxRandomVerticalOffset, maxRandomVerticalOffset));
        fieldUnit.transform.position += randomOffset;
        if (fieldUnit.flying)
        {
            fieldUnit.transform.position += Vector3.up * flightHeight;
        }
    }
}
