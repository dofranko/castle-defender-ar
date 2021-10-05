using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    enum State
    {
        NotSpawned,
        Spawned,
        InBetweenWaves
    }
    private float timeRemaining = 0;
    private State state = State.NotSpawned;
    public GameObject enemyPrefab;

    private Castle castleScript;
    private UpgradesSystem upgrades;

    void Start()
    {
        var placementScript = GetComponent<Placement>();
        placementScript.OnCastleSpawn += OnCastleSpawnEventHandler;
    }

    private void SpawnEnemies()
    {
        StartCoroutine(SpawnEnemiesEnumerator());
    }
    private IEnumerator SpawnEnemiesEnumerator()
    {
        if (state == State.Spawned)
            yield return new WaitForSeconds(0.0f);
        yield return new WaitForSeconds(3.0f);
        var castleLocation = GetCastle()?.transform.position ?? new Vector3();
        for (int i = 0; i < 2; i++)
        {
            Vector3 newLocation = new Vector3(
                castleLocation.x - Random.Range(2.0f, 12.0f) * (Random.Range(0, 2) * 2 - 1),
                castleLocation.y,
                castleLocation.z - Random.Range(2.0f, 10.0f) * (Random.Range(0, 2) * 2 - 1));
            var enemy = Instantiate(enemyPrefab, newLocation, new Quaternion());
            var enemyEnemyComp = enemy.GetComponent<Enemy>();
            enemyEnemyComp.CastlePosition = castleLocation;
            enemyEnemyComp.OnDie += OnEnemyDieEventHandler;
        }
        state = State.Spawned;

    }

    void Update()
    {
        if (state == State.InBetweenWaves)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                float minutes = Mathf.FloorToInt(timeRemaining / 60);
                float seconds = Mathf.FloorToInt(timeRemaining % 60);
                var text = string.Format("{0:00}:{1:00}", minutes, seconds);
                GetTimerText().SetTimerText(text);
            }
            else
            {
                timeRemaining = 0;
                state = State.NotSpawned;
                GetCastle()?.HideUpgrades(false);
                SpawnEnemies();
            }

        }
    }

    private void OnEnemyDieEventHandler(object? sender, Enemy.EnemyDieEventArgs e)
    {
        GetCastle()?.AddMoney(e.Money);
        StartCoroutine(CheckInBetweenWaves(e));
    }

    private void OnCastleSpawnEventHandler(object? sender, System.EventArgs e)
    {
        if (state != State.InBetweenWaves) //TODO zrobić z tym porządek
            SpawnEnemies();
    }

    private IEnumerator CheckInBetweenWaves(Enemy.EnemyDieEventArgs e)

    {
        yield return new WaitForSeconds(1.0f);
        if (state == State.Spawned)
        {
            var allEnemies = FindObjectsOfType<Enemy>();
            if (allEnemies.Length == 0)
            {
                GetCastle()?.DisplayUpgrades();
                timeRemaining = 60;
                state = State.InBetweenWaves;
            }
        }
    }
    private void OnCastleHideUpgradesEventHandler(object? sender, System.EventArgs e)
    {
        this.timeRemaining = 0;
    }

    private Castle GetCastle()
    {
        if (!castleScript)
        {
            castleScript = FindObjectOfType<Castle>();
            if (castleScript)
            {
                castleScript.OnHideUpgrades += OnCastleHideUpgradesEventHandler;
            }
        }
        return castleScript;
    }

    private UpgradesSystem GetTimerText()
    {
        if (!upgrades)
        {
            upgrades = GetCastle()?.upgradesPanel.GetComponent<UpgradesSystem>();
        }
        return upgrades;
    }
}
