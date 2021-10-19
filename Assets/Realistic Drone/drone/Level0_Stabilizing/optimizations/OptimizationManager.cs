using UnityEngine;
using System.Collections;

public abstract class OptimizationManager : MonoBehaviour {

    /// <summary>
    /// Register the result of the experiment to the manager class.
    /// </summary>
    /// <param name="fitness">Fitness of the drone obtained from the experiment</param>
    /// <param name="naturalDeath">Boolean that indicates if the drone died because the time has expired or because it get to far from the path.</param>
    public abstract void setExperimentResult(float fitness, bool naturalDeath);

    /// <summary>
    /// Enables the OptimizationBehaviour script passed as argument
    /// <para>This permits to switch off the scripts, so it is possible to use it just if it is necessary.
    /// In this way it won't work if a manager doesn't active it first.</para>
    /// </summary>
    /// <param name="opt">OptimizationBehaviour class that we want to activate</param>
    protected void activeDrone(OptimizationBehaviour opt) { opt.enabled = true; }

}
