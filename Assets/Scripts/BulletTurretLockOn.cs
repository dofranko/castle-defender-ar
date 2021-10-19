using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTurretLockOn : BulletTurretExplosive
{
    public Transform Target { get; set; }
    protected new void FixedUpdate()
    {
        base.FixedUpdate();
        if (Target) transform.LookAt(Target); //TODO Math.LerpAngle
        /*
        Vector3 direction = Point - transform.position;
 Quaternion toRotation = Quaternion.FromToRotation(transform.forward, direction);
 transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, speed * Time.time);
        */
    }
    protected void OnTriggerEnter(Collider other)
    {
        Instantiate(explosionGameObject, transform.position, Quaternion.identity);
        foreach (var enemyCollider in Physics.OverlapSphere(transform.position, explosionRadius, enemyLayerMask, QueryTriggerInteraction.UseGlobal))
        {
            var enemy = enemyCollider.GetComponentInParent<Enemy>();
            if (enemy)
            {
                enemy.SpeedPercentage -= 0.15f;
                if (other == enemyCollider)
                {
                    enemy.TakeDamage(Damage);
                }
            }
        }
        Destroy(gameObject);
    }
}
