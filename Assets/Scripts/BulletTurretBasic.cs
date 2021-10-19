using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTurretBasic : Bullet
{
    void OnTriggerEnter(Collider other)
    {
        var enemy = other.GetComponentInParent<Enemy>();
        if (enemy)
        {
            enemy.TakeDamage(Damage);
        }
        Destroy(gameObject);
    }
}
