using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int maxHealth = 10;
    private int currentHealth;

    public BulletType weakAgainst;
    public UnityEvent onHit;
    public UnityEvent onDeath;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount, BulletType incomingType)
    {
        if (incomingType != weakAgainst) return;
        print("Hit! " + amount + " damage taken.");
        currentHealth -= amount;
        onHit?.Invoke();

        if (currentHealth <= 0)
        {
            print("Enemy died.");
            Die();
        }
    }

    private void Die()
    {
        onDeath?.Invoke();
    }
}