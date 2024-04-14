using UnityEngine;

public class Hostage : MonoBehaviour
{
    public float knowledge = 100f;
    public float lifeForce = 100f;
    public float ransomValue = 10f;

    const float minKnowledge = 80f;
    const float maxKnowledge = 120f;
    const float minLifeForce = 100f;
    const float maxLifeForce = 150f;
    const float minRansomValue = 1f;
    const float maxRansomValue = 100f;

    private void Awake()
    {
        RandomizeStats();
    }

    private void RandomizeStats()
    {
        knowledge = Random.Range(minKnowledge, maxKnowledge);
        lifeForce = Random.Range(minLifeForce, maxLifeForce);
        ransomValue = Random.Range(minRansomValue, maxRansomValue);
    }
}
