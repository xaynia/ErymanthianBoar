using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public MenuReason nextMenuReason = MenuReason.Title;
        
    public int Score { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        ResetScore();
    }

    public void ResetScore()
    {
        Score = 0;
    }

    public void Add(int amount = 1)
    {
        Score += amount;
    }

    public void SaveLastRunAndHighScore(bool won)
    {
        PlayerPrefs.SetInt("lastScore", Score);
        PlayerPrefs.SetInt("lastResult", won ? 1 : 0);

        int best = PlayerPrefs.GetInt("highScore", 0);
        if (Score > best) PlayerPrefs.SetInt("highScore", Score);

        PlayerPrefs.Save();
    }
}