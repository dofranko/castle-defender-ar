using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeTurretSystem : MonoBehaviour
{
    [SerializeField] private UpgradableTurret turret;
    [SerializeField] private TMPro.TMP_Text costText;
    [SerializeField] private TMPro.TMP_Text lvlText;


    void Start()
    {
        UpdateTexts();
    }
    public int UpgradeTurret(int money)
    {
        var upgCost = turret.UpgradeCost;
        if (upgCost < 0) return 0;
        if (turret.Upgrade(money))
        {
            UpdateTexts();
            return upgCost;
        }
        return 0;
    }
    void UpdateTexts()
    {
        if (turret.UpgradeCost < 0)
        {
            costText.text = $"";
            lvlText.text = $"Max lvl";
        }
        else
        {
            costText.text = $"Cost: {turret.UpgradeCost}";
            lvlText.text = $"lvl {turret.UpgradeLvl} -> {turret.UpgradeLvl + 1}";
        }
    }
}
