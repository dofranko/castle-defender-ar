using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretLockOn : MonoBehaviour
{
    protected Transform target;
    [SerializeField] protected float fireRate;
    [SerializeField] protected int damage;
    [SerializeField] protected float range;
    [SerializeField] protected LayerMask enemyLayerMask;
    [SerializeField] private WeaponPrefabLockon weapon;
    protected float nextTimeToFire = 0.0f;

    void Start()
    {
        weapon.SetDamage(damage);
        InvokeRepeating("UpdateTarget", 1f, 1.0f / fireRate);

    }

    void UpdateTarget()
    {
        if (target) return;
        foreach (var enemyCollider in Physics.OverlapSphere(transform.position, range, enemyLayerMask, QueryTriggerInteraction.UseGlobal))
        {
            var enemy = enemyCollider.GetComponent<Enemy>();
            if (!enemy) enemy = enemyCollider.transform.parent.GetComponent<Enemy>();
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

        transform.LookAt(target);
        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            weapon.Shoot(target);
        }

    }
}
