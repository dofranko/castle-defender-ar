using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public Image depleted;
    [SerializeField] private int smoothing;
    [SerializeField] private float rechargeRate;
    [SerializeField] private float rechargeDelay;
    private float shield = 30;
    private float nextTimeToRecharge = 0.0f;

    void Start()
    {
        shield = slider.maxValue;
        depleted.enabled = false;
    }
    void Update()
    {
        if (shield < slider.maxValue && nextTimeToRecharge <= Time.time)
        {
            depleted.enabled = false;
            shield += rechargeRate * Time.deltaTime;
            fill.color = gradient.Evaluate(0.5f);
            if (shield >= slider.maxValue)
            {
                shield = (int)slider.maxValue;
                nextTimeToRecharge = 0.0f;
                fill.color = gradient.Evaluate(1.0f);
            }
        }
        if (slider.value != shield)
        {
            slider.value = Mathf.Lerp(slider.value, shield, smoothing * Time.deltaTime);
        }
    }

    public void SetInitShield(int max)
    {
        slider.maxValue = max;
        shield = max;
    }
    public void SetShield(int shield)
    {
        this.shield = shield;
    }
    public int TakeDamage(int dmg)
    {
        if (shield - dmg <= 0)
        {
            dmg -= (int)shield;
            shield = 0;
            fill.color = gradient.Evaluate(0.0f);
            depleted.enabled = true;
        }
        else
        {
            shield -= dmg;
            dmg = 0;
            fill.color = gradient.Evaluate(1.0f);
        }
        nextTimeToRecharge = Time.time + rechargeDelay;
        return dmg;
    }
    public void SetMaxShield(int max)
    {
        slider.maxValue = max;
    }

    public int GetShield()
    {
        return (int)shield;
    }
    public int GetMaxShield()
    {
        return (int)slider.maxValue;
    }
}
