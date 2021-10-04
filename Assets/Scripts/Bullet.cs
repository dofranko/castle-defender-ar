using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
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
        var castle = other.GetComponent<Castle>();
        if (!castle) castle = other.GetComponentInParent<Castle>();
        if (castle)
        {
            castle.TakeDamage(Damage);
        }
        Destroy(gameObject);
    }
}
