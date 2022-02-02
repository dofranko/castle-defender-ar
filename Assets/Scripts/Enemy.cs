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

    [SerializeField] private float _fireSpeed;
    [SerializeField] private int healthMax;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private float _speed;
    [SerializeField] private float range;
    [SerializeField] private float accuracy;
    [SerializeField] private int money;
    private float _speedPercentage = 1.0f;
    public float SpeedPercentage
    {
        get { return _speedPercentage; }
        set
        {
            nextTimeToRemoveFrozingEffetct = Time.time + 1.2f;

            if (value <= 0.02f)
                _speedPercentage = 0.02f;
            else if (value > 1.0f)
            {
                _speedPercentage = 1.0f;
                nextTimeToRemoveFrozingEffetct = 0.0f;
            }
            else
                _speedPercentage = value;
        }
    }
    private float Speed
    {
        get { return _speed * SpeedPercentage; }
        set { _speed = value; }
    }
    private float FireSpeed
    {
        get { return _fireSpeed * SpeedPercentage; }
        set { _fireSpeed = value; }
    }

    private float nextTimeToRemoveFrozingEffetct = 0.0f;
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
                //transform.LookAt(CastlePosition);
                if (Vector3.Distance(transform.position, CastlePosition) >= 1.0f)
                {
                    transform.position += transform.forward * Speed * Time.deltaTime;
                    if (Vector3.Distance(transform.position, CastlePosition) <= 1.5f)
                    {
                        state = State.Shooting;
                        var anim = GetComponentInChildren<Animator>();
                        if (anim) anim.StopPlayback();
                        var castle = FindObjectOfType<Castle>();
                        if (castle) transform.LookAt(castle.gameObject.transform);
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
        if (nextTimeToRemoveFrozingEffetct != 0.0f && nextTimeToRemoveFrozingEffetct <= Time.time)
        {
            SpeedPercentage += 0.1f;
        }
    }
}
