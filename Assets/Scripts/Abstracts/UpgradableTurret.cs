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
    [SerializeField] private List<GameObject> upgradesNewGameObjects;
    [SerializeField] private GameObject upgradesPanel;
    private int MaxUpgradeLvl { get { return upgradesCostsList.Count; } }
    [SerializeField] private GameObject cannon;
    public bool Upgrade(int money)
    {
        if (MaxUpgradeLvl == UpgradeLvl) return false;
        if (UpgradeCost < 0) return false;
        if (money < UpgradeCost) return false;
        if (upgradesNewGameObjects != null && upgradesNewGameObjects.Count >= UpgradeLvl + 1)
            upgradesNewGameObjects[UpgradeLvl]?.SetActive(true);
        UpgradeLvl++;
        return true;
    }

    public void HideUpgrades()
    {
        upgradesPanel.SetActive(false);
    }

    public void ShowUpgrades()
    {
        if (UpgradeCost > 0)
            upgradesPanel.SetActive(true);
    }

    protected void CannonLookAt(Transform lookTarget)
    {
        if (cannon) cannon.transform.LookAt(lookTarget);
    }
}

