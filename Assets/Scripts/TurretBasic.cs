using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBasic : MonoBehaviour
{
    // Start is called before the first frame update

    private Transform target;
    [SerializeField] private float fireRate;
    [SerializeField] private int damage;
    [SerializeField] private float range;
    [SerializeField] private LayerMask enemyLayerMask;
    void Start()
    {
        InvokeRepeating("UpdateTarget", 1f, 1.0f / fireRate);
    }

    private void UpdateTarget()
    {
        if (target) return;
        foreach (var enemyCollider in Physics.OverlapSphere(transform.position, range, enemyLayerMask, QueryTriggerInteraction.Ignore))
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
    // Update is called once per frame
    void Update()
    {
        if (!target) return;
        transform.LookAt(target);
    }
}
