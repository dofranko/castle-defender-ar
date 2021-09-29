using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseUpgradeScript : MonoBehaviour
{

    private Camera cam;
    private int masksToIgnore;
    void Start()
    {
        cam = Camera.main;
        masksToIgnore = LayerMask.GetMask("Raycasted UI");
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 3, masksToIgnore))
            {
                Debug.Log("hit " + hit.transform.name);
                if (hit.transform.name == "DefenseImage")
                {
                    FindObjectOfType<CastleScript>().HideUpgrades();
                }
            }
        }
    }
}
