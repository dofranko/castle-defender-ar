using UnityEngine;
using System.Collections;

public class IA : MonoBehaviour {

    // model and istance of the WaypointCircuit
    public GameObject baseWaypointCircuit;
    WaypointCircuit usedWaypoint;

    // model and istance of the SinglePoint
    public GameObject singlePointObject;
    singlePoint usedSinglePoint;

    WaypointProgressTracker myWpPrTrk;

    // fixed height for the WaypointCircuit
    float wpHeight = 3;

    // number of points in the WaypointCircuit
    int actualDotsQty;
    int maxDotsQty = 15;

    /// <summary>
    /// Function Called when the object is activated for the first time. Used for initializations
    /// </summary>
    void Awake () {
        myWpPrTrk = transform.GetComponent<WaypointProgressTracker>();        
        usedWaypoint = ((GameObject)Instantiate(baseWaypointCircuit, transform.position + Vector3.up * wpHeight, Quaternion.identity)).GetComponent<WaypointCircuit>();
        usedSinglePoint = ((GameObject)Instantiate(singlePointObject, transform.position + Vector3.up * wpHeight, Quaternion.identity)).GetComponent<singlePoint>();
        
        myWpPrTrk.setWaypoint(usedSinglePoint);
        actualDotsQty = usedWaypoint.transform.childCount;
	}    
    
    /// <summary>
    /// Specifies the max number of points in the circuit. When its points exceedes this number, we'll start to remove the old ones
    /// </summary>
    /// <param name="pts">Max number of points in the circuit</param>
    protected void setNrOfPointsInCircuit(int pts) { maxDotsQty = pts; }

    /// <summary>
    /// Adds a point to the circuit and eventually removes the oldest one
    /// </summary>
    /// <param name="p">Point that has to be added to the circuit</param>
    protected void addPointToCircuit(Vector3 p)
    {
        usedWaypoint.addPointAtTheEndOfTheCircuit(p);
        actualDotsQty++;
        if (actualDotsQty > maxDotsQty)
        {
            usedWaypoint.cleanOldestWaypoint();
            actualDotsQty--;
        }
        myWpPrTrk.hasToRecalculateDistance();
    }

    float pointMovingStep = 0.1f;
    /// <summary>
    /// The position of the singlePoint
    /// </summary>
    /// <returns>The position of the singlePoint</returns>
    protected Vector3 getSinglePointPosition() { return usedSinglePoint.transform.position; }
    
    /// <summary>
    /// Move the singlePoint using a direction vector
    /// </summary>
    /// <param name="v">Directional vector that indicates the direction and distance of movement</param>
    protected void moveSinglePoint(Vector3 v)
    {
        usedSinglePoint.moveSinglePoint(v);
    }

    /// <summary>
    /// Sets a new position for the SinglePoint
    /// </summary>
    /// <param name="pt">New position of SinglePoint</param>
    protected void setSinglePointPosition(Vector3 v)
    {
        usedSinglePoint.setReferencePoint(v);
    }


    /// <summary>
    /// Lock the SinglePoint so it won't move to the drone if it doesn't receibe commands
    /// </summary>
    protected void lockSinglePoint() { usedSinglePoint.moveBackToHomeDuringIdle = false; }
    
    /// <summary>
    /// Unlock the SinglePoint so it will move to the drone if it doesn't receibe commands
    /// </summary>
    protected void unlockSinglePoint() { usedSinglePoint.moveBackToHomeDuringIdle = true; }

    /// <summary>
    /// With this function we can activate o deactivate the reception of commands in the SinglePoint
    /// </summary>
    /// <param name="b">boolean that specify is the SinglePoint is going to receibe commands or not</param>
    protected void setSinglePointcommandResponsive(bool b) { usedSinglePoint.acceptCommands = b; }

    /// <summary>
    /// Sets the distance of the point the SinglePoint is using as reference for rotation. 
    /// <para> when we want the drone doesn't change its orientation we just have to set this point really far</para>
    /// </summary>
    /// <param name="f">Distance of the point the SinglePoint is using as reference for rotation</param>
    protected void setDistanceOfPointToLookAt(float f) { usedSinglePoint.distance = f; }

    /// <summary>
    /// Modify the Waypoint in the WaypointProgressTracker, putting the SinglePoint
    /// </summary>
    protected void switchToSinglePoint() { myWpPrTrk.setWaypoint(usedSinglePoint); usedSinglePoint.acceptCommands = true;  }

    /// <summary>
    /// Modify the Waypoint in the WaypointProgressTracker, putting the WaypointCircuit
    /// </summary>
    protected void switchToCircuit() { myWpPrTrk.setWaypoint(usedWaypoint); usedSinglePoint.acceptCommands = false; }

    /// <summary>
    /// Function that says if the drone has completed the circuit or not.
    /// <para>!!Now it always return FALSE!!</para>
    /// </summary>
    /// <returns>Returns true if the drone has completed the circuit, otherwise false</returns>
    protected bool hasDoneAllTheCircuit() { return false; }

    /// <summary>
    /// Says to the WaypointProgressTracker that is necessary to recalculate the actual distance, 
    /// from the starting point, of the RoutePosition in the circuit
    /// </summary>
    protected void needToRecalculateDistanceFromCircuit() { myWpPrTrk.hasToRecalculateDistance(); }

    /// <summary>
    /// Gets the distance of the drone from its routePosition
    /// </summary>
    /// <returns>The distance of the drone from its routePosition</returns>
    protected float getDistanceFromRoutePoint() { return Vector3.Distance(transform.position, myWpPrTrk.getRoutePosition()); }
}
