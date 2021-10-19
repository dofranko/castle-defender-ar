using System;
using UnityEngine;

public class WaypointProgressTracker : MonoBehaviour
{

    // A reference to the waypoint-based route we should follow
    [SerializeField] private Waypoint waypoint; 

    /// <summary>
    /// Sets the waypoint that the drone will use to move
    /// </summary>
    /// <param name="wpc">Waypoint used by the drone. It can be a SinglePoint or a WaypointCircuit</param>
    public void setWaypoint(Waypoint wpc) {
        waypoint = wpc;
        progressDistance = 0;
        if (!wpc.isCircuit())
            ((singlePoint)waypoint).setHome(transform);
        else
            hasToRecalculateDistance();
    }

    [SerializeField] private float lookAheadForTargetOffset = 5;
    [SerializeField] private float maxDistFromCircuit = 1;
    [SerializeField] private float overallDistanceFromTrajectory = 0;
    [SerializeField] private float distanceOfPointToLookAt = 8;   

    public Transform target;
    private float progressDistance; // The progress round the route
    /// <summary>
    /// Gets the position of the drone in the circuit
    /// </summary>
    /// <returns>the position of the drone in the circuit</returns>
    public Vector3 getRoutePosition() { return waypoint.GetRoutePosition(progressDistance); }    

    float timer = 0.1f;
    float actualTimer = 1;

    /// <summary>
    /// We specify a rect and a point outside the rect. This function return the point INSIDE the rect, 
    /// that forms, with the point specified, a rect perpendicular to the one passed as argument
    /// </summary>
    /// <param name="ptR1">First point of the rect</param>
    /// <param name="ptR2">Second point of the rect</param>
    /// <param name="point">Point outside of the rect, that we want to find the corresponding perpendicular point</param>
    /// <returns>A point that forms a rect, with the point passed as argument, perpendicolar to the rect specified </returns>
    private Vector3 getPerpendicolarPoint(Vector3 ptR1, Vector3 ptR2, Vector3 point)
    {
        Vector2 A = new Vector2(ptR1.x, ptR1.z);
        Vector2 B = new Vector2(ptR2.x, ptR2.z);
        Vector2 C = new Vector2(point.x, point.z);

        float m = (B.y - A.y) / (B.x - A.x);
        float k = m * C.y + C.x;       
        float n = B.y - A.y;
        float o = B.x - A.x;
        float l = o * A.y - n * A.x;

        float newX = -(l * m - k * o) / (o + m * n);
        float newY = (k * n + l) / (o + m * n);

        // we do not consider the Y axes, so we always use the same as 'point' as they were all in the same Y plane
        return new Vector3(newX, point.y, newY);
    }

    private bool needToRecalculateDistanceFromCircuit = false;
    /// <summary>
    /// In some cases is necessary to recalculate the actual distance 
    /// instead of using the old one and summing to it the distance traveled.
    /// This function forces the recalculation.
    /// </summary>
    public void hasToRecalculateDistance() { needToRecalculateDistanceFromCircuit = true; }

    /// <summary>
    /// Function called each frame
    /// </summary>
    private void Update()
    {
        // if we are using a WaypointCircuit we recalculate the position of the objects used to navigate it
        if (waypoint.isCircuit())
        {
            if (actualTimer >= 0)
                actualTimer -= Time.deltaTime;
            else {
                actualTimer += timer;

                // calculating test-Points in circuit
                int nOfPoints = (int)lookAheadForTargetOffset;
                Vector3[] pBetween = waypoint.pointsBetween(getRoutePosition(), target.transform.position, nOfPoints);

                // calculating the matching points in the line between the drone and the target
                Vector3[] destPoints = new Vector3[pBetween.Length];
                for (int i = 0; i < pBetween.Length; i++)
                    destPoints[i] = getPerpendicolarPoint(transform.position, target.position, pBetween[i]);

                // calculating sum of distances between the test point and it's matching point
                overallDistanceFromTrajectory = 0;
                for (int i = 0; i < pBetween.Length - 1; i++)
                    overallDistanceFromTrajectory += Vector3.Distance(pBetween[i], destPoints[i]);

                // so we increase o decrease the distance of the target 
                if (overallDistanceFromTrajectory > maxDistFromCircuit)
                    lookAheadForTargetOffset -= 0.25f;
                else
                    lookAheadForTargetOffset += 0.25f;
            

                lookAheadForTargetOffset = droneSettings.keepOnRange(lookAheadForTargetOffset, 3f, 12f);            
            }

            // determine the position we should currently be aiming for
            // (this is different to the current progress position, it is a certain amount ahead along the route)        
            target.position = waypoint.GetRoutePoint(progressDistance + lookAheadForTargetOffset).position;
            target.rotation = Quaternion.LookRotation(waypoint.GetRoutePoint(progressDistance).direction);
            // get our current progress along the route       

            needToRecalculateDistanceFromCircuit = Vector3.Distance(transform.position, getRoutePosition()) > 10 ? true : needToRecalculateDistanceFromCircuit;
            if (needToRecalculateDistanceFromCircuit)
            {
                progressDistance = waypoint.getNearestPointTo(transform.position);
                needToRecalculateDistanceFromCircuit = false;
            }
            else
            {
                WaypointCircuit.RoutePoint progressPoint = waypoint.GetRoutePoint(progressDistance);
                Vector3 progressDelta = progressPoint.position - transform.position;
                if (Vector3.Dot(progressDelta, progressPoint.direction) < 0)
                {
                    progressDistance += progressDelta.magnitude * 0.5f;
                }
            }
            
            // setting drones variables
            gameObject.GetComponent<droneMovementController>().setRoutePos(getRoutePosition());
            float distToRoute = Vector3.Distance(transform.position, getRoutePosition());
            gameObject.GetComponent<droneMovementController>().setLookingPoint(waypoint.GetRoutePosition(progressDistance + distanceOfPointToLookAt - distToRoute));
            gameObject.GetComponent<droneMovementController>().stayOnFixedPoint = false;
        }
        else
        {
            Vector3 routePos = getRoutePosition();

            // setting drones variables
            gameObject.GetComponent<droneMovementController>().setRoutePos(routePos);
            target.position = getRoutePosition();// + Vector3.forward;
            gameObject.GetComponent<droneMovementController>().stayOnFixedPoint = true;         
            gameObject.GetComponent<droneMovementController>().setLookingPoint(((singlePoint) waypoint).getLookingAtPoint());
    
        }

        
        
    }

    // --- function used to draw lines and shapes in the editor-mode

    public bool drawGizoms = true;
        private void OnDrawGizmos()
        {
            if (Application.isPlaying && drawGizoms)
            { 
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, target.position);
                Gizmos.DrawWireSphere(waypoint.GetRoutePosition(progressDistance), 0.25f);
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(target.position, target.position + target.forward);
            }
        }
    }
