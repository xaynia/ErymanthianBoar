using UnityEngine;
using TMPro;

public class ScoreHUD : MonoBehaviour

// COUNTDOWN LOGIC

{
    public TextMeshProUGUI text;

    void Reset(){ text = GetComponent<TextMeshProUGUI>(); }

    void LateUpdate()
    {
        if (!ScoreManager.Instance || !text) return;
        float t = ScoreManager.Instance.TimeRemaining;
        int cs = Mathf.FloorToInt((t * 100f) % 100f);   // centiseconds
        int s  = Mathf.FloorToInt(t) % 60;
        int m  = Mathf.FloorToInt(t / 60f);
        text.text = $"{m:00}:{s:00}.{cs:00}";
    }
}