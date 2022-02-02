using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
public class ShadowsDisabler : MonoBehaviour
{
    [SerializeField] private ARPlaneManager arPlaneManager;
    void Start()
    {
        StartCoroutine(TurnOffShadowPlanePrefab());
    }
    private IEnumerator TurnOffShadowPlanePrefab()
    {
        yield return new WaitForSeconds(2.0f);
        if (PlayerPrefs.HasKey("shadows"))
        {
            if (PlayerPrefs.GetInt("shadows") == 0)
            {
                int tries = 10;
                while (tries > 0)
                {
                    try
                    {
                        arPlaneManager.planePrefab = null;
                        tries = 0;
                        break;
                    }
                    catch
                    {
                    }
                    tries--;
                    yield return new WaitForSeconds(0.2f);
                }
                arPlaneManager.SetTrackablesActive(false);
            }
        }
    }
}
