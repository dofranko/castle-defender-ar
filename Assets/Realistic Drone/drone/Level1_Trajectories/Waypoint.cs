using UnityEngine;
using System.Collections;

public abstract class Waypoint : MonoBehaviour {

    /// <summary>
    /// Structure that save the position and direction of the target
    /// </summary>
    public struct RoutePoint
    {
        public Vector3 position;
        public Vector3 direction;


        public RoutePoint(Vector3 position, Vector3 direction)
        {
            this.position = position;
            this.direction = direction;
        }
    }

    /// <summary>
    /// Specify is the waypoint is a WaypointCircuit or a CinglePoint
    /// </summary>
    /// <returns>True if it is a WaypointCircuit, otherwise false</returns>
    public abstract bool isCircuit();

    /// <summary>
    /// Given two points that can be inside or outside the circuit, the function returns a number 'nPoints' of points
    /// in the circuit between these points
    /// </summary>
    /// <param name="pt1">First point</param>
    /// <param name="pt2">Second point</param>
    /// <param name="nOfPoints">Number of points needed to extract</param>
    /// <returns>Returns an array of points containing all the points IN the circuit between the points passed as parameters </returns>
    public abstract Vector3[] pointsBetween(Vector3 pt1, Vector3 pt2, int nOfPoints);

    /// <summary>
    /// Given a point, it returns the distance from the beginning of the nearest point in the circuit to this point
    /// </summary>
    /// <param name="position">Point we use to look for the nearest point in the circuit</param>
    /// <returns>The nearest point to the one passed as parameter, in the circuit</returns>
    public abstract float getNearestPointTo(Vector3 position);

    /// <summary>
    /// Given an absolute distance from the first point, it returns a RoutePoint at that distance
    /// </summary>
    /// <param name="distance">Float containing the distance from the first point</param>
    /// <returns>A routePoint containing the position and the direction of the resulting point </returns>
    public abstract RoutePoint GetRoutePoint(float distance);

    /// <summary>
    /// Given an absolute distance from the first point, it returns a point in the circuit at that distance
    /// </summary>
    /// <param name="progressDistance">Float containing the distance from the first point</param>
    /// <returns>returns a point in the circuit at the specified distance</returns>
    public abstract Vector3 GetRoutePosition(float progressDistance);
}
