using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    public Player player;
    public Slider slider;
    
    private void Reset() { slider = GetComponent<Slider>(); }
    

    // Update is called once per frame
    void Update()
    {
        if (player && slider) slider.value = player.Stamina01;
    }
}
