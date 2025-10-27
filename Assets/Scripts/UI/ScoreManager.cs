using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    // NEW TIMER BASED SCORE LOGIC
    [Header("Countdown (seconds)")]
    public float countdownStart = 60f;
    
    [Header("Scene Names")]
    public string menuScene = "MenuScene";
    public string gameScene = "GameScene";
    public string winScene  = "WinScene";
    
    public float TimeRemaining { get; private set; }   // live timer
    public float TimeElapsed   { get; private set; }   // for “best time”
    public bool Running { get; private set; }

    public MenuReason nextMenuReason = MenuReason.Title;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        ResetStats();
    }

    private void OnEnable()  { SceneManager.sceneLoaded += OnSceneLoaded; }
    private void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    private void OnSceneLoaded(Scene s, LoadSceneMode _)
    {
        string n = s.name;

        if (n == gameScene)
        {
            // Auto start the run whenever GameScene is entered.
            StartRun();
        }
        else if (n == menuScene || n == winScene)
        {
            // Ensure timer is not running in Menu or Win scenes.
            StopRun(false); // don't reset stats so the menu can read the last result
        }
    }

    public void ResetStats()
    {
        TimeRemaining = countdownStart;
        TimeElapsed   = 0f;
        Running       = false;
    }
    
    public void StartRun()
    {
        ResetStats();
        Running = true;
    }
    
    public void StopRun(bool reset = false)
    {
        Running = false;
        if (reset) ResetStats();
    }

    private void Update()
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
        if (won && lastMs > 0)
        {
            int bestMs = PlayerPrefs.HasKey("bestTimeMs")
                ? PlayerPrefs.GetInt("bestTimeMs")
                : int.MaxValue;

            if (lastMs < bestMs)
                PlayerPrefs.SetInt("bestTimeMs", lastMs);
        }

        PlayerPrefs.Save();
    }
}