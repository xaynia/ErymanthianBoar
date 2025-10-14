using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    public TextMeshProUGUI titleText;      // e.g., "Herc vs The Boar"
    public TextMeshProUGUI lastScoreText;  // shows last score or 0
    public TextMeshProUGUI highScoreText;  // shows best
    public string gameSceneName = "GameScene";

    private void Start()
    {
        Time.timeScale = 1f;

        int last = PlayerPrefs.GetInt("lastScore", 0);
        int best = PlayerPrefs.GetInt("highScore", 0);
        int result = PlayerPrefs.GetInt("lastResult", 0); // 1=win, 0=lose

        if (result == 1) titleText.text = "You Caught the Boar!";
        else             titleText.text = "Game Over";

        lastScoreText.text = $"Last: {last}";
        highScoreText.text = $"Best: {best}";
    }

    public void PlayGame()
    {
        
        Debug.Log("Play Game");
        SceneManager.LoadScene("Scenes/GameScene");
    }
}