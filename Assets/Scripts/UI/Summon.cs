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
    public bool canBeSummoned = true;

    public float medianBloodCost { get { return (maxBloodCost + minBloodcost) * 0.5f; } }

    private float _actualBloodCost = 0f;
    private float _randomFactor = 0.1f;

    private const float LowRiskThreshold = 100f;
    private const float MediumRiskthreshold = 250f;
    private const float HighRiskThreshold = 500f;
    private const float UnknownRisk = 1000f;

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
        if (canBeSummoned)
        {
            unitSummonedEventChannel?.RaiseEvent(unitPrefab, ApplyRandomFactor(_actualBloodCost));
        }
    }

    public void OnKnowledgeUpdated(float knowledge, float totalLifeForce, int numberOfAcolytes)
    {
        summonButton.interactable = numberOfAcolytes > 0;
        canBeSummoned = knowledge >= minKnowledgeRequired;
        if (numberOfAcolytes <= 0)
        {
            hintTextMesh.text = SummonHintText.Impossible;
            return;
        }

        _actualBloodCost = CalculateActualBloodCost(knowledge);

        if (!canBeSummoned) hintTextMesh.text = SummonHintText.Unknown;
        else if (_actualBloodCost < LowRiskThreshold) hintTextMesh.text = SummonHintText.Low;
        else if (_actualBloodCost < MediumRiskthreshold) hintTextMesh.text = SummonHintText.Medium;
        else if (_actualBloodCost < HighRiskThreshold) hintTextMesh.text = SummonHintText.Hard;
        else if (_actualBloodCost < UnknownRisk) hintTextMesh.text = SummonHintText.Extreme;
        else hintTextMesh.text = SummonHintText.Lethal;
    }

    private float CalculateActualBloodCost(float knowledge)
    {
        if (knowledge <= minKnowledgeRequired) return maxBloodCost;
        if (knowledge >= maxKnowledgeForSummon) return minBloodcost;

        float knowledgeGap = maxKnowledgeForSummon - minKnowledgeRequired;
        if (knowledgeGap <= 0) throw new System.Exception("knowledgeGap is negative, check your summon config");
        float scaledKnowledge = (knowledge - minKnowledgeRequired) / knowledgeGap;
        return Mathf.SmoothStep(maxBloodCost, minBloodcost, scaledKnowledge);
    }

    private float ApplyRandomFactor(float input)
    {
        return Random.Range(input * (1f - _randomFactor), input * (1f + _randomFactor));
    }
}
