using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    enum State
    {
        NotSpawned,
        Spawned
    }

    private State state = State.NotSpawned;
    public GameObject enemyPrefab;

    public void SpawnEnemies(Vector3 castleLocation)
    {
        if (state == State.Spawned)
            return;
        for (int i = 0; i < 5; i++)
        {
            Vector3 newLocation = new Vector3(
                castleLocation.x - Random.Range(2, 12) * (Random.Range(0, 1) * 2 - 1),
                castleLocation.y,
                castleLocation.z - Random.Range(2, 10) * (Random.Range(0, 1) * 2 - 1));
            var enemy = Instantiate(enemyPrefab, newLocation, new Quaternion());
            enemy.GetComponent<Enemy>().CastlePosition = castleLocation;
        }
        state = State.Spawned;

    }
    void Update()
    {
        if (state == State.Spawned)
        {
            var a = Object.FindObjectsOfType<Enemy>();
            if (a.Length == 0)
            {
                var b = Object.FindObjectOfType<CastleScript>();
                if (b)
                {
                    b.DisplayUpgrades();
                    state = State.NotSpawned;
                }
            }
        }
    }

}
