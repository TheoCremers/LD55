using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public ResourceCounter lifeForceCounter;
    public ResourceCounter knowledgeCounter;
    public AcolytePanel acolytePanel;
    public SummonPanel summonPanel;
    public HostagePanel hostagePanel;
    public UnitSummonedEventChannel unitSummonedEventChannel;
    public HostageUsedEventChannel hostageUsedEventChannel;
    public ResourceUpdateEventChannel resourceUpdateEventChannel;

    public float sacrificeMultiplier = 5f;

    private void Awake()
    {
        acolytePanel ??= GetComponentInChildren<AcolytePanel>();

        unitSummonedEventChannel.OnEventRaised += OnUnitSummmoned;
        hostageUsedEventChannel.OnEventRaised += OnHostageUsed;
    }

    private void Start()
    {
        summonPanel.UpdateAllSummons(knowledgeCounter.amount, CalculateTotalLifeForce());
    }

    private void OnDestroy()
    {
        unitSummonedEventChannel.OnEventRaised -= OnUnitSummmoned;
        hostageUsedEventChannel.OnEventRaised -= OnHostageUsed;
    }

    public void OnUnitSummmoned(GameObject unit, float bloodCost)
    {
        // drain life resources, first from bank, then from acolytes
        var remainder = lifeForceCounter.ChangeResource(-bloodCost);
        if (remainder > 0f)
        {
            if (!acolytePanel.DrainBloodFromAcolytes(remainder))
            {
                //TODO: Game over here
            }
        }
        resourceUpdateEventChannel.RaiseEvent(knowledgeCounter.amount, CalculateTotalLifeForce());

        if (acolytePanel.hasEmptySlots)
        {
            hostagePanel.AllowRecruit();
        }
    }

    public void OnHostageUsed(HostageUseType type, float knowledge, float lifeForce, float ransomValue)
    {
        switch (type)
        {
            case HostageUseType.Sacrifice:
                OnSacrifice(lifeForce);
                break;
            case HostageUseType.Recruit:
                OnRecruit(knowledge, lifeForce);
                break;
            case HostageUseType.Ransom:
                OnAcolyteSlotAdd();
                break;
        }
    }

    private void OnSacrifice(float lifeForce)
    {
        lifeForceCounter.ChangeResource(lifeForce * sacrificeMultiplier);
        resourceUpdateEventChannel.RaiseEvent(knowledgeCounter.amount, CalculateTotalLifeForce());
    }

    private void OnRecruit(float knowledge, float lifeForce)
    {
        knowledgeCounter.ChangeResource(knowledge);
        acolytePanel.AddAcolyte(lifeForce);
        resourceUpdateEventChannel.RaiseEvent(knowledgeCounter.amount, CalculateTotalLifeForce());

        if (!acolytePanel.hasEmptySlots)
        {
            hostagePanel.BlockRecruit();
        }
    }

    private void OnAcolyteSlotAdd()
    {
        acolytePanel.AddAcolyteSlot();
        hostagePanel.AllowRecruit();
    }

    private float CalculateTotalLifeForce()
    {
        return lifeForceCounter.amount + acolytePanel.GetTotalLifeForce();
    }
}
