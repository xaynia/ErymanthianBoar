using UnityEngine;
using TMPro;

public class ScoreHUD : MonoBehaviour
{
    public TextMeshProUGUI text;
    public string format = "{0}";

    void Reset()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void LateUpdate()
    {
        int s = ScoreManager.Instance ? ScoreManager.Instance.Score : 0;
        if (text) text.text = string.Format(format, s);
    }
}