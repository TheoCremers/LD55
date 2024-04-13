using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    public Image fillImage;
    public float healthRatio;

    public void Start()
    {
        SetHealthRatio(healthRatio);
    }

    public void Update()
    {
        SetHealthRatio(healthRatio);
    }

    public void SetHealthRatio(float ratio)
    {
        healthRatio = ratio;
        fillImage.material.SetFloat("_TransitionRatio", healthRatio);
    }
}
