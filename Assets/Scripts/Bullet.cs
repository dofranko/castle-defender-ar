using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int Damage { get; set; }
    public Rigidbody rb;
    [SerializeField] protected float speed;
    [SerializeField] protected float lifeTime;

    protected void Start()
    {

        Destroy(gameObject, lifeTime);
    }
    protected void FixedUpdate()
    {
        rb.velocity = transform.forward * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        var castle = other.GetComponentInParent<Castle>();
        if (castle)
        {
            castle.TakeDamage(Damage);
            Destroy(gameObject);
        }
    }
}
