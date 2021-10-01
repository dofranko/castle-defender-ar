using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesSystemScript : MonoBehaviour
{

    public CastleScript castle;
    [SerializeField] private Text timerText;
    [SerializeField] private Text defenseText;
    [SerializeField] private Text healthText;
    [SerializeField] private Text shieldText;
    [SerializeField] private Text moneyText;
    [SerializeField] private TMPro.TMP_Text infoText;
    [SerializeField] private Text moneyInfoText;
    private Camera cam;
    private int masksToFilter;

    private void Awake()
    {
        castle.OnShowUpgrades += OnCastleShowUpgrades;
    }
    void Start()
    {
        cam = Camera.main;
        masksToFilter = LayerMask.GetMask("Raycastable UI");
    }
    private void OnCastleShowUpgrades(object? sender, System.EventArgs e)
    {
        moneyInfoText.text = $"Money: {castle.Money}";
        infoText.text = "Upgrade equipement or start new wave...";
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 3, masksToFilter))
            {
                Debug.Log("hit " + hit.transform.name);
                switch (hit.transform.name)
                {
                    case "DefenseImage":
                        if (castle.Money < 20)
                        {
                            ShowNotEnoughMoney();
                            break;
                        }
                        castle.UpgradeDefense(20); //TODO Parametrize
                        defenseText.text = $"Lvl {castle.DefenseUpgradeLevel}";
                        infoText.text = $"Upgraded defense from lvl {castle.DefenseUpgradeLevel - 1} to lvl {castle.DefenseUpgradeLevel}";
                        break;
                    case "HealthImage":
                        if (castle.Money < 20)
                        {
                            ShowNotEnoughMoney();
                            break;
                        }
                        castle.UpgradeHealth(20);
                        healthText.text = $"Lvl {castle.HealthUpgradeLevel}";
                        infoText.text = $"Upgraded health from lvl {castle.HealthUpgradeLevel - 1} to lvl {castle.HealthUpgradeLevel}\n";
                        infoText.text += $"Health: {castle.GetHealth()}/{castle.GetMaxHealth()}";
                        break;
                    case "ShieldImage":
                        if (castle.Money < 20)
                        {
                            ShowNotEnoughMoney();
                            break;
                        }
                        castle.UpgradeShield(20);
                        shieldText.text = $"Lvl {castle.ShieldUpgradeLevel}";
                        infoText.text = $"Upgraded shield from lvl {castle.ShieldUpgradeLevel - 1} to lvl {castle.ShieldUpgradeLevel}\n";
                        infoText.text += $"Shield: {castle.GetShield()}";
                        break;
                    case "MoneyImage":
                        if (castle.Money < 20)
                        {
                            ShowNotEnoughMoney();
                            break;
                        }
                        castle.UpgradeMoney(20);
                        moneyText.text = $"Lvl {castle.MoneyUpgradeLevel}";
                        infoText.text = $"Upgraded money bonus from lvl {castle.MoneyUpgradeLevel - 1} to lvl {castle.MoneyUpgradeLevel}\n";
                        infoText.text += $"Money multiplier: {castle.GetMoneyMultiplier()}";
                        break;
                    case "SkipImage":
                        castle.HideUpgrades();
                        break;
                }
                moneyInfoText.text = $"Money: {castle.Money}";
            }
        }
    }

    public void SetTimerText(string text)
    {
        timerText.text = text;
    }

    private void ShowNotEnoughMoney()
    {
        infoText.text = "Not enough money to buy it";
    }
}
