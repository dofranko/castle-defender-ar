using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTurretExplosive : Bullet
{

    [SerializeField] protected float explosionRadius;
    [SerializeField] protected LayerMask enemyLayerMask;
    [SerializeField] protected GameObject explosionGameObject;
    void OnTriggerEnter(Collider other)
    {
        Instantiate(explosionGameObject, transform.position, Quaternion.identity);
        foreach (var enemyCollider in Physics.OverlapSphere(transform.position, explosionRadius, enemyLayerMask, QueryTriggerInteraction.UseGlobal))
        {
            var enemy = enemyCollider.GetComponentInParent<Enemy>();
            if (enemy)
            {
                enemy.TakeDamage(Damage);

            }
        }
        Destroy(gameObject);
    }
}
