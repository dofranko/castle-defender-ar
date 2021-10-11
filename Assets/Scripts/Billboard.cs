using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Transform bottomLeft;
    [SerializeField] private Transform topRight;
    [SerializeField] private float maxViewportWidth;
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
            doScale = true;
            originalScale = new Vector2(transform.localScale.x, transform.localScale.y);
            scalingVector = new Vector3(0.15f * originalScale.x, 0.15f * originalScale.y);
            startScalingUpViewport = maxViewportWidth - 0.1f;
            if (startScalingUpViewport <= 0) startScalingUpViewport = 0.02f;
        }
    }
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.transform.forward);
        if (!doScale) return;

        var firstX = cam.WorldToViewportPoint(bottomLeft.position).x;
        var secondX = cam.WorldToViewportPoint(topRight.position).x;
        if ((firstX < 0 && secondX < 0) || (firstX > 1 && secondX > 1)) return;//jeśli są całkiem poza kadrem

        var uiViewportWidth = Mathf.Abs(secondX - firstX);
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
