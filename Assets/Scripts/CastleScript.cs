using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleScript : MonoBehaviour
{
    public HealthBar healthBar;
    public ShieldBarScript shieldBar;
    public GameObject upgradesPanel;
    public event System.EventHandler OnHideUpgrades;
    public event System.EventHandler OnDie;
    public event System.EventHandler OnShowUpgrades;

    public int DefenseUpgradeLevel { get; private set; } = 0;
    public int HealthUpgradeLevel { get; private set; } = 0;
    public int ShieldUpgradeLevel { get; private set; } = 0;
    public int MoneyUpgradeLevel { get; private set; } = 0;
    public int Money { get; private set; } = 200;
    private int defense = 5;
    private float moneyMultiplier = 1.0f;
    void Start()
    {
        healthBar.SetInitHealth(200);
        upgradesPanel.SetActive(false);
        shieldBar.SetInitShield(50);
    }
    public void TakeDamage(int damage)
    {
        damage = shieldBar.TakeDamage(damage);
        if (damage == 0) return;
        damage -= defense;
        int health = healthBar.GetHealth();
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
        OnShowUpgrades?.Invoke(this, System.EventArgs.Empty);
    }

    public void HideUpgrades(bool invokeHandler = true)
    {
        upgradesPanel.SetActive(false);
        if (invokeHandler)
        {
            OnHideUpgrades?.Invoke(this, System.EventArgs.Empty);
        }
    }

    public void UpgradeDefense(int moneyCost)
    {
        defense++;
        DefenseUpgradeLevel++;
        Money -= moneyCost;
    }

    public void UpgradeHealth(int moneyCost)
    {
        healthBar.SetMaxHealth(healthBar.GetMaxHealth() + 20);
        healthBar.SetHealth(healthBar.GetHealth() + 40
            > healthBar.GetMaxHealth() ? healthBar.GetMaxHealth() : healthBar.GetHealth() + 40);
        HealthUpgradeLevel++;
        Money -= moneyCost;
    }

    public void UpgradeShield(int moneyCost)
    {
        ShieldUpgradeLevel++;
        shieldBar.SetMaxShield(shieldBar.GetMaxShield() + 10);
        Money -= moneyCost;
    }
    public void UpgradeMoney(int moneyCost)
    {
        MoneyUpgradeLevel++;
        moneyMultiplier += 0.1f;
        Money -= moneyCost;
    }

    public int GetMaxHealth()
    {
        return healthBar.GetMaxHealth();
    }
    public int GetHealth()
    {
        return healthBar.GetHealth();
    }
    public float GetMoneyMultiplier()
    {
        return moneyMultiplier;
    }
    public int GetShield()
    {
        return shieldBar.GetShield();
    }
}
