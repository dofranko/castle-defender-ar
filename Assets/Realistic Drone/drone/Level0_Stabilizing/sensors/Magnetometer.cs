using UnityEngine;
using System.Collections;

/// <summary>
/// Magnetometer sensor class
/// </summary>
public class Magnetometer : MonoBehaviour {

    // the north pole is the fixed point used by the magnetometer to measure the rotation
    private static Vector2 northPolePosition = new Vector2(0,40000);
    public Vector3 getNorthPolePosition() { return northPolePosition; }

    private rotor helixV1;
    private rotor helixV2;
    private rotor helixO1;
    private rotor helixO2;
     
    noiseAdder nYaw;
    noiseAdder nSpeed;
     
    private float yaw;
    /// <summary>
    /// Gets the actual Yaw of the drone
    /// </summary>
    public float getYaw() { return nYaw.getNoise(yaw); }
    private float lastYaw;
    private float yawVel;
    /// <summary>
    /// Gets the actual Yaw of the drone
    /// </summary>
    public float getYawVel() { return nSpeed.getNoise(yawVel); }

    /// <summary>
    /// Trasforms world coordinates to local coordinates, using as rotation reference the lookingAtPoint
    /// </summary>
    /// <param name="worldPt">Global coordinates that we want to transform to local </param>
    /// <param name="lookingAtPoint">Point the drone is facing to</param>
    /// <returns>The world coordinates transformed to local</returns>
    public Vector3 worldToLocalPoint(Vector3 worldPt, Vector3 lookingAtPoint)
    {
        Vector3 forwardVector = new Vector3(0, 0, Vector3.Distance(transform.position, worldPt));
        return Quaternion.AngleAxis((getYawToCenterOn(lookingAtPoint) - getYaw()) * -180, transform.up) * forwardVector;        
    }

    /// <summary>
    /// Function Called when the object is activated for the first time
    /// </summary>
    void Awake()
    {
        // initializes the noiseAdders
        nYaw = new noiseAdder();
        nSpeed = new noiseAdder();

        // look for the rotors in the droneMovementController
        droneMovementController dmc = gameObject.GetComponent<droneMovementController>();
        helixO1 = dmc.helixO1;
        helixO2 = dmc.helixO2;
        helixV1 = dmc.helixV1;
        helixV2 = dmc.helixV2;
        lastYaw = yaw = getYawINT();     
    }

    /// <summary>
    /// Yaw that corresponds to the rotation needed to face the destPoint
    /// </summary>
    /// <param name="destPoint">Point the drone has to face</param>
    /// <returns>Yaw to face the destPoint from the actual position </returns>
    public float getYawToCenterOn(Vector3 destPoint) { return getYawToCenterOn(new Vector2(destPoint.x, destPoint.z)); }
    /// <summary>
    /// Yaw that corresponds to the rotation needed to face the destPoint
    /// </summary>
    /// <param name="destPoint">Point the drone has to face</param>
    /// <returns>Yaw to face the destPoint from the actual position </returns>
    public float getYawToCenterOn(Vector2 destPoint)
    {
        Vector2 myPos = new Vector2(transform.position.x, transform.position.z);
        
        Vector2 direction = destPoint - myPos;
        Vector2 reference = northPolePosition - myPos;
         
        bool isPositive = myPos.x > destPoint.x; //&& destPoint.x < northPolePosition.x || myPos.x > destPoint.x && destPoint.x > northPolePosition.x;
        float alpha = Vector2.Angle(direction, reference);

        return droneSettings.normalizeBetween(alpha, 0, 180f) * (isPositive ? 1 : -1);
    }

    /// <summary>
    /// Private function used to calculate the Yaw of the drone
    /// </summary>
    /// <returns>Yaw of the drone</returns>
    private float getYawINT()
    {
        Vector2 v1 = new Vector2(helixV1.transform.position.x, helixV1.transform.position.z);
        Vector2 v2 = new Vector2(helixV2.transform.position.x, helixV2.transform.position.z);
        Vector2 o1 = new Vector2(helixO1.transform.position.x, helixO1.transform.position.z);
        Vector2 o2 = new Vector2(helixO2.transform.position.x, helixO2.transform.position.z);

        // The line between the midpoints is our direction
        Vector2 direction = ((v1 + o1) / 2f) - ((v2 + o2) / 2f);
        Vector2 reference = northPolePosition - new Vector2(transform.position.x, transform.position.z);

        bool isPositive = Vector2.Distance((v2 + o1) / 2f, northPolePosition) > Vector2.Distance((v1 + o2) / 2f, northPolePosition);
        float alpha = Vector2.Angle(direction, reference);

        return droneSettings.normalizeBetween(alpha, 0,180f) * (isPositive ? 1 : -1);

    }

    /// <summary>
    /// Function at regular time interval
    /// </summary>
    void FixedUpdate()
    {
        yaw = getYawINT();
        float covedredDistance = (yaw - lastYaw);
        covedredDistance = (Mathf.Abs(covedredDistance) < 1 ? covedredDistance : (covedredDistance > 0 ? covedredDistance - 2 : covedredDistance + 2));
        yawVel = covedredDistance / Time.deltaTime;

        lastYaw = yaw;
    }


}
