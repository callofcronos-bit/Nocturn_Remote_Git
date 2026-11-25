using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[] enemyPrefabs;   // NoxMelee, NoxRange
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] float spawnInterval = 3;
    [SerializeField] int maxAlive = 6;

    float timer;
    int spawned;

    void Start() => EnemyBase.ResetCount();

    void Update()
    {
        if (spawned >= GameManager.KillTarget) return;
        if (EnemyBase.AliveCount >= maxAlive) return;
        if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0;
            var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            var point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(prefab, point.position, Quaternion.identity);
            spawned++;
        }
    }
}
