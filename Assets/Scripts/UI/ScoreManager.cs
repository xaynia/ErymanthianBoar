using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI scoreText;

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
        UpdateUI();
    }

    public void Add(int amount = 1)
    {
        Score += amount;
        UpdateUI();
    }

    public void SaveLastRunAndHighScore(bool won)
    {
        PlayerPrefs.SetInt("lastScore", Score);
        PlayerPrefs.SetInt("lastResult", won ? 1 : 0);

        int best = PlayerPrefs.GetInt("highScore", 0);
        if (Score > best) PlayerPrefs.SetInt("highScore", Score);

        PlayerPrefs.Save();
    }

    private void UpdateUI()
    {
        if (scoreText) scoreText.text = Score.ToString();
    }
}