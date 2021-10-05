using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTurretBasic : MonoBehaviour
{

    public int Damage { get; set; }
    public Rigidbody rb;
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;

    // Update is called once per frame
    void Start()
    {

        Destroy(gameObject, lifeTime);
    }
    void FixedUpdate()
    {
        rb.velocity = transform.forward * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        var enemy = other.GetComponent<Enemy>();
        if (!enemy) enemy = other.GetComponentInParent<Enemy>();
        if (enemy)
        {
            enemy.TakeDamage(Damage);
            Destroy(gameObject);
        }
    }
}
