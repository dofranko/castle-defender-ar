using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesSystemScript : MonoBehaviour
{

    public CastleScript castle;
    [SerializeField]
    private Text timerText;
    private Camera cam;
    private int masksToFilter;
    void Start()
    {
        cam = Camera.main;
        masksToFilter = LayerMask.GetMask("Raycastable UI");
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 3, masksToFilter))
            {
                Debug.Log("hit " + hit.transform.name);
                switch (hit.transform.name)
                {
                    case "DefenseImage":
                        castle.defense += 1;
                        Debug.Log("upgraded defense");
                        break;
                    case "HealthImage":
                        break;
                    // TODO
                    case "ShieldImage":
                        break;
                    // TODO
                    case "MoneyImage":
                        break;
                    // TODO
                    case "SkipImage":
                        castle.HideUpgrades();
                        break;
                }
            }
        }
    }

    public void SetTimerText(string text)
    {
        timerText.text = text;
    }
}
