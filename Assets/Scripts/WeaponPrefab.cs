using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPrefab : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    private int damage = 0;
    public void Shoot()
    {
        var bulletInstance = Instantiate(bulletPrefab, transform.position, transform.rotation);
        var bullet = bulletInstance.GetComponent<Bullet>();
        if (bullet && damage > 0) bullet.Damage = damage;
    }

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }
}
