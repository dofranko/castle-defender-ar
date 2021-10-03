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
    public class EnemyDieEventArgs : System.EventArgs
    {
        public int Money { get; set; }
    }
    public Vector3 CastlePosition { get; set; } = new Vector3();
    public HealthBar healthBar;
    public WeaponPrefab weapon;

    public event System.EventHandler<EnemyDieEventArgs> OnDie;

    [SerializeField] private float fireSpeed;
    [SerializeField] private int healthMax;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private float speed;
    [SerializeField] private float range;
    [SerializeField] private float accuracy;
    [SerializeField] private int money;

    private State state = State.None;
    private float nextTimeToFire = 0.0f;

    void Awake()
    {
    }
    void Start()
    {
        healthBar.SetInitHealth(healthMax);
        weapon.SetDamage(attack);
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
        OnDie?.Invoke(this, new EnemyDieEventArgs() { Money = this.money });
        Destroy(gameObject);
        //TODO on die give money
    }
    public void TakeDamage(int damage)
    {
        int health = healthBar.GetHealth();
        damage -= defense;
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
                    transform.position += transform.forward * speed * Time.deltaTime;
                    if (Vector3.Distance(transform.position, CastlePosition) <= 1.5f)
                    {
                        state = State.Shooting;
                    }
                }
                break;
            case State.Shooting:
                if (Time.time >= nextTimeToFire)
                {
                    nextTimeToFire = Time.time + 1f / fireSpeed;
                    weapon.Shoot();
                }
                break;
        }
    }
}
