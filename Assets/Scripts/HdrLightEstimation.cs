using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class HdrLightEstimation : MonoBehaviour
{

    [SerializeField] private Light light;
    [SerializeField] private ARCameraManager cameraManager;

    void OnEnable()
    {
        if (PlayerPrefs.HasKey("dynamic_light"))
        {
            if (PlayerPrefs.GetInt("dynamic_light") == 0){
                enabled = false;
                return;
            }
        }
        if (cameraManager && light)
        {
            cameraManager.frameReceived += FrameChanged;
        }
    }

    void OnDisable()
    {
        if (cameraManager && light)
        {
            cameraManager.frameReceived -= FrameChanged;
        }
    }

    void FrameChanged(ARCameraFrameEventArgs args)
    {
        //intensity
        if (args.lightEstimation.averageMainLightBrightness.HasValue)
        {
            light.intensity = args.lightEstimation.averageMainLightBrightness.Value;
        }
        else if (args.lightEstimation.averageBrightness.HasValue)
        {
            light.intensity = args.lightEstimation.averageBrightness.Value;
        }
        //color temperature
        if (args.lightEstimation.averageColorTemperature.HasValue)
        {
            light.colorTemperature = args.lightEstimation.averageColorTemperature.Value;
        }
        //color
        if (args.lightEstimation.mainLightColor.HasValue)
        {
            light.color = args.lightEstimation.mainLightColor.Value;
        }
        else if (args.lightEstimation.colorCorrection.HasValue)
        {
            light.color = args.lightEstimation.colorCorrection.Value;
        }
        //light direction
        if (args.lightEstimation.mainLightDirection.HasValue)
        {
            light.transform.rotation = Quaternion.LookRotation(args.lightEstimation.mainLightDirection.Value);
        }
        //light ambient 
        if (args.lightEstimation.ambientSphericalHarmonics.HasValue)
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
            RenderSettings.ambientProbe = args.lightEstimation.ambientSphericalHarmonics.Value;
        }

    }
}
