using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float speed;
    public int damage;
    public Rigidbody rb;

    // Update is called once per frame
    void Start()
    {
        if (speed == 0)
        {
            speed = 0.002f;
        }
    }
    void FixedUpdate()
    {
        rb.velocity = transform.forward * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        var castle = other.GetComponent<CastleScript>();
        if (castle)
        {
            castle.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
