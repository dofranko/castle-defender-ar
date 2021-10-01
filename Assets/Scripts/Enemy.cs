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
        Shooting
    }

    public Vector3 CastlePosition { get; set; } = new Vector3();
    public HealthBar healthBar;
    public WeaponPrefabScript weapon;

    public event System.EventHandler OnDie;

    private readonly CharacterStatistics stats = new CharacterStatistics(20, 3, 2, 0.3f, 1.5f, 0.85f);
    private State state = State.None;
    private float FireSpeed = 0.5f;
    private float nextTimeToFire = 0.0f;

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
        OnDie?.Invoke(this, System.EventArgs.Empty);
        Destroy(gameObject);
    }
    public void TakeDamage(int damage)
    {
        int health = healthBar.GetHealth();
        damage -= stats.Defense;
        if (damage <= 0) damage = 1;
        health -= damage;
        if (health <= 0) Die();
        healthBar.SetHealth(health);
    }
    void Update()
    {
        switch (state)
        {
            case State.Moving:
                transform.LookAt(CastlePosition);
                if (Vector3.Distance(transform.position, CastlePosition) >= 1.0f)
                {
                    transform.position += transform.forward * stats.Speed * Time.deltaTime;
                    if (Vector3.Distance(transform.position, CastlePosition) <= 1.5f)
                    {
                        state = State.Shooting;
                    }
                }
                break;
            case State.Shooting:
                if (Time.time >= nextTimeToFire)
                {
                    nextTimeToFire = Time.time + 1f / FireSpeed;
                    weapon.Shoot();
                }
                break;
        }
    }
}
