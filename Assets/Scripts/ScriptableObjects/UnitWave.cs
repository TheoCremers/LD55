using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/UnitWave")]
public class UnitWave : ScriptableObject
{
    public List<UnitTally> units;
}
