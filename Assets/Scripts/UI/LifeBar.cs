using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    public RectMask2D mask;
    public float healthRatio;
    public float maxTopMask = 45f;

    private float _initialTopMask = 7f;

    private void Awake()
    {
        mask ??= GetComponentInChildren<RectMask2D>();
        _initialTopMask = mask.padding.w;
        SetHealthRatio(healthRatio);
    }

    public void SetHealthRatio(float ratio)
    {
        healthRatio = ratio;
        var targetTopPadding = Mathf.Lerp(maxTopMask, _initialTopMask, ratio);
        var padding = mask.padding;
        padding.w = targetTopPadding;
        mask.padding = padding;
    }
}
