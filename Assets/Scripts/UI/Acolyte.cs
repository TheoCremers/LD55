using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Acolyte : MonoBehaviour
{
    public Image icon;
    public LifeBar lifeBar;
    public float maxHealth;
    public float currentHealth { get; private set; }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void SetCurrentHealth(float currentHealth)
    {
        this.currentHealth = currentHealth;
        lifeBar.SetHealthRatio(this.currentHealth / maxHealth);
    }

    public float ChangeCurrentHealth(float delta)
    {
        var remainder = 0f;
        var newAmount = currentHealth + delta;
        if (newAmount < 0f)
        {
            remainder = -newAmount;
            newAmount = 0f;
        }
        
        SetCurrentHealth(newAmount);
        return remainder;
    }

    public async Task DeathAnimation()
    {
        icon.DOKill();
        await icon.DOColor(new Color(0.25f, 0, 0), 0.2f).AsyncWaitForCompletion();
        await icon.DOFade(0f, 0.2f).AsyncWaitForCompletion();
        Destroy(gameObject);
    }
}
