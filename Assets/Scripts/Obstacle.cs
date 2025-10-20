using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Tooltip("Multiply global scroll speed for this obstacle (1 = same as ground).")]
    public float speedMultiplier = 1f;

    private void Start()
    {
        Invoke(nameof(DestroyAfterTime), 8f);
    }

    private void Update()
    {
        float s = GameManager.Instance.ScrollSpeed * speedMultiplier;
        transform.Translate(Vector3.left * (s * Time.deltaTime));
    }

    private void DestroyAfterTime()
    {
        Destroy(gameObject);
    }
}