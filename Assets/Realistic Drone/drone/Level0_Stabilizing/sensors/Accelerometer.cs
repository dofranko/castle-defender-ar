using UnityEngine;
using System.Collections;

/// <summary>
/// Accelerometer sensor class
/// </summary>
public class Accelerometer : MonoBehaviour {

    private Vector3 LocalLinearVelocity;
    /// <summary>
    /// Gets the local linear velocity of the drone
    /// </summary>
    /// <returns></returns>
    public Vector3 getLocalLinearVelocity() { return noiseVel.getNoise(LocalLinearVelocity); }
    private Vector3 lastVelocity;

    private Vector3 LocalLinearAcceleration;
    /// <summary>
    /// Gets the local linear acceleration of the drone
    /// </summary>
    /// <returns></returns>
    public Vector3 getLinearAcceleration() { return noiseAcc.getNoise(LocalLinearAcceleration); }

    public Vector3 LV;

    noiseAddVector3 noiseVel;
    noiseAddVector3 noiseAcc;
    /// <summary>
    /// Function Called when the object is activated for the first time
    /// </summary>
    void Awake() {
        noiseAcc = new noiseAddVector3();
        noiseVel = new noiseAddVector3();
    }

    /// <summary>
    /// Function at regular time interval
    /// </summary>
    void FixedUpdate()
    {
        //rotate the velocity obtained from the rigidbody by 45° to obtain the local velocity
        LocalLinearVelocity = droneSettings.setZeroIflessThan(
            Quaternion.AngleAxis(45,transform.up) * transform.InverseTransformDirection(transform.GetComponent<Rigidbody>().velocity), 
            0.0001f);
        LocalLinearAcceleration = droneSettings.setZeroIflessThan((LocalLinearVelocity - lastVelocity) / Time.deltaTime, 0.0001f);

        lastVelocity = LocalLinearVelocity;
        LV = droneSettings.setZeroIflessThan(transform.GetComponent<Rigidbody>().velocity, 0.0001f);

    }

}
