using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    public RectMask2D mask;
    public float healthRatio;

    private float _maxTopMask = 45f;
    private float _initialTopMask = 7f;

    private void Start()
    {
        SetHealthRatio(healthRatio);
    }

    public void SetHealthRatio(float ratio)
    {
        healthRatio = ratio;
        var targetTopPadding = Mathf.Lerp(_maxTopMask, _initialTopMask, ratio);
        var padding = mask.padding;
        padding.w = targetTopPadding;
        mask.padding = padding;
    }
}
