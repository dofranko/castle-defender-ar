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
    public Enemy enemyPrefab;
    private int waveNumber = 0;
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
        for (int i = 0; i < Random.Range(waveNumber + 2 + (int)(waveNumber * 1.6), waveNumber + 4 + (int)(waveNumber * 1.8)); i++)
        {
            Vector3 newLocation = new Vector3(
                castleLocation.x - Random.Range(4.0f, 5.0f) * (Random.Range(0, 2) * 2 - 1),
                castleLocation.y,
                castleLocation.z - Random.Range(4.0f, 5.0f) * (Random.Range(0, 2) * 2 - 1));
            var enemy = Instantiate(enemyPrefab, newLocation, new Quaternion());
            enemy.CastlePosition = castleLocation;
            enemy.OnDie += OnEnemyDieEventHandler;
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
                CloseShopsAndStartWave();
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
            CloseShopsAndStartWave();
    }

    private IEnumerator CheckInBetweenWaves(Enemy.EnemyDieEventArgs e)

    {
        yield return new WaitForSeconds(1.0f);
        if (state == State.Spawned)
        {
            var allEnemies = FindObjectsOfType<Enemy>();
            if (allEnemies.Length == 0 && state == State.Spawned) //double check 
            {
                StartShops();
                timeRemaining = 60;
            }
        }
    }
    private void StartShops()
    {
        state = State.InBetweenWaves;
        GetCastle()?.ShowUpgrades();
        foreach (var turret in FindObjectsOfType<UpgradableTurret>())
        {
            turret.ShowUpgrades();
        }
    }

    private void CloseShopsAndStartWave()
    {
        timeRemaining = 0;
        waveNumber++;
        state = State.NotSpawned;
        GetCastle()?.HideUpgrades();
        foreach (var turret in FindObjectsOfType<UpgradableTurret>())
        {
            turret.HideUpgrades();
        }
        SpawnEnemies();
    }
    private void OnSkipButtonPressedEventHandler(object? sender, System.EventArgs e)
    {
        CloseShopsAndStartWave();
    }

    private Castle GetCastle()
    {
        if (!castleScript)
        {
            castleScript = FindObjectOfType<Castle>();
            if (castleScript)
            {
                castleScript.upgradesPanel.OnSkipButtonPressed += OnSkipButtonPressedEventHandler;
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
