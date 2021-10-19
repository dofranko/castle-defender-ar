using UnityEngine;
using System.Collections;

/// <summary>
/// Gyro sensor class
/// </summary>
public class Gyro : MonoBehaviour {
    
    private rotor helixV1;
    private rotor helixV2;
    private rotor helixO1;
    private rotor helixO2;
    private float distanceBetweenHelixes;

    private float vDifference;
    private float oDifference;

    private float pitch = 0;
    /// <summary>
    /// Gets the actual pitch
    /// </summary>
    public float getPitch() { return nPitch.getNoise(pitch); }
    private float lastPitch = 0;    
    private float pitchVel = 0;
    /// <summary>
    /// Gets the actual pitch-velocity
    /// </summary>
    public float getPitchVel() { return nPitchSpeed.getNoise(pitchVel); }
    private float lastPitchVel = 0;
    private float pitchAcc = 0;
    /// <summary>
    /// Gets the actual pitch-acceleration
    /// </summary>
    public float getPitchAcc() { return nPitchAcc.getNoise(pitchAcc); }

    private float roll = 0;
    /// <summary>
    /// Gets the actual roll
    /// </summary>
    public float getRoll() { return nRoll.getNoise(roll); }
    private float lastRoll = 0;
    private float rollVel = 0;
    /// <summary>
    /// Gets the actual roll-velocity
    /// </summary>
    public float getRollVel() { return nRollSpeed.getNoise(rollVel); }
    private float lastRollVel = 0;
    private float rollAcc = 0;
    /// <summary>
    /// Gets the actual roll-acceleration
    /// </summary>
    public float getRollAcc() { return nRollAcc.getNoise(rollAcc); }

    noiseAdder nRoll;
    noiseAdder nRollSpeed;
    noiseAdder nRollAcc;
    noiseAdder nPitch;
    noiseAdder nPitchSpeed;
    noiseAdder nPitchAcc;

    public Transform body;

    Vector3 AngularVelocity;
    Vector3 AngularAcceleration;

    private float normalize (float n) { return droneSettings.normalizeBetween(n, 0, distanceBetweenHelixes); }

    /// <summary>
    /// Function called before of the first update
    /// </summary>
    void Start()
    {
        droneMovementController dmc = gameObject.GetComponent<droneMovementController>();

        // associate the rotors
        helixO1 = dmc.helixO1;
        helixO2 = dmc.helixO2;
        helixV1 = dmc.helixV1;
        helixV2 = dmc.helixV2;

        // finds the rotors position
        Vector3 v1 = helixV1.transform.position;
        Vector3 v2 = helixV2.transform.position;
        Vector3 o1 = helixO1.transform.position;
        Vector3 o2 = helixO2.transform.position;

        Vector3 mid_v1o1 = (v1 + o1) / 2f;
        Vector3 mid_v2o2 = (o2 + v2) / 2f;
        distanceBetweenHelixes = Vector3.Distance(mid_v1o1, mid_v2o2);

        // instantiates the noiseAdders, to add the noise to the measurement
        nRoll = new noiseAdder();
        nRollSpeed = new noiseAdder();
        nRollAcc = new noiseAdder();
        nPitch = new noiseAdder();
        nPitchSpeed = new noiseAdder();
        nPitchAcc = new noiseAdder();
    }

    /// <summary>
    /// Function at regular time interval
    /// </summary>
    void FixedUpdate()
    {
        Vector3 v1 = helixV1.transform.position;
        Vector3 v2 = helixV2.transform.position;
        Vector3 o1 = helixO1.transform.position;
        Vector3 o2 = helixO2.transform.position;

        Vector3 mid_v1o2 = (v1 + o2) / 2f;
        Vector3 mid_v1o1 = (v1 + o1) / 2f;
        Vector3 mid_v2o2 = (o2 + v2) / 2f;
        Vector3 mid_v2o1 = (o1 + v2) / 2f;
         
        // calculate pitch values
        pitch = mid_v2o2.y - mid_v1o1.y;  
        pitchVel = (pitch - lastPitch) / Time.deltaTime;        
        pitchAcc = (pitchVel - lastPitchVel) / Time.deltaTime;
        lastPitch = pitch;
        lastPitchVel = pitchVel;
        pitch = normalize(pitch);

        // calculate roll values
        roll = mid_v2o1.y - mid_v1o2.y;
        rollVel = (roll - lastRoll) / Time.deltaTime;
        rollAcc = (rollVel - lastRollVel) / Time.deltaTime;
        lastRoll = roll;
        lastRollVel = rollVel;
        roll = normalize(roll);


        // --- NOT USED --- //
        // calculate angular acceleration and velocity of the drone     
        //AngularAcceleration = droneSettings.setZeroIflessThan((transform.GetComponent<Rigidbody>().angularVelocity - AngularVelocity) / Time.deltaTime, 0.00001f);
        //AngularVelocity = droneSettings.setZeroIflessThan(transform.GetComponent<Rigidbody>().angularVelocity, 0.00001f);

    }

}
