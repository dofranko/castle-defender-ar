using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    enum State
    {
        None,
        Spawned,
        Moving,
        StoppedMoving,
        Dead,
        WaitingForCastle
    }

    public Vector3 CastlePosition { get; set; }
    public HealthBar healthBar;

    private readonly CharacterStatistics stats = new CharacterStatistics(20, 3, 2, 0.3f, 1.5f, 0.85f);
    private State state = State.None;

    void Awake()
    {
    }
    void Start()
    {
        healthBar.SetMaxHealth(stats.HealthMax);
        Spawn();
    }
    public void Spawn()
    {
        state = State.Spawned;
        Move();
    }

    private void Move()
    {
        state = State.Moving;
    }
    public void SpawnRandom()
    {

    }

    public void Die()
    {

    }
    public void TakeDamage(int damage)
    {
        int health = healthBar.GetHealth();
        if (damage - stats.Defense < 1) damage = 1;
        health -= damage;
        if (health < 0) health = 0;
        healthBar.SetHealth(health);
        Die();
    }
    void Update()
    {

        if (state == State.Moving)
        {
            transform.LookAt(CastlePosition);
            if (Vector3.Distance(transform.position, CastlePosition) >= 1.0f)
            {

                transform.position += transform.forward * stats.Speed * Time.deltaTime;



                if (Vector3.Distance(transform.position, CastlePosition) <= 1.5f)
                {
                    //Here Call any function U want Like Shoot at here or something
                }

            }
        }
    }
}
