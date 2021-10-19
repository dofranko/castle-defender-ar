using UnityEngine;
using System.Collections;

public abstract class OptimizationBehaviour : MonoBehaviour {

    // list of PIDs that will be initialized with the values of the training
    // and injected to the drone, substituting the default ones
    protected PID yawPID;
    protected PID rollPID;
    protected PID pitchPID;
    protected PID yPID;
    protected PID zPID;
    protected PID xPID;

    protected int id = 0;
    private static int idNr = 1;
    /// <summary>
    /// Sets the id of the drone using the static variable idNr
    /// </summary>
    private void setID() { id = idNr++; }
    /// <summary>
    /// Gets the id of the drone
    /// </summary>
    public int getID() { return id; }

    /// <summary>
    /// Function that hides the mesh of the drone. 
    /// In this way it is possible to use more of them without overloading the GPU
    /// </summary>
    /// <param name="b">Boolean that indicates to hide the body or not</param>
    private void hideBody(bool b)
    {
        if (!b) return;
        transform.Find("Body").gameObject.SetActive(false);
        transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false;
        transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        transform.GetChild(4).GetComponent<MeshRenderer>().enabled = false;
        transform.GetChild(3).GetComponent<MeshRenderer>().enabled = false;
    }

    private float age = 0;
    /// <summary>
    /// Gets the age, in seconds, of the drone
    /// </summary>
    public float getAge() { return age; }

    private float lifeTime;
    /// <summary>
    /// Sets the lifetime of the drone, in seconds. 
    /// After this time it'll die of natural death and send its data to the manager
    /// </summary>
    /// <param name="lt"></param>
    private void setLifeTime(float lt) { lifeTime = lt; }

    /// <summary>
    /// returns true if the age of the drone is greather than its lifetime
    /// <para>If this function is true the drone die of natural death and send its data to the manager</para>
    /// </summary>
    protected bool isOldDead() { return age > lifeTime; }

    /// <summary>
    /// returns true if the drone gets too far from the circuit or too near to the ground
    /// <para>If this function is true the drone die of non-natural death and send its data to the manager, 
    /// but it won't be taken in consideration as if it were dead of natural death</para>
    /// </summary>
    protected bool hasToBeKilled() { return Vector3.Distance(transform.position, routePosition) > maxDistance || transform.position.y < 1f; }

    private float maxDistance = 10f;

    protected OptimizationManager father;
    /// <summary>
    /// Sets the manager of the drone. 
    /// The reference will be used to send the result of the experiment (the fitness)
    /// </summary>
    public void setFather(OptimizationManager om) { father = om; }
    protected droneMovementController dmc;
    protected WaypointProgressTracker wpt;

    protected GameObject spawnedWaypoint;

    private Vector3 lastroutePosition;
    private Vector3 routePosition;   
    protected float distanceMade;
    protected float accumulatedDistanceFromRoutePosition = 0;

    /// <summary>
    /// Function called in the Start function that initializes the variables used for the experiment
    /// </summary>
    /// <param name="lifeTime">Time the drone will live before of dying of natural death </param>
    /// <param name="hidebody">Indicate if the mesh of the drone has to be hidden or not </param>
    /// <param name="routePos">Route position in the circuit. 
    /// It is used to estabilish if the drone has to be killed because too far from the circuit </param>
    protected void startOperations(float lifeTime, bool hidebody, Vector3 routePos) {
        setID();
        hideBody(hidebody);
        setLifeTime(lifeTime);
        dmc = gameObject.GetComponent<droneMovementController>();
        routePosition = routePos;
        lastroutePosition = routePos;
    }

    /// <summary>
    /// Function called in the Update that keep update the parameter
    /// </summary>
    /// <param name="routePos">Route position in the circuit. 
    /// It is used to estabilish if the drone has to be killed because too far from the circuit </param>
    protected void updateOperations(Vector3 routePos) {
        age += Time.deltaTime;
        lastroutePosition = routePosition;
        routePosition = routePos;
        distanceMade += Vector3.Distance(lastroutePosition, routePosition);
        accumulatedDistanceFromRoutePosition += Vector3.Distance(transform.position, routePosition);
    }

    /// <summary>
    /// function used to calculate the fitness of the configuration used
    /// </summary>
    public abstract float fitness();
    
    /// <summary>
    /// returns the keys used for the experiment
    /// </summary>
    public abstract float[] getKeys();

    /// <summary>
    /// Sets the keys used for the experiment and the waypoint
    /// </summary>
    /// <param name="ks">Array of float containing the parameters</param>
    /// <param name="waypoint">WaypointCircuit that will be used for the experiment</param>
    public abstract void setKeys(float[] ks, GameObject waypoint);

    /// <summary>
    /// Special case where the keys are not setted by the manager, 
    /// they are calculated randomly by the same individual
    /// </summary>
    /// <param name="waypoint">WaypointCircuit that will be used for the experiment</param>
    public abstract void setInitialKs(GameObject waypoint);

    /// <summary>
    /// Sets the keys used for the experiment and the waypoint
    /// </summary>
    /// <param name="myVals">Array of float containing the parameters</param>
    /// <param name="waypointCircuit">WaypointCircuit that will be used for the experiment</param>
    protected void writeKeysOnDMC(float[] myVals, GameObject waypointCircuit)
    {
        // if the variables are empty, is assigns them
        dmc = (dmc != null ? dmc : GetComponent<droneMovementController>());
        wpt = (wpt != null ? wpt : GetComponent<WaypointProgressTracker>());

        // initializing new PIDs
        yPID = new PID(myVals[0], myVals[1], myVals[2], myVals[3]);
        zPID = new PID(myVals[4], myVals[5], myVals[6], myVals[7]);
        xPID = new PID(myVals[4], myVals[5], myVals[6], myVals[7]);
        yawPID = new PID(myVals[8], myVals[9], myVals[10], myVals[11]);
        rollPID = new PID(myVals[12], myVals[13], myVals[14], myVals[15]);
        pitchPID = new PID(myVals[12], myVals[13], myVals[14], myVals[15]);

        // calls the functions in the droneMovementController to set the keys and the constants
        dmc.setKs(yPID, zPID, xPID, pitchPID, rollPID, yawPID);
        dmc.setConsts(myVals[16], myVals[17], myVals[18], myVals[19], myVals[20], myVals[22], myVals[23]);

        // spawns the WaypointCircuit and assign it to the drone
        Vector3 wpcPosition = this.transform.position + new Vector3(2, 6, 2);
        spawnedWaypoint = (GameObject)Instantiate(waypointCircuit, wpcPosition, Quaternion.identity);
        wpt.setWaypoint(spawnedWaypoint.GetComponent<WaypointCircuit>());
    }

}
