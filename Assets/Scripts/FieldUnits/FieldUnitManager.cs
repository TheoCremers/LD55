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
    public VoidEventChannel gameConfiguredEvent;

    public FieldUnit playerCastle;
    public FieldUnit enemyCastle;

    public CinemachineVirtualCamera virtualCamera;

    private GameModeType _gameModeType;
    private GameModeSettings _gameSettings;

    private static List<FieldUnit> _fieldUnits = new List<FieldUnit>();
    public static List<FieldUnit> fieldUnits => _fieldUnits;

    public const float flightHeight = 1.35f;
    public const float maxRandomHorizontalOffset = 0.1f;
    public const float maxRandomVerticalOffset = 0.15f;
    public int hostagePerKills = 5;
    private static int _killCount;

    private void Awake()
    {
        _killCount = 0;
        unitSummonedEventChannel.OnEventRaised += OnUnitSummonedEvent;
        fieldUnitSlainEventChannel.OnEventRaised += OnFieldUnitSlainEvent;
        gameStartEvent.OnEventRaised += OnGameStartEvent;
        
        _fieldUnits.Clear();
        _fieldUnits.Add(playerCastle);
        _fieldUnits.Add(enemyCastle);
    }

    private void Update()
    {
        if (virtualCamera != null)
        {
            // Set camera lookat object to right-most friendly unit
            var friendlyUnits = _fieldUnits.Where(f => f.isPlayerFaction);
            var rightMostUnit = friendlyUnits.OrderByDescending(u => u.transform.position.x).FirstOrDefault();
            virtualCamera.Follow = rightMostUnit.transform;
        }
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
        playerCastle.maxHealth = 1000 * _gameSettings.PlayerCastleHealthMod;
        playerCastle.currentHealth = playerCastle.maxHealth;
        enemyCastle.maxHealth = 1000 * _gameSettings.EnemyCastleHealthMod;
        enemyCastle.currentHealth = enemyCastle.maxHealth;
        gameConfiguredEvent.RaiseEvent();
    }

    public void OnUnitSummonedEvent(GameObject unitPrefab, float lifeForce)
    {
        SpawnFieldUnitPrefab(unitPrefab, true);
    }

    private void OnFieldUnitSlainEvent(FieldUnit fieldUnit)
    {
        if (fieldUnit.IsCastle)
        {
            if (fieldUnit.isPlayerFaction)
            {
                // gg go next
                // TODO: Loss animation, or text, or something
                SceneManager.LoadScene( SceneManager.GetActiveScene().name );
            }
            else
            {
                // TODO: Win screen, or text, or credits
                DataPersistenceManager.instance.SaveGame(new GameData() {HighestClearedMode = _gameModeType});
                SceneManager.LoadScene( SceneManager.GetActiveScene().name );
            }
        }
        else
        {
            // TODO: Death effect / animation
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

    public void SpawnFieldUnitPrefab(GameObject unitPrefab, bool isPlayerFaction)
    {
        var newUnit = Instantiate(unitPrefab);
        var fieldUnit = newUnit.GetComponent<FieldUnit>();
        fieldUnit.ApplyFaction(isPlayerFaction);
        fieldUnit.maxHealth *= (isPlayerFaction ? _gameSettings.PlayerUnitHealthMod : _gameSettings.EnemyUnitHealthMod);
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
