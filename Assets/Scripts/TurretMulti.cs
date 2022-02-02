using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMulti : UpgradableTurret
{
    [SerializeField] private float fireRate;
    [SerializeField] private float retargetRate;
    [SerializeField] private int damage;
    [SerializeField] private float range;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private WeaponPrefabMulti weapon;
    private float nextTimeToFire = 0.0f;
    private List<Transform> targets = new List<Transform>();
    void Start()
    {
        HideUpgrades();
        weapon.SetDamage(damage);
        InvokeRepeating("UpdateTargets", 1f, 1.0f / retargetRate);
    }

    private void UpdateTargets()
    {
        targets = new List<Transform>();
        foreach (var enemyCollider in Physics.OverlapSphere(transform.position, range, enemyLayerMask, QueryTriggerInteraction.UseGlobal))
        {
            var enemy = enemyCollider.GetComponentInParent<Enemy>();
            if (enemy)
            {
                targets.Add(enemy.transform);
            }
        }
    }
    void Update()
    {
        if (targets.Count == 0) return;

        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            weapon.Shoot(targets);
        }
    }

    public new bool Upgrade(int money)
    {
        if (!base.Upgrade(money))
            return false;

        damage = (int)(1.7f * damage);
        weapon.SetDamage(damage);
        fireRate *= 1.1f;
        range *= 1.2f;
        return true;
    }
}
