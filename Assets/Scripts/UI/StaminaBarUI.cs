using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
// {
//     public Player player;
//     public Slider slider;
//     
//     private void Reset() { slider = GetComponent<Slider>(); }
//     
//
//     // Update is called once per frame
//     void Update()
//     {
//         if (player && slider) slider.value = player.Stamina01;
//     }
// }

{
    public Player player;
    public Slider slider;
    [Header("FX")]
    public CanvasGroup canvasGroup;         // add a CanvasGroup to the bar root
    public float smoothSpeed = 10f;         // how fast the bar interpolates to target
    public float fadeSpeed = 6f;            // how fast it fades in/out
    public float stayVisibleAfterChange = 1.2f; // seconds to remain visible after value changes
    public Color fullColor = new Color(0.2f, 0.9f, 0.3f, 1f);
    public Color midColor  = new Color(1f,   0.8f, 0.2f, 1f);
    public Color lowColor  = new Color(1f,   0.2f, 0.2f, 1f);
    public float lowThreshold = 0.2f;

    private float displayValue;
    private float visibleTimer;
    private Image fillImage;

    void Awake()
    {
        if (!slider) slider = GetComponent<Slider>();
        fillImage = slider.fillRect ? slider.fillRect.GetComponent<Image>() : null;
        displayValue = slider.value;
        if (canvasGroup) canvasGroup.alpha = 0f;
        slider.interactable = false;
        slider.navigation = new Navigation { mode = Navigation.Mode.None };
    }

    void Update()
    {
        float target = (player ? player.Stamina01 : 0f);
        // smooth follow
        displayValue = Mathf.Lerp(displayValue, target, Time.deltaTime * smoothSpeed);
        slider.value = displayValue;

        // color feedback
        if (fillImage)
        {
            Color c = (displayValue < lowThreshold) ? lowColor :
                      (displayValue < 0.6f)        ? midColor  : fullColor;
            fillImage.color = c;
        }

        // auto show/hide when changing
        if (Mathf.Abs(displayValue - target) > 0.002f) visibleTimer = stayVisibleAfterChange;
        if (canvasGroup)
        {
            visibleTimer -= Time.deltaTime;
            float targetAlpha = (visibleTimer > 0f || displayValue < 0.999f) ? 1f : 0f; // hide when full and idle
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        }
    }
}