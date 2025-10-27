using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    // NEW TIMER BASED SCORE LOGIC
    [Header("Countdown (seconds)")]
    public float countdownStart = 60f;

    public float TimeRemaining { get; private set; }   // live timer
    public float TimeElapsed   { get; private set; }   // for “best time”
    public bool Running { get; private set; }

    public MenuReason nextMenuReason = MenuReason.Title;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        ResetStats();
    }

    public void ResetStats()
    {
        TimeRemaining = countdownStart;
        TimeElapsed   = 0f;
        Running       = false;
    }

    public void StartRun()  { ResetStats(); Running = true; }
    public void StopRun()   { Running = false; }

    void Update()
    {
        if (!Running) return;

        TimeElapsed   += Time.deltaTime;
        TimeRemaining  = Mathf.Max(0f, TimeRemaining - Time.deltaTime);

        if (TimeRemaining <= 0f)
        {
            Running = false;
            GameManager.Instance?.OnPlayerHitObstacle();   // treat as lose
        }
    }

    // Save times for the menu
    public void SaveLastRunAndHighScore(bool won)
    {
        // save last time (ms) for consistent formatting
        int lastMs = Mathf.RoundToInt(TimeElapsed * 1000f);
        PlayerPrefs.SetInt("lastTimeMs", lastMs);
        PlayerPrefs.SetInt("lastResult", won ? 1 : 0);

        // best time: LOWER is better (only update on win)
        int bestMs = PlayerPrefs.GetInt("bestTimeMs", int.MaxValue);
        if (won && lastMs < bestMs) PlayerPrefs.SetInt("bestTimeMs", lastMs);

        PlayerPrefs.Save();
    }
}