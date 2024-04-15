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
        if (_canBeSummoned)
        {
            unitSummonedEventChannel?.RaiseEvent(unitPrefab, _actualBloodCost);
        }
    }

    public void OnKnowledgeUpdated(float knowledge, float totalLifeForce)
    {
        _canBeSummoned = knowledge >= minKnowledgeRequired;

        if (!_canBeSummoned) 
        { 
            gameObject.SetActive(false);
            return;
        }
        else
        { 
            gameObject.SetActive(true);
        }

        _actualBloodCost = CalculateActualBloodCost(knowledge);

        if (_actualBloodCost < LowRiskThreshold) hintTextMesh.text = SummonHintText.Low;
        else if (_actualBloodCost < MediumRiskthreshold) hintTextMesh.text = SummonHintText.Medium;
        else if (_actualBloodCost < HighRiskThreshold) hintTextMesh.text = SummonHintText.Hard;
        else if (_actualBloodCost < UnknownRisk) hintTextMesh.text = SummonHintText.Extreme;
        else hintTextMesh.text = SummonHintText.Lethal;

        //if (_actualBloodCost >= totalLifeForce)
        //{
        //    hintTextMesh.text = SummonHintText.Lethal;
        //    return;
        //}

        //if (knowledge <= minKnowledgeRequired)
        //{
        //    hintTextMesh.text = SummonHintText.Extreme;
        //    return;
        //}

        //if (knowledge >= maxKnowledgeForSummon)
        //{
        //    hintTextMesh.text = SummonHintText.Low;
        //    return;
        //}

        //hintTextMesh.text = _actualBloodCost < medianBloodCost ? SummonHintText.Medium : SummonHintText.Hard;
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
}
