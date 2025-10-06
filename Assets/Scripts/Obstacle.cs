using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("DestroyAfterTime", 10);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }

    void DestroyAfterTime()
    {
        Destroy(gameObject);
    }
}
