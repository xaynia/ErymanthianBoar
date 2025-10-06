using System;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public GameObject groundPrefab;
    
    public float speed;

    private bool hasSpawnedGround = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
    //     
    // }

    // Update is called once per frame
    private void Update()
    {
        if (Vector3.Distance(new Vector3(9f, -4.93876886f, 0), transform.position) < 0.2f && !hasSpawnedGround) 
        {
            Instantiate(groundPrefab, new Vector3(9f + 20, -4.93876886f, 0), Quaternion.identity);
            hasSpawnedGround = true;
        }
        else if(Vector3.Distance(new Vector3(-21f, -4.93876886f, 0), transform.position) < 0.2f)
        {
            Destroy(groundPrefab);    
        }
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }
}
