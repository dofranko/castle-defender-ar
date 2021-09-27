using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float speed;
    public int damage = 2;
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
        Debug.Log("hit object");
        CastleScript castle = other.GetComponent<CastleScript>();
        if (castle)
        {
            Debug.Log("it s castle");
            castle.TakeDamage(damage);
        }
        Debug.Log("destroing bullet");
        Destroy(gameObject);
    }
}
