using UnityEngine;

public class geneticBehaviour : OptimizationBehaviour {
     
    public bool hidebody;

    public bool fixedValues = false;
    public float[] myVals;
    public float[] valsAlphas;

    /// <summary>
    /// Gets the fitness of the individual
    /// </summary>
    public override float fitness()
    {
        float f1 = distanceMade * 15f;
        float f2 = -accumulatedDistanceFromRoutePosition / 5;
        return GeneticSettings.normalizeFitness(f1 + f2);
    }

    /// <summary>
    /// Function called before of the first update
    /// </summary>
    void Start() {
        startOperations(GeneticSettings.timelimit, hidebody, GetComponent<WaypointProgressTracker>().getRoutePosition());        
    }

    /// <summary>
    /// Function called each frame
    /// </summary>
    void Update() {
        updateOperations(GetComponent<WaypointProgressTracker>().getRoutePosition());
        if (hasToBeKilled()) die(false);
        if (isOldDead() || fatherSayIHaveToSendMyData) { ((GAmanager) father).storeDataAboutIndividualLife(this); die(true); }
    }
    public bool fatherSayIHaveToSendMyData = false;

    /// <summary>
    /// Kill the individual
    /// </summary>
    /// <param name="fullFitness">indicates if the drone died for natural death or not</param>
    public void die (bool fullFitness)
    {
        if (GeneticSettings.hasToBeSaved(id) && fullFitness)
            ((GAmanager)father).addDataToDs(new float[] { getAge(), fitness()} );        
        Destroy(spawnedWaypoint);
        Destroy(this.gameObject);
    }

    /// <summary>
    /// returns the keys used for the experiment
    /// </summary>
    public override float[] getKeys() { return myVals; }

    /// <summary>
    /// Sets the keys used for the experiment and the waypoint
    /// </summary>
    /// <param name="ks">Array of float containing the parameters</param>
    /// <param name="waypoint">WaypointCircuit that will be used for the experiment</param>
    public override void setKeys(float[] ks, GameObject waypoint) {

        myVals = new float[valsAlphas.Length];
        for (int i = 0; i < myVals.Length; i++)
            myVals[i] = ks[i];
        if (Random.Range(0f, 1f) < GeneticSettings.mutationProbability) mutate();
        writeKeysOnDMC(myVals, waypoint);
    }

    /// <summary>
    /// Special case where the keys are not setted by the manager, 
    /// they are calculated more or less randomly by the same individual
    /// </summary>
    /// <param name="waypoint">WaypointCircuit that will be used for the experiment</param>
    public override void setInitialKs(GameObject waypoint)
    {
        if (!fixedValues)
        {
            myVals = new float[valsAlphas.Length];
            for (int i = 0; i < myVals.Length; i++)
                myVals[i] = Random.Range(-1f, 1f);
        }
        else
        {
            for (int i = 0; i < myVals.Length; i++)
            {
                float limit = Mathf.Abs(myVals[i] * valsAlphas[i] * 2);
                myVals[i] += Random.Range(-limit, limit);
            }
                
        }

        writeKeysOnDMC(myVals, waypoint);
    }

    /// <summary>
    /// Sets the keys used for the experiment and the waypoint
    /// </summary>
    /// <param name="ks">Array of float containing the parameters</param>
    /// <param name="waypoint">WaypointCircuit that will be used for the experiment</param>
    /// <param name="changes">array of floats that represents the maximum change of the keys</param>
    public void setInitialKs(GameObject waypoint, float[] vals, float[] changes)
    {
        for (int i = 0; i < myVals.Length; i++)
            myVals[i] = vals[i] + Random.Range(-changes[i], changes[i]);
        writeKeysOnDMC(myVals, waypoint);
    }

    /// <summary>
    /// Genetic cross between the key of the individual and the keys of other one
    /// </summary>
    /// <returns>A new set of keys, resulting from the cross</returns>
    public float[] cross(float[] otherKs)
    {
        numberOfSonsCreated++;
        float[] kTmp = new float[myVals.Length];
        for (int i = 0; i < kTmp.Length; i++)
            kTmp[i] = BLXalpha_cross(myVals[i], otherKs[i], valsAlphas[i]);   
        return kTmp;
    }
    
    /// <summary>
    /// Randomly mutate the keys of the individual
    /// </summary>
    protected void mutate()
    {

        for (int i = 0; i < myVals.Length; i++)
            myVals[i] = BLXalpha_mutation(myVals[i], valsAlphas[i] / 5f);
    }

    public float numberOfSonsCreated = 0;

    /// <summary>
    /// Perform a BLXalpha cross without specifying the alpha (0.3f as default)
    /// </summary>
    /// <param name="A">Value of a gene of the first individual</param>
    /// <param name="B">Value of a gene of the second individual</param>
    /// <returns>Resulting gene, obtained crossing the two values</returns>
    float BLXalpha_cross(float A, float B) { return BLXalpha_cross(A, B, 0.3f); }

    /// <summary>
    /// Perform a BLXalpha cross specifying the alpa
    /// </summary>
    /// <param name="A">Value of a gene of the first individual</param>
    /// <param name="B">Value of a gene of the second individual</param>
    /// <param name="rangeM">Alpha value</param>
    /// <returns>Resulting gene, obtained crossing the two values</returns>
    float BLXalpha_cross(float A, float B, float rangeM)
    {
        float min = System.Math.Min(A, B);
        float max = System.Math.Max(A, B);
        float alpha = Random.Range(0, rangeM * (max - min));
        return Random.Range(System.Math.Max(min - alpha, 0),
                            System.Math.Min(max + alpha, 1));
    }

    /// <summary>
    /// Perform a BLXalpha mutation without specifying the alpha (0.5f as default)
    /// </summary>
    /// <param name="A">Value of the gene to mutate</param>
    /// <returns>Mutated gene</returns>
    float BLXalpha_mutation(float A) { return BLXalpha_mutation(A, 0.5f); }

    /// <summary>
    /// Perform a BLXalpha mutation specifying the alpha value
    /// </summary>
    /// <param name="A">Value of the gene to mutate</param>
    /// <param name="rangeM">Alpha value</param>
    /// <returns>Mutated gene</returns>
    float BLXalpha_mutation(float A, float alpha)
    {
        float k = Random.Range(0, alpha * A);
        return Random.Range(System.Math.Max(A - k, 0),
                            System.Math.Min(A + k, 1));
    }

}
