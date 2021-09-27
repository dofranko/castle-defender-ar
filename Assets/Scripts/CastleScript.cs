using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleScript : MonoBehaviour
{
    public HealthBar healthBar;

    void Start()
    {
        healthBar.SetMaxHealth(100);
    }
    public void TakeDamage(int damage)
    {
        Debug.Log("taking dmg");
        int health = healthBar.GetHealth();
        health -= damage;
        if (health <= 0) Die();
        healthBar.SetHealth(health);
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
