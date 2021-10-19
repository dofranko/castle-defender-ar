using UnityEngine;
using System.Collections;

public class player_attack : MonoBehaviour
{
    private Animator anim; 

    // Use this for initialization
    void Awake()
    {
        // Set up references.
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        // Attack
        if (Input.GetButtonDown("Attack_1"))
        {
            anim.SetTrigger("Attack_1");
            return;
        }

        // dead
        if (Input.GetButtonDown("Dead"))
        {
            anim.SetTrigger("Dead");
            return;
        }

        // damage
        if (Input.GetButtonDown("Damage"))
        {
            anim.SetTrigger("Damage");
            return;
        }

        // burst
        if (Input.GetButtonDown("Burst"))
        {
            anim.SetTrigger("Burst");
            return;
        }

        // defense
        if (Input.GetButtonDown("Defense"))
        {
            anim.SetTrigger("Defense");
            return;
        }
    }
}
