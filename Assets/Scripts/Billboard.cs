using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Transform bottomLeft;
    [SerializeField] private Transform topRight;
    [SerializeField] private float maxViewportWidth;
    [SerializeField] private int degreeSnap;
    [SerializeField] private bool rotateUp;

    private bool doScale = false;
    private Vector2 originalScale;
    private Vector3 scalingVector;
    private float startScalingUpViewport;

    // private Vector3 scalingVectorUp;
    private Camera cam;
    void Start()
    {
        cam = Camera.main;
        if (bottomLeft && topRight && canvas && maxViewportWidth > 0)
        {
            doScale = false;
            originalScale = new Vector2(transform.localScale.x, transform.localScale.y);
            scalingVector = new Vector3(0.15f * originalScale.x, 0.15f * originalScale.y);
            startScalingUpViewport = maxViewportWidth - 0.1f;
            if (startScalingUpViewport <= 0) startScalingUpViewport = 0.02f;
        }
    }
    void LateUpdate()
    {
        if (rotateUp)
        {
            transform.LookAt(transform.position + cam.transform.forward);
        }
        else
        {
            var lookPos = transform.position - cam.transform.position;
            lookPos.y = 0;
            transform.rotation = Quaternion.LookRotation(lookPos);
        }
        if (degreeSnap != 0)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Round(transform.eulerAngles.y / degreeSnap) * degreeSnap, transform.eulerAngles.z);
        }
        if (!doScale) return;

        var firstX = cam.WorldToViewportPoint(bottomLeft.position);
        var secondX = cam.WorldToViewportPoint(topRight.position);
        if ((firstX.x < 0 && secondX.x < 0) || (firstX.x > 1 && secondX.x > 1)) return;//jeśli są całkiem poza kadrem

        var uiViewportWidth = Mathf.Abs(Vector2.Distance(new Vector2(secondX.x, secondX.y), new Vector2(firstX.x, firstX.y)));
        if (uiViewportWidth > maxViewportWidth)
        {
            transform.localScale -= scalingVector * Time.deltaTime;
        }
        else if (uiViewportWidth <= startScalingUpViewport && transform.localScale.x < originalScale.x)
        {
            transform.localScale += scalingVector * Time.deltaTime;
        }
    }
}
