using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPrefab : MonoBehaviour
{
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected Bullet bulletPrefab;
    protected int damage = 1;
    public void SetDamage(int dmg)
    {
        damage = dmg;
    }
    public void Shoot()
    {
        var bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        if (bullet) bullet.Damage = damage;
    }

}
