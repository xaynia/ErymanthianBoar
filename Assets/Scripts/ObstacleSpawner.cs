using System.Collections;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Set")]
    public GameObject[] obstaclePrefabs;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        // Small initial delay so the player sees the scene
        yield return new WaitForSeconds(0.8f);

        while (true)
        {
            // // Spawn one
            // Instantiate(ChooseRandomObstacle(), transform.position, Quaternion.identity);
            //
            // // Wait based on dynamic difficulty
            // float minT = GameManager.Instance.CurrentSpawnMin;
            // float maxT = GameManager.Instance.CurrentSpawnMax;
            // float wait = Random.Range(minT, maxT);
            // yield return new WaitForSeconds(wait);
            
            Instantiate(ChooseRandomObstacle(), transform.position, Quaternion.identity);

            float minT = GameManager.Instance.CurrentSpawnMin;
            float maxT = GameManager.Instance.CurrentSpawnMax;

            // // shrink wait when the world speeds up so it LOOKS busier/faster
            // float speedMul = GameManager.Instance.SpeedMul01; // 1..speedEndMultiplier
            // float wait = Random.Range(minT, maxT) / speedMul;
            //
            
            // compress gaps as world speed ramps
            float wait = Random.Range(minT, maxT) / GameManager.Instance.SpeedMul01;
            
            yield return new WaitForSeconds(wait);
        }
        // ReSharper disable once IteratorNeverReturns
    }

    private GameObject ChooseRandomObstacle()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return null;
        int idx = Random.Range(0, obstaclePrefabs.Length); // max is exclusive
        return obstaclePrefabs[idx];
    }
}