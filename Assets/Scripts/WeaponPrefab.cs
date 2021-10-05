using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPrefab : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    private int damage = 3;
    public void Shoot()
    {
        var bulletInstance = Instantiate(bulletPrefab, transform.position, transform.rotation);
        var bullet = bulletInstance.GetComponent<Bullet>();
        if (bullet)
        {
            bullet.Damage = damage;
            return;
        }
        var bullet1 = bulletInstance.GetComponent<BulletTurretBasic>();
        if (bullet1)
        {
            bullet1.Damage = damage;
            return;
        }
        var bullet2 = bulletInstance.GetComponent<BulletTurretExplosive>();
        if (bullet2)
        {
            bullet2.Damage = damage;
            return;
        }
    }

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }
}
