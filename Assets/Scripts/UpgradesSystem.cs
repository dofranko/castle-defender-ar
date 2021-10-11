using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesSystem : MonoBehaviour
{

    [SerializeField] private Castle castle;
    [SerializeField] private GameObject basicTurretGameObject;
    [SerializeField] private int basicTurretCost;
    [SerializeField] private GameObject explosiveTurretGameObject;
    [SerializeField] private int explosiveTurretCost;
    [SerializeField] private GameObject electricTurretGameObject;
    [SerializeField] private int electricTurretCost;
    [SerializeField] private GameObject frozingTurretGameObject;
    [SerializeField] private int frozingTurretCost;
    [SerializeField] private Text timerText;
    [SerializeField] private Text defenseText;
    [SerializeField] private Text healthText;
    [SerializeField] private Text shieldText;
    [SerializeField] private Text moneyText;
    [SerializeField] private TMPro.TMP_Text infoText;
    [SerializeField] private Text moneyInfoText;
    [SerializeField] private LayerMask raycastableUILayerMask;
    public event System.EventHandler OnSkipButtonPressed;
    private Camera cam;
    private Placement placement;

    void Start()
    {
        cam = Camera.main;
    }
    public void PrepareShops(int money)
    {
        moneyInfoText.text = $"Money: {money}";
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
                        OnSkipButtonPressed?.Invoke(this, System.EventArgs.Empty);
                        break;
                    case "BasicTurretImage":
                        var place = GetPlacement();
                        if (place && castle.Money >= basicTurretCost)
                        {
                            place.enabled = true;
                            place.castleToPlace = basicTurretGameObject;
                            castle.SpendMoney(basicTurretCost);
                        }
                        else ShowNotEnoughMoney();
                        break;

                    case "ExplosiveTurretImage":
                        var placeE = GetPlacement();
                        if (placeE && castle.Money >= explosiveTurretCost)
                        {
                            placeE.enabled = true;
                            placeE.castleToPlace = explosiveTurretGameObject;
                            castle.SpendMoney(explosiveTurretCost);
                        }
                        else ShowNotEnoughMoney();
                        break;
                    case "ElectricTurretImage":
                        var placeEl = GetPlacement();
                        if (placeEl && castle.Money >= electricTurretCost)
                        {
                            placeEl.enabled = true;
                            placeEl.castleToPlace = electricTurretGameObject;
                            castle.SpendMoney(electricTurretCost);
                        }
                        else ShowNotEnoughMoney();
                        break;
                    case "FrozingTurretImage":
                        var placeF = GetPlacement();
                        if (placeF && castle.Money >= frozingTurretCost)
                        {
                            placeF.enabled = true;
                            placeF.castleToPlace = frozingTurretGameObject;
                            castle.SpendMoney(frozingTurretCost);
                        }
                        else ShowNotEnoughMoney();
                        break;
                    case "UpgradeTurretImage":
                        var uts = hit.transform.GetComponentInParent<UpgradeTurretSystem>();
                        if (uts) castle.SpendMoney(uts.UpgradeTurret(castle.Money));
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
