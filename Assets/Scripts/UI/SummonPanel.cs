using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SummonPanel : MonoBehaviour
{
    public ResourceUpdateEventChannel resourceUpdateEventChannel;
    private List<Summon> _summonList;

    private void Awake()
    {
        _summonList = GetComponentsInChildren<Summon>().ToList();

        resourceUpdateEventChannel.OnEventRaised += UpdateAllSummons;
    }

    public void UpdateAllSummons(float knowledge, float totalLifeForce)
    {
        foreach (var summon in _summonList) 
        { 
            summon.OnKnowledgeUpdated(knowledge, totalLifeForce);
        }
    }

    private void OnDestroy()
    {
        resourceUpdateEventChannel.OnEventRaised -= UpdateAllSummons;
    }
}
