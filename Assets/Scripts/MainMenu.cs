using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI TimeText;
    public int playerRotate = -300;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        var timeToDisplay = PlayerPrefs.GetFloat("highscore");

        var t0 = (int)timeToDisplay;
        var m = t0 / 60;
        var s = (t0 - m * 60);
        var ms = (int)((timeToDisplay - t0) * 100);
        TimeText.text = $"{m:00}:{s:00}:{ms:00}";

        gameObject.transform.Rotate(0, 0,playerRotate * Time.deltaTime);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("obstacle")) 
        {
            collision.GetComponent<SpriteRenderer>().color = gameObject.GetComponent<SpriteRenderer>().color;
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);

    }

}
