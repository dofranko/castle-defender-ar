using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTurretExplosive : Bullet
{

    [SerializeField] private float radius;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private GameObject explosionGameObject;
    void OnTriggerEnter(Collider other)
    {
        Instantiate(explosionGameObject, other.transform.position, Quaternion.identity);
        foreach (var enemyCollider in Physics.OverlapSphere(transform.position, radius, enemyLayerMask, QueryTriggerInteraction.UseGlobal))
        {
            var enemy = enemyCollider.GetComponent<Enemy>();
            if (!enemy) enemy = enemyCollider.GetComponentInParent<Enemy>();
            if (enemy)
            {
                enemy.TakeDamage(Damage);

            }
        }
        Destroy(gameObject);
    }
}
