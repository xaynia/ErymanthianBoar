using UnityEngine;
using UnityEngine.UI;

public class BoarEnduranceBarUI : MonoBehaviour
{
    public Slider slider;

    private void Reset() { slider = GetComponent<Slider>(); }

    private void Update()
    {
        if (slider) slider.value = GameManager.Instance.GetBoarEndurance01();
    }
}