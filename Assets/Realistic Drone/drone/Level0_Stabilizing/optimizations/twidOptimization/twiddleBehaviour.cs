using UnityEngine;
using System.Collections;
using System;

public class twiddleBehaviour : OptimizationBehaviour {

    public bool hidebody;

    /// <summary>
    /// Gets the fitness of the individual
    /// </summary>
    public override float fitness()
    {
        float f6 = distanceMade * 15f;
        float f7 = -accumulatedDistanceFromRoutePosition / 4;
        return GeneticSettings.normalizeFitness(f6 + f7);
    }

    /// <summary>
    /// Function called before of the first update
    /// </summary>
    void Start()
    {
        startOperations(twiddleSettings.timelimit, hidebody, GetComponent<WaypointProgressTracker>().getRoutePosition());
    }

    /// <summary>
    /// Function called each frame
    /// </summary>
    void Update()
    {
        updateOperations(GetComponent<WaypointProgressTracker>().getRoutePosition());
        if (hasToBeKilled()) { die(false); }
        if (isOldDead()) { die(true); }
    }

    /// <summary>
    /// Kill the individual
    /// </summary>
    /// <param name="naturalDeath">indicates if the drone died for natural death or not</param>
    public void die(bool naturalDeath)
    {

        father.setExperimentResult(fitness(), naturalDeath);
        Destroy(spawnedWaypoint);
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Sets the keys used for the experiment and the waypoint
    /// </summary>
    /// <param name="ks">Array of float containing the parameters</param>
    /// <param name="waypoint">WaypointCircuit that will be used for the experiment</param>
    public override void setKeys(float[] ks, GameObject waypointC)
    {
        writeKeysOnDMC(ks, waypointC);
    }

    /// <summary>
    /// [NOT USED IN twiddle]
    /// <para> Special case where the keys are not setted by the manager, 
    /// they are calculated more or less randomly by the same individual </para>
    /// </summary>
    /// <param name="waypoint">WaypointCircuit that will be used for the experiment</param>
    public override void setInitialKs(GameObject waypoint)
    {
        Debug.Log("Something is wrong. You shouldn't be here!");
    }

    /// <summary>
    /// returns the keys used for the experiment [NOT USED IN twiddle]
    /// </summary>
    public override float[] getKeys()
    {
        Debug.Log("Something is wrong. You shouldn't be here!");
        return new float[] { 0 };
    }
}
