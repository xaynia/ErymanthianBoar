using UnityEngine;

public class Ground : MonoBehaviour
// {
//     public GameObject groundPrefab;
//     
//     public float speed;
//
//     private bool hasSpawnedGround = false;
//     
//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     // void Start()
//     // {
//     //     
//     // }
//
//     // Update is called once per frame
//     private void Update()
//     {
//         if (Vector3.Distance(new Vector3(9f, -4.93876886f, 0), transform.position) < 0.2f && !hasSpawnedGround) 
//         {
//             Instantiate(groundPrefab, new Vector3(9f + 20, -4.93876886f, 0), Quaternion.identity);
//             hasSpawnedGround = true;
//         }
//         else if(Vector3.Distance(new Vector3(-21f, -4.93876886f, 0), transform.position) < 0.2f)
//         {
//             Destroy(groundPrefab);    
//         }
//         transform.Translate(Vector3.left * speed * Time.deltaTime);
//     }
// }

{
    [Header("Setup")]
    public GameObject groundPrefab;
    [Tooltip("World width of a single ground tile.")]
    public float segmentWidth = 20f;

    [Header("Recycle Landmarks")]
    public float spawnNextAtX = 9f;    // when this tile reaches this X, spawn the next one to the right
    public float destroyAtX   = -21f;  // when this tile passes this X, destroy it

    private bool spawnedNext;

    private void Update()
    {
        // Move with global scroll speed
        transform.Translate(Vector3.left * (GameManager.Instance.ScrollSpeed * Time.deltaTime));

        // Spawn next tile once when we pass the spawn landmark
        if (!spawnedNext && transform.position.x <= spawnNextAtX)
        {
            Vector3 spawnPos = new Vector3(transform.position.x + segmentWidth, transform.position.y, transform.position.z);
            Instantiate(groundPrefab, spawnPos, Quaternion.identity);
            spawnedNext = true;
        }

        // Destroy this tile when it goes offscreen left
        if (transform.position.x <= destroyAtX)
        {
            Destroy(gameObject); // destroy THIS instance (not the prefab reference)
        }
    }
}
