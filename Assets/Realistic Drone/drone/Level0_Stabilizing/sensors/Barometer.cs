using UnityEngine;
using System.Collections;

/// <summary>
/// Barometer sensor class
/// </summary>
public class Barometer : MonoBehaviour {

    private float height;
    /// <summary>
    /// Gets the height of the drone
    /// </summary>
    public float getHeight() { return nHeight.getNoise(height); }
    private float lastHeight;

    private float verticalSpeed;
    /// <summary>
    /// Gets the vertical speed of the drone
    /// </summary>
    public float getverticalSpeed() { return nSpeed.getNoise(verticalSpeed); }
    private float lastSpeed;

    private float verticalAcc;
    /// <summary>
    /// Gets the vertical acceleration of the drone
    /// </summary>
    public float getverticalAcc() { return nAcc.getNoise(verticalAcc); }

    noiseAdder nHeight;
    noiseAdder nSpeed;
    noiseAdder nAcc;

    /// <summary>
    /// Function Called when the object is activated for the first time
    /// </summary>
    void Awake()
    {
        nHeight = new noiseAdder();
        nSpeed = new noiseAdder();
        nAcc = new noiseAdder();

        lastHeight = height = transform.position.y;
        lastSpeed = verticalSpeed = 0;
        verticalAcc = 0;
    }

    /// <summary>
    /// Function at regular time interval
    /// </summary>
    void FixedUpdate()
    {
        height = transform.position.y;
        verticalSpeed = (height - lastHeight) / Time.deltaTime;
        verticalAcc = (verticalSpeed - lastSpeed) / Time.deltaTime;

        lastHeight = height;
        lastSpeed = verticalSpeed;
    }
}
