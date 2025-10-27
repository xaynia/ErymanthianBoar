using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI titleText;      // e.g., "Herc vs The Boar"
    public TextMeshProUGUI lastScoreText;  // shows last score or 0
    public TextMeshProUGUI highScoreText;  // shows best
    public TextMeshProUGUI controlsText;    // "SPACE: Jump | LEFT SHIFT: Sprint..."
    
    [Header("Scenes")]
    public string gameSceneName = "Scenes/GameScene";

    private void Start()
    {
        Time.timeScale = 1f;
        
        var reason = ScoreManager.Instance 
            ? ScoreManager.Instance.nextMenuReason 
            : MenuReason.Title;

        switch (reason)
        {
            case MenuReason.Title:
                titleText.text    = "HERACLES & THE ERYMANTHIAN BOAR";
                controlsText.text = "SPACE: Jump\nLEFT SHIFT/MOUSE: Sprint";
                break;

            case MenuReason.GameOver:
                titleText.text    = "Game Over";
                controlsText.text = "SPACE: Jump\n LEFT SHIFT/MOUSE: Sprint";
                break;

            case MenuReason.Victory:
                titleText.text    = "You Caught the Boar!";
                controlsText.text = "";
                break;
        }

        int lastMs = PlayerPrefs.GetInt("lastTimeMs", -1);
        int bestMs = PlayerPrefs.GetInt("bestTimeMs", -1);
        
        string F(int ms){
            if (ms < 0) return "--:--.--";
            int t = ms; int cs = (t/10)%100; t/=1000;
            int s = t%60; int m = t/60;
            return $"{m:00}:{s:00}.{cs:00}";
        }

        lastScoreText.text = $"Last: {F(lastMs)}";
        highScoreText.text = $"Best: {F(bestMs)}";

    }
    
    public void PlayGame()
    {
        ScoreManager.Instance?.StartRun();
        
        // next time we return to the menu (if player quits early), default to Title
        if (ScoreManager.Instance) ScoreManager.Instance.nextMenuReason = MenuReason.Title;

        SceneManager.LoadScene(gameSceneName);
    }
}