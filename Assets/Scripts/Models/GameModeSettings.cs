using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameModeSettings : ScriptableObject
{
    public GameModeType GameModeType;

    public float PlayerCastleHealthMod;
    
    public float EnemyCastleHealthMod;
    
    public float PlayerUnitHealthMod;
    
    public float EnemyUnitHealthMod;
    
    public List<UnitWave> IntervalWaves;
}
