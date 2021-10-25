using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Castle : MonoBehaviour
{
    public HealthBar healthBar;
    public ShieldBar shieldBar;
    public UpgradesSystem upgradesPanel;
    public GameObject turretsPanel;
    public event System.EventHandler OnDie;
    public event System.EventHandler OnShowUpgrades;

    public int DefenseUpgradeLevel { get; private set; } = 0;
    public int HealthUpgradeLevel { get; private set; } = 0;
    public int ShieldUpgradeLevel { get; private set; } = 0;
    public int MoneyUpgradeLevel { get; private set; } = 0;
    public int Money { get; private set; } = 200;
    [SerializeField] private int defense = 5;
    [SerializeField] private List<GameObject> hideableUi;
    private EnemySpawner engine;


    private float moneyMultiplier = 1.0f;
    void Start()
    {
        engine = FindObjectOfType<EnemySpawner>();
        healthBar.SetInitHealth(200);
        shieldBar.SetInitShield(50);
        HideUpgrades();
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

    public void ShowUpgrades()
    {
        upgradesPanel.gameObject.SetActive(true);
        turretsPanel.SetActive(true);
        foreach (var obj in hideableUi)
            obj?.SetActive(true);
        upgradesPanel.PrepareShops(Money);
        engine?.ShowHand();
    }

    public void HideUpgrades(bool doHideAll = true)
    {
        upgradesPanel.gameObject.SetActive(false);
        turretsPanel.SetActive(false);
        if (doHideAll)
        {
            foreach (var obj in hideableUi)
                obj?.SetActive(false);
            engine?.ShowCrosshair();
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

    public void AddMoney(int value)
    {
        Money += (int)(value * moneyMultiplier);
    }

    public void SpendMoney(int value)
    {
        if (value <= 0) return;
        Money -= value;
    }
}
