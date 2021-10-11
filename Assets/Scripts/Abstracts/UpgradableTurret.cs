using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UpgradableTurret : MonoBehaviour
{
    public int UpgradeLvl { get; private set; } = 0;
    public int UpgradeCost
    {
        get
        {
            return upgradesCostsList.Count < UpgradeLvl + 1 ? -1 : upgradesCostsList[UpgradeLvl];
        }
    }
    [SerializeField] private List<int> upgradesCostsList;
    [SerializeField] private GameObject upgradesPanel;
    private int MaxUpgradeLvl { get { return upgradesCostsList.Count; } }

    public bool Upgrade(int money)
    {
        if (MaxUpgradeLvl == UpgradeLvl) return false;
        if (UpgradeCost < 0) return false;
        if (money < UpgradeCost) return false;
        UpgradeLvl++;
        return true;
    }

    public void HideUpgrades()
    {
        upgradesPanel.SetActive(false);
    }

    public void ShowUpgrades()
    {
        upgradesPanel.SetActive(true);
    }
}

