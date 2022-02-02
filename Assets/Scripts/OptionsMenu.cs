using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Toggle shadowsToggle;
    [SerializeField] private Toggle dynamicToggle;
    [SerializeField] private Toggle planesToggle;

    private void Start()
    {
        if (PlayerPrefs.HasKey("shadows"))
        {
            shadowsToggle.isOn = PlayerPrefs.GetInt("shadows") == 1;
        }
        if (PlayerPrefs.HasKey("dynamic_light"))
        {
            dynamicToggle.isOn = PlayerPrefs.GetInt("dynamic_light") == 1;
        }
        if (PlayerPrefs.HasKey("visualize_planes"))
        {
            planesToggle.isOn = PlayerPrefs.GetInt("visualize_planes") == 1;
        }
    }
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetShadows(bool isShadow)
    {
        PlayerPrefs.SetInt("shadows", isShadow ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetDynamicLight(bool isDynamic)
    {
        PlayerPrefs.SetInt("dynamic_light", isDynamic ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetVisualizePlanes(bool isVisualize)
    {
        PlayerPrefs.SetInt("visualize_planes", isVisualize ? 1 : 0);
        PlayerPrefs.Save();
    }

}
