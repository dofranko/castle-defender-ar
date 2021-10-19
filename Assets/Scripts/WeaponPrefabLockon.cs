using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPrefabLockon : MonoBehaviour
{

    [SerializeField] protected Transform firePoint;
    [SerializeField] protected BulletTurretLockOn bulletPrefab;
    protected int damage = 1;
    public void SetDamage(int dmg)
    {
        damage = dmg;
    }
    public void Shoot(Transform target)
    {
        var bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        if (bullet)
        {
            bullet.Damage = damage;
            bullet.Target = target;
            return;
        }
    }
}
