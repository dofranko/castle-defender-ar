using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesSystemScript : MonoBehaviour
{

    private Camera cam;
    private int masksToFilter;
    void Start()
    {
        cam = Camera.main;
        masksToFilter = LayerMask.GetMask("Raycasted UI");
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 3, masksToFilter))
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
