using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPrefabMulti : MonoBehaviour
{
    public GameObject rayPrefab;
    private int damage = 3;
    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public void Shoot(List<Transform> targets)
    {
        foreach (var target in targets)
        {
            if (!target) continue;
            var electGameObject = Instantiate(rayPrefab, gameObject.transform.position, Quaternion.identity);
            var caster = electGameObject.GetComponent<ParticleCaster>();
            if (caster)
            {
                caster.Cast(gameObject.transform.position, target.transform.position);
                var enemy = target.GetComponent<Enemy>();
                {
                    if (enemy) enemy.TakeDamage(damage);
                }
            }
        }
    }
}
