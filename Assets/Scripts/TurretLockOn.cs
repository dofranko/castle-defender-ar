using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretLockOn : UpgradableTurret
{
    protected Transform target;
    [SerializeField] protected float fireRate;
    [SerializeField] protected int damage;
    [SerializeField] protected float range;
    [SerializeField] protected float explosionRadius;
    [SerializeField] protected LayerMask enemyLayerMask;
    [SerializeField] private WeaponPrefabLockon weapon;
    protected float nextTimeToFire = 0.0f;

    void Start()
    {
        HideUpgrades();
        weapon.SetDamage(damage);
        InvokeRepeating("UpdateTarget", 1f, 1.0f / fireRate);

    }

    void UpdateTarget()
    {
        if (target) return;
        foreach (var enemyCollider in Physics.OverlapSphere(transform.position, range, enemyLayerMask, QueryTriggerInteraction.UseGlobal))
        {
            var enemy = enemyCollider.GetComponentInParent<Enemy>();
            if (enemy)
            {
                target = enemy.transform;
                break;
            }
        }
    }
    void Update()
    {
        if (!target) return;

        CannonLookAt(target);
        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            weapon.Shoot(target);
        }

    }

    public new bool Upgrade(int money)
    {
        if (!base.Upgrade(money))
            return false;

        damage = (int)(1.2f * damage);
        weapon.SetDamage(damage);
        fireRate *= 1.3f;
        range *= 1.4f;
        explosionRadius *= 1.3f;
        return true;
    }
}
