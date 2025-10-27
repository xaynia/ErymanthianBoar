using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Scenes")]
    public string menuSceneName = "Scenes/MenuScene";
    public string winScene = "Scenes/WinScene";

    [Header("Scroll / Speed")]
    [Tooltip("Base world scroll speed (units/sec).")]
    public float baseScrollSpeed = 5f;
    [Tooltip("Extra speed while sprinting (multiplier). 1.25 = 25% faster.")]
    public float sprintSpeedMultiplier = 1.25f;

    [Header("Chase (Distance)")]
    [Range(0f, 1f)] public float distance01 = 0.1f; // start somewhat behind
    public float sprintGainPerSecond = 0.30f;
    public float passiveLossPerSecond = 0.15f;
    public float knockbackOnHit = 0.25f;     // how much distance you lose on collision
    public float winThreshold = 1.0f;
    public float loseThreshold = 0.0f;

    [Header("Boar Endurance (optional)")]
    public bool useBoarEndurance = false;
    public float boarEnduranceMax = 100f;
    public float boarBaseDrainPerSecond = 0.5f;
    public float boarPressureDrainPerSecond = 2.5f;
    [Range(0f, 1f)] public float pressureThreshold = 0.7f;
    private float boarEndurance;

    [Header("Difficulty Ramp (Spawner)")]
    public float rampDurationSeconds = 90f; // time to reach max difficulty
    public float spawnMinStart = 1.6f;
    public float spawnMinEnd = 0.6f;
    public float spawnMaxStart = 2.4f;
    public float spawnMaxEnd = 1.0f;
    
    [Header("Speed Ramp (world feel)")]
    public float speedRampDuration = 90f;   // seconds to reach full speed
    public float speedEndMultiplier = 1.50f; // world goes +30% by the end

    
    
    [Header("Difficulty curve (0..1)")]
    public AnimationCurve difficultyCurve = AnimationCurve.EaseInOut(0,0,1,1);

    
    [Header("On-Hit Policy")]
    public bool instantLoseOnHit = true;  // set true for game over on hit


    private bool isSprinting;
    private float elapsed;
    private bool isLoading;
    
    // t rex changes start
    // [Header("T-Rex Steps (optional)")]
    // public int[] speedScoreSteps   = { 10, 20, 35, 55, 80 };
    // public float[] speedStepMults  = { 1.15f, 1.3f, 1.45f, 1.6f, 1.8f };
    //
    // float stepMul = 1f;
    //
    // void OnEnable()  { ScoreManager.ScoreChanged += OnScoreChanged; }
    // void OnDisable() { ScoreManager.ScoreChanged -= OnScoreChanged; }
    // void OnScoreChanged(int s)
    // {
    //     stepMul = 1f;
    //     for (int i = 0; i < speedScoreSteps.Length; i++)
    //         if (s >= speedScoreSteps[i]) stepMul = speedStepMults[i];
    // }
    
    // HIDE EVEN IF YOU ENABLE T REX
    // public float ScrollSpeed =>
    //     baseScrollSpeed * stepMul * SpeedMul01 * (isSprinting ? sprintSpeedMultiplier : 1f);
    // t rex changes end
    
    // Normalized difficulty based on elapsed time and your curve
    public float Difficulty01 => 
        difficultyCurve.Evaluate(Mathf.Clamp01(elapsed / rampDurationSeconds));
    
    // World-speed multiplier that grows over speedRampDuration
    public float SpeedMul01   => 
        Mathf.Lerp(1f, speedEndMultiplier, Mathf.Clamp01(elapsed / speedRampDuration));


    public float ScrollSpeed
    {
        get
        {
            // float sprintMul = isSprinting ? sprintSpeedMultiplier : 1f;
            // return baseScrollSpeed * sprintMul;
            float worldMul = SpeedMul01;
            float sprintMul = isSprinting ? sprintSpeedMultiplier : 1f;
            return baseScrollSpeed * worldMul * sprintMul;
            
        }
    }
    
    public void OnPlayerHitObstacle()
    {
        if (instantLoseOnHit) OnLose();
        else ApplyKnockback();
    }

    public float CurrentSpawnMin
    {
        get
        {
            // float t = Mathf.Clamp01(elapsed / rampDurationSeconds);
            // return Mathf.Lerp(spawnMinStart, spawnMinEnd, t);
            float t = Difficulty01;
            return Mathf.Lerp(spawnMinStart, spawnMinEnd, t);
        }
    }

    public float CurrentSpawnMax
    {
        get
        {
            // float t = Mathf.Clamp01(elapsed / rampDurationSeconds);
            // return Mathf.Lerp(spawnMaxStart, spawnMaxEnd, t);
            float t = Difficulty01;
            return Mathf.Lerp(spawnMaxStart, spawnMaxEnd, t);
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        // DontDestroyOnLoad(gameObject);

        boarEndurance = boarEnduranceMax;
    }
    //
    // private bool IsGameplay => SceneManager.GetActiveScene().name == gameplaySceneName;
    
    private void Update()
    {
        // if (!IsGameplay) return;
        
        elapsed += Time.deltaTime;

        // Distance meter: lose a bit over time, gain while sprinting
        float delta = (isSprinting ? sprintGainPerSecond : -passiveLossPerSecond) * Time.deltaTime;
        distance01 = Mathf.Clamp01(distance01 + delta);

        // Optional boar endurance timeline
        if (useBoarEndurance)
        {
            float drain = boarBaseDrainPerSecond * Time.deltaTime;
            if (distance01 >= pressureThreshold) drain += boarPressureDrainPerSecond * Time.deltaTime;
            boarEndurance = Mathf.Max(0f, boarEndurance - drain);
        }

        // Win / Lose checks
        if (distance01 >= winThreshold || (useBoarEndurance && boarEndurance <= 0f))
        {
            OnWin();
        }
        else if (distance01 <= loseThreshold)
        {
            OnLose();
        }
    }

    public void SetSprinting(bool sprinting) => isSprinting = sprinting;

    public void ApplyKnockback()
    {
        distance01 = Mathf.Max(loseThreshold, distance01 - knockbackOnHit);
    }

    // Called by UI bars
    public float GetDistance01() => Mathf.Clamp01(distance01);
    public float GetBoarEndurance01() => useBoarEndurance ? (boarEndurance / boarEnduranceMax) : 1f;

    private void OnWin()
    {
        if (isLoading) return;
        isLoading = true;
        
        ScoreManager.Instance?.StopRun();
        ScoreManager.Instance?.SaveLastRunAndHighScore(true);
        
        if (ScoreManager.Instance) ScoreManager.Instance.nextMenuReason = MenuReason.Victory;
        
        SceneManager.LoadScene(winScene);
    }

    private void OnLose()
    {
        if (isLoading) return;
        isLoading = true;
        
        ScoreManager.Instance?.StopRun();
        ScoreManager.Instance?.SaveLastRunAndHighScore(false);
        
        if (ScoreManager.Instance) ScoreManager.Instance.nextMenuReason = MenuReason.GameOver;
        
        SceneManager.LoadScene(menuSceneName);
    }
}