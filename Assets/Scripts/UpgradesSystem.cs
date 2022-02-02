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
        moneyInfoText.text = $"{money}";
        infoText.text = "Upgrade your Castle";
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
                        castle.UpgradeDefense(20);
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
                        if (!PlaceTurret(basicTurretCost, basicTurretGameObject))
                            ShowNotEnoughMoney();
                        break;

                    case "ExplosiveTurretImage":
                        if (!PlaceTurret(explosiveTurretCost, explosiveTurretGameObject))
                            ShowNotEnoughMoney();
                        break;
                    case "ElectricTurretImage":
                        if (!PlaceTurret(electricTurretCost, electricTurretGameObject))
                            ShowNotEnoughMoney();
                        break;
                    case "FrozingTurretImage":
                        if (!PlaceTurret(frozingTurretCost, frozingTurretGameObject))
                            ShowNotEnoughMoney();
                        break;
                    case "UpgradeTurretImage":
                        var uts = hit.transform.GetComponentInParent<UpgradeTurretSystem>();
                        if (uts) castle.SpendMoney(uts.UpgradeTurret(castle.Money));
                        break;
                }
                moneyInfoText.text = $"{castle.Money}";
            }
        }
    }

    private bool PlaceTurret(int cost, GameObject turretPrefab)
    {
        var placeF = GetPlacement();
        if (placeF && castle.Money >= cost)
        {
            placeF.enabled = true;
            placeF.castleToPlace = turretPrefab;
            placeF.OnPlace += OnTurretPlace;
            castle.SpendMoney(cost);
            castle.HideUpgrades(false);
            return true;
        }
        return false;
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
    private void OnTurretPlace(object? sender, System.EventArgs e)
    {
        (sender as Placement).OnPlace -= OnTurretPlace;
        var engine = FindObjectOfType<EnemySpawner>();
        if (engine && engine.state == EnemySpawner.State.InBetweenWaves)
        {
            castle.ShowUpgrades();
        }

    }
}
