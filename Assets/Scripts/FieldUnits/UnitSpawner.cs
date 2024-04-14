using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public UnitWave unitWave;
    public bool isPlayerFaction;
    public float baseSpawnInterval = 15f;

    private FieldUnitManager _fieldUnitManager;
    private SimpleTimer _spawnTimer;

    private void Awake()
    {
        _fieldUnitManager = GetComponentInParent<FieldUnitManager>();
        _spawnTimer = gameObject.AddComponent<SimpleTimer>();
        _spawnTimer.SetTimer(baseSpawnInterval, true);
        _spawnTimer.OnTimerElapsed += SpawnWave;
        _spawnTimer.StartTimer();

        SpawnWave();
    }

    public async void SpawnWave()
    {
        Debug.Log("Spawn unit wave");

        foreach (var unitTally in unitWave.units)
        {
            var amountSpawned = Random.Range(unitTally.minAmount, unitTally.maxAmount + 1); // +1 because it's max exclusive for integers
            for (int i = 0; i < amountSpawned; i++)
            {
                _fieldUnitManager.SpawnFieldUnitPrefab(unitTally.unit, isPlayerFaction);
                await TimeHelper.WaitForSeconds(0.2f);
            }
        }
    }
}