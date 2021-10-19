using UnityEngine;
using System.Collections;
using System;

public class singlePoint : Waypoint {

    /// <summary>
    /// Function called before of the first update
    /// </summary>
    void Start() { setReferencePoint(transform.position); }

    // -- single point view vars
    private float rotationVelocity = 0.8f;
    public float distance = 8;
    private float angle = 0;
    public Transform viewPoint;
    public bool moveBackToHomeDuringIdle = true;

    /// <summary>
    /// Function called each frame. It is used to give commands to the SinglePoint
    /// </summary>
    void Update() {
        //setReferencePoint(transform.position);

        if (!acceptCommands) return;

        Vector3 v = new Vector3();
        if (Input.GetKey(KeyCode.UpArrow)) v = transform.forward;
        else if (Input.GetKey(KeyCode.DownArrow)) v = transform.forward * -1;
        if (Input.GetKey(KeyCode.RightArrow)) v = transform.right;
        else if (Input.GetKey(KeyCode.LeftArrow)) v = transform.right * -1;
        if (Input.GetKey(KeyCode.W)) v = Vector3.up;
        else if (Input.GetKey(KeyCode.S)) v = Vector3.up * -1;
        if (Input.GetKey(KeyCode.A)) rotate(1);
        else if (Input.GetKey(KeyCode.D)) rotate(-1);

        // if a command is given it moves the point
        if (!v.Equals(new Vector3()))
            moveSinglePoint(v);
        // else it stays OR goes back to the home (the drone)
        else if (moveBackToHomeDuringIdle && home != null)
            setReferencePoint(Vector3.MoveTowards(transform.position, home.position, pointMovingStep / 4f * Time.deltaTime));
    }

    /// <summary>
    /// Rotates the SinglePoint, so the orientation of the drone will change too
    /// </summary>
    /// <param name="verse">Verse of the rotation</param>
    public void rotate(int verse) { angle += rotationVelocity * (verse == 0 ? 0 : Mathf.Sign(verse)) * Time.deltaTime; }


    public bool acceptCommands = false;
    public float pointMovingStep = 5f;

    /// <summary>
    /// Move the singlePoint using a direction vector
    /// </summary>
    /// <param name="v">Directional vector that indicates the direction and distance of movement</param>
    public void moveSinglePoint(Vector3 v)
    {
        transform.position += v * pointMovingStep * Time.deltaTime;
        setReferencePoint(transform.position);
    }

    private Vector3 referencePoint;
    private Transform home;
    /// <summary>
    /// Sets the variable referencePoint and moves the SinglePoint on it
    /// </summary>
    /// <param name="pt">New position of referencePoint</param>
    public void setReferencePoint(Vector3 pt) { referencePoint = pt; transform.position = pt; }

    /// <summary>
    /// Sets the transform that will be used as home.
    /// <para>The home is the point where the drone will go if it didn't recibe any command</para>
    /// </summary>
    /// <param name="t">Transform used as home (the drone)</param>
    public void setHome(Transform t) { home = t; }

    /// <summary>
    /// Execute a calculation and find the point the drone has to look at
    /// </summary>
    /// <returns>The point the drone has to look at</returns>
    public Vector3 getLookingAtPoint()
    {        
        viewPoint.transform.position = new Vector3(Mathf.Cos(angle) * distance + transform.position.x, transform.position.y, Mathf.Sin(angle) * distance + transform.position.z);
        transform.LookAt(viewPoint.transform.position);
        return viewPoint.transform.position;
    }


    // --- Methods of the abstract class

    /// <summary>
    /// Given a point, it returns the distance from the beginning of the nearest point in the circuit to this point
    /// </summary>
    /// <param name="position">Point we use to look for the nearest point in the circuit</param>
    /// <returns>It is a single point so it'll always return 0</returns>
    public override float getNearestPointTo(Vector3 position) { return 0; }

    /// <summary>
    /// Given an absolute distance from the first point, it returns a RoutePoint at that distance
    /// </summary>
    /// <param name="distance">Float containing the distance from the first point</param>
    /// <returns>It is a single point, so it'll return a RoutePoint containing the SinglePoint position and a fake direction </returns>
    public override RoutePoint GetRoutePoint(float v) { return new RoutePoint(referencePoint, new Vector3()); }

    /// <summary>
    /// Given an absolute distance from the first point, it returns a point in the circuit at that distance
    /// </summary>
    /// <param name="progressDistance">Float containing the distance from the first point</param>
    /// <returns>It is a single point, so it'll return the SinglePoint position</returns>
    public override Vector3 GetRoutePosition(float progressDistance) { return referencePoint; }

    /// <summary>
    /// Specify is the waypoint is a WaypointCircuit or a CinglePoint
    /// </summary>
    /// <returns>Returns always FALSE</returns>
    public override bool isCircuit() { return false; }

    /// <summary>
    /// Given two points that can be inside or outside the circuit, the function returns a number 'nPoints' of points
    /// in the circuit between these points
    /// </summary>
    /// <param name="pt1">First point</param>
    /// <param name="pt2">Second point</param>
    /// <param name="nOfPoints">Number of points needed to extract</param>
    /// <returns>It is a single point so it'll always return an empty array of points </returns>
    public override Vector3[] pointsBetween(Vector3 vector3, Vector3 position, int nOfPoints) { return new Vector3[0]; }

}
