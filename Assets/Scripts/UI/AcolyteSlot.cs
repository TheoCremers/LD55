using UnityEngine;

public class AcolyteSlot : MonoBehaviour
{
    public Acolyte acolyte = null;
    public ParticleSystem summoningParticleSystem;

    public bool isActive { get; private set; } = true;
    public bool hasAcolyte => acolyte != null;
    public bool isAcolyteAlive => hasAcolyte? acolyte.currentHealth > 0 : false;

    private void Awake()
    {
        acolyte ??= GetComponentInChildren<Acolyte>();
        if (acolyte != null)
        {
            summoningParticleSystem.Play();
        }
    }

    public bool AddAcolyte(float lifeForce, GameObject acolytePrefab)
    {
        if (acolyte != null) return false;

        var newAcolyteObject = Instantiate(acolytePrefab, transform, false);
        var newAcolyte = newAcolyteObject.GetComponent<Acolyte>();
        newAcolyte.maxHealth = lifeForce;
        acolyte = newAcolyte;
        summoningParticleSystem.Play();
        return true;
    }

    public void SacrificeAcolyte()
    {
        summoningParticleSystem.Stop();
        _ = acolyte.DeathAnimation();
        acolyte = null;
    }

    public void DisableSlot()
    {
        isActive = false;
        gameObject.SetActive(false);
    }

    public void EnableSlot()
    {
        isActive = true;
        gameObject.SetActive(true);
    }
}
