using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleScript : MonoBehaviour
{
    public HealthBar healthBar;
    public GameObject upgradesPanel;
    public int defense;
    public event System.EventHandler OnHideUpgrades;
    public event System.EventHandler OnDie;

    void Start()
    {
        healthBar.SetMaxHealth(400);
        upgradesPanel.SetActive(false);
    }
    public void TakeDamage(int damage)
    {
        int health = healthBar.GetHealth();
        damage -= defense;
        if (damage <= 0) damage = 1;
        health -= damage;
        healthBar.SetHealth(health);
        if (health <= 0) Die();
    }

    void Die()
    {
        Destroy(gameObject);
        OnDie?.Invoke(this, System.EventArgs.Empty);
    }

    public void DisplayUpgrades()
    {
        upgradesPanel.SetActive(true);
    }

    public void HideUpgrades(bool invokeHandler = true)
    {
        upgradesPanel.SetActive(false);
        if (invokeHandler)
        {
            OnHideUpgrades?.Invoke(this, System.EventArgs.Empty);
        }
    }
}
