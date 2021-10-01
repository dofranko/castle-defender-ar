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

    private CastleScript castleScript;
    private UpgradesSystemScript upgrades;

    void Start()
    {
        var placementScript = GetComponent<PlacementScript>();
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
        var castleLocation = GetCastle().transform.position;
        for (int i = 0; i < 2; i++)
        {
            Vector3 newLocation = new Vector3(
                castleLocation.x - Random.Range(2, 12) * (Random.Range(0, 1) * 2 - 1),
                castleLocation.y,
                castleLocation.z - Random.Range(2, 10) * (Random.Range(0, 1) * 2 - 1));
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
                GetCastle().HideUpgrades(false);
                SpawnEnemies();
            }

        }
    }

    private void OnEnemyDieEventHandler(object? sender, System.EventArgs e)
    {
        StartCoroutine(CheckInBetweenWaves());
    }

    private void OnCastleSpawnEventHandler(object? sender, System.EventArgs e)
    {
        SpawnEnemies();
    }

    private IEnumerator CheckInBetweenWaves()

    {
        yield return new WaitForSeconds(1.0f);
        GetCastle();
        if (state == State.Spawned)
        {
            var allEnemies = FindObjectsOfType<Enemy>();
            if (allEnemies.Length == 0)
            {
                castleScript.DisplayUpgrades();
                timeRemaining = 60;
                state = State.InBetweenWaves;
            }
        }
    }
    private void OnCastleHideUpgradesEventHandler(object? sender, System.EventArgs e)
    {
        this.timeRemaining = 0;
    }

    private CastleScript GetCastle()
    {
        if (!castleScript)
        {
            castleScript = FindObjectOfType<CastleScript>();
            castleScript.OnHideUpgrades += OnCastleHideUpgradesEventHandler;
        }
        return castleScript;
    }

    private UpgradesSystemScript GetTimerText()
    {
        if (!upgrades)
        {
            upgrades = castleScript.upgradesPanel.GetComponent<UpgradesSystemScript>();
        }
        return upgrades;
    }
}
