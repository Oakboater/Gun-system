using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public UnityEvent onDeath; // Assign in Inspector for death effects
    public Slider healthSlider; // Assign UI Slider in Inspector
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        UpdateHealthUI();
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }



    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }


    void Die()
    {
        onDeath.Invoke();
        Destroy(gameObject);
    }
}
