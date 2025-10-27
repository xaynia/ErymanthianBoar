using UnityEngine;

public class ScoreZone : MonoBehaviour
{
    [Tooltip("If true, this trigger destroys itself after awarding score once.")]
    private bool destroyAfterScore = false;

    private bool consumed;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (consumed) return;
        if (!other.CompareTag("Player")) return;

        consumed = true;

        if (destroyAfterScore) Destroy(gameObject);
    }
}