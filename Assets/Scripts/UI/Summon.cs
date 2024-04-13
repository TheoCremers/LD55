using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Summon : MonoBehaviour
{
    public Button summonButton;
    public Image summonIcon;
    public TextMeshProUGUI hintTextMesh;
    public GameObject unitPrefab;
    public UnitSummonedEventChannel unitSummonedEventChannel; 

    public float minKnowledgeRequired = 100f;
    public float maxKnowledgeForSummon = 400f;
    public float maxBloodCost = 200f;
    public float minBloodcost = 50f;

    public float medianBloodCost { get { return (maxBloodCost + minBloodcost) * 0.5f; } }

    private bool _canBeSummoned = true;
    private float _actualBloodCost = 0f;

    private void Start()
    {
        summonButton ??= GetComponentInChildren<Button>();
        summonButton?.onClick.AddListener(OnSummonButtonDown);
    }

    private void OnDestroy()
    {
        summonButton?.onClick.RemoveAllListeners();
    }

    private void OnSummonButtonDown()
    {
        if (_canBeSummoned)
        {
            unitSummonedEventChannel?.RaiseEvent(unitPrefab, _actualBloodCost);
        }
    }

    public void OnKnowledgeUpdated(float knowledge, float totalLifeForce)
    {
        //_canBeSummoned = knowledge >= minKnowledgeRequired;

        if (knowledge <= minKnowledgeRequired)
        {
            _actualBloodCost = maxBloodCost;
            hintTextMesh.text = SummonHintText.Extreme;
            return;
        }

        if (knowledge >= maxKnowledgeForSummon)
        {
            _actualBloodCost = minBloodcost;
            hintTextMesh.text = SummonHintText.Low;
            return;
        }

        _actualBloodCost = CalculateActualBloodCost(knowledge);

        if (_actualBloodCost > totalLifeForce)
        {
            hintTextMesh.text = SummonHintText.Lethal;
        }
        else 
        {
            hintTextMesh.text = _actualBloodCost < medianBloodCost ? SummonHintText.Medium : SummonHintText.Hard;
        }
    }

    private float CalculateActualBloodCost(float knowledge)
    {
        float knowledgeGap = (maxKnowledgeForSummon - minKnowledgeRequired);
        if (knowledgeGap <= 0) { return minBloodcost; }
        float scaledKnowledge = (knowledge - minKnowledgeRequired) / knowledgeGap;
        return Mathf.SmoothStep(maxBloodCost, minBloodcost, scaledKnowledge);
    }
}
