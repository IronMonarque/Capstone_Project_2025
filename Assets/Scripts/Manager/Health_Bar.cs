using UnityEngine;
using UnityEngine.UI;

public class Health_Bar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    void Awake()
    {
        if (!slider) slider = GetComponent<Slider>() ?? GetComponentInChildren<Slider>();
        if (!fill && slider && slider.fillRect) fill = slider.fillRect.GetComponent<Image>();
        if (gradient == null) gradient = new Gradient(); // fallback to avoid null
    }
    public void SetMaxHealth(int health)
    {
        if (!slider) { Debug.LogError("Health_Bar: Slider not assigned."); return; }

        slider.maxValue = health;
        slider.value = health;

        if (fill) fill.color = gradient.Evaluate(1f);
    }
    public void SetHealth(int health)
    {
        if (!slider) { Debug.LogError("Health_Bar: Slider not assigned."); return; }
        slider.value = health;

        if (fill) fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
