using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesSystem : MonoBehaviour
{

    [SerializeField] private Castle castle;
    [SerializeField] private GameObject basicTurretGameObject;
    [SerializeField] private GameObject explosiveTurretGameObject;
    [SerializeField] private GameObject electricTurretGameObject;
    [SerializeField] private GameObject frozingTurretGameObject;
    [SerializeField] private Text timerText;
    [SerializeField] private Text defenseText;
    [SerializeField] private Text healthText;
    [SerializeField] private Text shieldText;
    [SerializeField] private Text moneyText;
    [SerializeField] private TMPro.TMP_Text infoText;
    [SerializeField] private Text moneyInfoText;
    [SerializeField] private LayerMask raycastableUILayerMask;
    private Camera cam;
    private Placement placement;
    private void Awake()
    {
        castle.OnShowUpgrades += OnCastleShowUpgrades;
    }
    void Start()
    {
        cam = Camera.main;
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
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 100, raycastableUILayerMask))
            {
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
                    case "BasicTurretImage":
                        var place = GetPlacement();
                        if (place)
                        {
                            place.enabled = true;
                            place.castleToPlace = basicTurretGameObject;
                        }
                        break;

                    case "ExplosiveTurretImage":
                        var placeE = GetPlacement();
                        if (placeE)
                        {
                            placeE.enabled = true;
                            placeE.castleToPlace = explosiveTurretGameObject;
                        }
                        break;
                    case "ElectricTurretImage":
                        var placeEl = GetPlacement();
                        if (placeEl)
                        {
                            placeEl.enabled = true;
                            placeEl.castleToPlace = electricTurretGameObject;
                        }
                        break;
                    case "FrozingTurretImage":
                        var placeF = GetPlacement();
                        if (placeF)
                        {
                            placeF.enabled = true;
                            placeF.castleToPlace = frozingTurretGameObject;
                        }
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

    private Placement GetPlacement()
    {
        if (!placement) placement = FindObjectOfType<Placement>();
        return placement;
    }
}
