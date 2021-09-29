using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleScript : MonoBehaviour
{
    public HealthBar healthBar;
    public GameObject upgradesPanel;
    void Start()
    {
        healthBar.SetMaxHealth(400);
        upgradesPanel.SetActive(false);
    }
    public void TakeDamage(int damage)
    {
        int health = healthBar.GetHealth();
        health -= damage;
        if (health <= 0) Die();
        healthBar.SetHealth(health);
    }

    void Die()
    {
        Destroy(gameObject);
    }

    public void DisplayUpgrades()
    {
        upgradesPanel.SetActive(true);
    }

    public void HideUpgrades()
    {
        upgradesPanel.SetActive(false);
    }
}
