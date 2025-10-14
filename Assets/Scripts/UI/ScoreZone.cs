using UnityEngine;

public class ScoreZone : MonoBehaviour
{
    [Tooltip("If true, this trigger destroys itself after awarding score once.")]
    public bool destroyAfterScore = true;

    private bool consumed;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (consumed) return;
        if (!other.CompareTag("Player")) return;

        consumed = true;
        ScoreManager.Instance?.Add(1);

        if (destroyAfterScore) Destroy(gameObject);
    }
}