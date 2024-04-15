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

    public void UpdateAllSummons(float knowledge, float totalLifeForce, int numberOfAcolytes)
    {
        var unknownSummonSet = false;
        foreach (var summon in _summonList) 
        { 
            summon.OnKnowledgeUpdated(knowledge, totalLifeForce, numberOfAcolytes);
            if (!summon.canBeSummoned && !unknownSummonSet)
            {
                summon.canBeSummoned = true;
                unknownSummonSet = true;
            }

            summon.gameObject.SetActive(summon.canBeSummoned);
        }
    }

    private void OnDestroy()
    {
        resourceUpdateEventChannel.OnEventRaised -= UpdateAllSummons;
    }
}
