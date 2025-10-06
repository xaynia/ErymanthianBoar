using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject cactusA;
    // ...
    
    public float minTimeBetweenSpawns;
    public float maxTimeBetweenSpawns;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnObstacle();
    }

    // Update is called once per frame
    void SpawnObstacle()
    {
        Instantiate(ChooseRandomObstacle(), transform.position, Quaternion.identity);
        Invoke("SpawnObstacle", Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns));
    }

    GameObject ChooseRandomObstacle()
    {
        int random = Random.Range(0, 1);
        switch (random) 
        {
            case 0: return cactusA;
            // ....
        }
        return cactusA;
    }
}
