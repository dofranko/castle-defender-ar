using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distancer : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text distanceText;
    private Camera cam;
    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        distanceText.text = Vector3.Distance(transform.position, cam.transform.position).ToString("0.00") + " m";
    }
}
