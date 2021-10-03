using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    private int health = 0;
    [SerializeField] private int smoothing;
    private void Update()
    {
        if (slider.value != health)
        {
            slider.value = Mathf.Lerp(slider.value, health, smoothing * Time.deltaTime);
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
    }
    public void SetInitHealth(int max)
    {
        slider.maxValue = max;
        health = max;

        gradient.Evaluate(slider.normalizedValue);
    }
    public void SetHealth(int health)
    {
        this.health = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void SetMaxHealth(int max)
    {
        slider.maxValue = max;
        gradient.Evaluate(slider.normalizedValue);
    }

    public int GetHealth()
    {
        return health;
    }
    public int GetMaxHealth()
    {
        return (int)slider.maxValue;
    }
}
