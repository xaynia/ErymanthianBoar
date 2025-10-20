using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI titleText;      // e.g., "Herc vs The Boar"
    public TextMeshProUGUI lastScoreText;  // shows last score or 0
    public TextMeshProUGUI highScoreText;  // shows best
    public TextMeshProUGUI controlsText;    // "SPACE — Jump | LEFT SHIFT — Sprint..."
    
    [Header("Scenes")]
    public string gameSceneName = "Scenes/GameScene";

    private void Start()
    {
        Time.timeScale = 1f;

        int last = PlayerPrefs.GetInt("lastScore", 0);
        int best = PlayerPrefs.GetInt("highScore", 0);
        
        var reason = ScoreManager.Instance 
            ? ScoreManager.Instance.nextMenuReason 
            : MenuReason.Title;

        switch (reason)
        {
            case MenuReason.Title:
                titleText.text    = "HERACLES & THE ERYMANTHIAN BOAR";
                controlsText.text = "SPACE — Jump\nLEFT SHIFT — Sprint";
                break;

            case MenuReason.GameOver:
                titleText.text    = "Game Over";
                controlsText.text = "SPACE — Jump | LEFT SHIFT — Sprint";
                break;

            case MenuReason.Victory:
                titleText.text    = "You Caught the Boar!";
                controlsText.text = "";
                break;
        }

        lastScoreText.text  = $"Last: {last}";
        highScoreText.text  = $"Best: {best}";
    }
    
    public void PlayGame()
    {
        ScoreManager.Instance?.ResetScore();
        
        // next time we return to the menu (if player quits early), default to Title
        if (ScoreManager.Instance) ScoreManager.Instance.nextMenuReason = MenuReason.Title;

        SceneManager.LoadScene(gameSceneName);
    }
}