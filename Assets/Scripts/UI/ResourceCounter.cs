using TMPro;
using UnityEngine;

public class ResourceCounter : MonoBehaviour
{
    public TextMeshProUGUI amountTextMesh;
    public float startingAmount = 0f;
    public float amount { get; private set; }

    private void Awake()
    {
        SetResourceAmount(startingAmount);
    }

    public void SetResourceAmount(float amount)
    {
        this.amount = amount;
        amountTextMesh.text = Mathf.RoundToInt(this.amount).ToString();
    }

    public float ChangeResource(float delta)
    {
        var remainder = 0f;
        var newAmount = amount + delta;
        if (newAmount < 0f)
        {
            remainder = -newAmount;
            newAmount = 0f;
        }

        SetResourceAmount(newAmount);
        return remainder;
    }
}
