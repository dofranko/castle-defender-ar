using UnityEngine;
using System.Collections.Generic;
 
public class GAmanager : OptimizationManager {

    public GameObject originalIndividual;
    public GameObject waypointCircuit;
    private GameObject[] population;

    public float distanceBetweenClones = 5f;
    public int rowLength;
    private Vector3 storeInitialPos;
    public Vector3 nextPosition;

    /// <summary>
    /// Creates the first generation of the drones, initializing randomly the population
    /// </summary>
    void initializeRndPopulation()
    {
        initializePopulationVector(true);

        for (int i = 0; i < GeneticSettings.maxPopulation; i++)
        {
            population[i] = (GameObject)Instantiate(originalIndividual, nextPosition, Quaternion.identity);
            nextPosition += ((i + 1) % rowLength == 0 ? 
                new Vector3(storeInitialPos.x - nextPosition.x, 0, -distanceBetweenClones) : 
                new Vector3(distanceBetweenClones, 0, 0));

            activeDrone(population[i].GetComponent<geneticBehaviour>());
            if (readFile)
            {
                float[,] t = dataReader.readFile(folder + "\\" + PathFileToRead);
                if (t == null) { Debug.Log("Check the filename"); readFile = false; return; }

                float[] vals = new float[t.GetUpperBound(1) + 1];
                for (int k = 0; k < vals.Length; k++)
                    vals[k] = t[0, k];

                float[] changes = new float[t.GetUpperBound(1) + 1];
                for (int k = 0; k < changes.Length; k++)
                    changes[k] = t[1, k];

                //Debug.Log("file readed");

                population[i].GetComponent<geneticBehaviour>().setInitialKs(waypointCircuit, vals, changes);
            }
            else
                population[i].GetComponent<geneticBehaviour>().setInitialKs(waypointCircuit);
            
            population[i].GetComponent<geneticBehaviour>().setFather(this);
            population[i].transform.SetParent(transform);
            nOfSonsAlive++;
        }
        nextPosition = storeInitialPos;
        step = -1;
        actGeneration = 1;
    }

    /// <summary>
    /// Initializes the array containing all the drones
    /// </summary>
    /// <param name="firstTime">Indicates if it is the first call or it just has to be emptied</param>
    void initializePopulationVector(bool firstTime)
    {
        if (!firstTime) population.Initialize();
        population = new GameObject[GeneticSettings.maxPopulation];
        nOfSonsAlive = 0;            
        rowLength = (int)Mathf.Sqrt(GeneticSettings.maxPopulation);
    }

    /// <summary>
    /// Use the stored genes, that come from the list filled up with the results crossing of the genes of the best individual.
    /// <para>To speed up the process it initializes a group of drones for each call.
    /// Initializing them all toghether would slow down the simulation, while initializing them one by one would need too much time</para>
    /// </summary>
    /// <param name="howMany">Number of drones that has to be initialized in this frame</param>
    /// <returns>returns true if the max number of drones has been reached, while it return false if not</returns>
    bool generatePopulationFromStoredGenes(int howMany)
    {
        if (howMany == 0) return false;

        if(nOfSonsAlive < GeneticSettings.maxPopulation && ksList.Count > 0)
        {
            float[] ktmp = ksList.First.Value;
            ksList.RemoveFirst();

            population[nOfSonsAlive] = (GameObject)Instantiate(originalIndividual, nextPosition + Vector3.up * 5, Quaternion.identity);
            nextPosition += ((nOfSonsAlive + 1) % rowLength == 0 ?
                    new Vector3(storeInitialPos.x - nextPosition.x, 0, -distanceBetweenClones) :
                    new Vector3(distanceBetweenClones, 0, 0));

            activeDrone(population[nOfSonsAlive].GetComponent<geneticBehaviour>());
            population[nOfSonsAlive].GetComponent<geneticBehaviour>().setKeys(ktmp, waypointCircuit);
            population[nOfSonsAlive].GetComponent<geneticBehaviour>().setFather(this);
            population[nOfSonsAlive].transform.SetParent(transform);

            nOfSonsAlive++;
            return generatePopulationFromStoredGenes(howMany - 1);
        }

        hasReachedMaximum = false;
        nextPosition = storeInitialPos;
        return true;
    }

    // list containing the genes for the next population
    LinkedList<float[]> ksList = new LinkedList<float[]>();
    bool hasReachedMaximum = false;
    /// <summary>
    /// Add a set of keys to the list containing the genes used to initialize the next population
    /// </summary>
    /// <param name="ks">Set of keys that will be added to the list</param>
    void storeSons(float[] ks) { ksList.AddFirst((float[]) ks.Clone()); if (ksList.Count > GeneticSettings.maxPopulation + 2) hasReachedMaximum = true; }

    dataSaver ds;
    public bool save;
    /// <summary>
    /// initialize the dataSaver instances so it'll be possible to save the genes and restart the simulation from a specific situation
    /// </summary>
    void initializeds()
    {
        // this dataSaver will be used to see the fitness trend of the drones in different populations
        ds = new dataSaver("geneticData" ,new string[] {"lifetime", "fitness"});
    }

    /// <summary>
    /// Add an array of floats to the first dataSaver
    /// </summary>
    /// <param name="data">array of floats containing the lifetime of a drone, its fitness and the constants of a single PID. </param>
    public void addDataToDs(float[] data) { ds.addLine(data); }
    
    int indxSaving = 0;

    /// <summary>
    /// Save a set of parameters to a file.
    /// <para>This function is used to save the parameters of the best individuals</para>
    /// </summary>
    /// <param name="keys">array of parameters used for an experiment</param>
    /// <param name="fitness">fitness obtained with that set of keys</param>
    void SaveStats(float[] keys, float fitness)
    {
        if (!autoSave) return;

        float[] stats = new float[keys.Length + 1];
        stats[0] = fitness;
        for (int i = 1; i < stats.Length; i++)
            stats[i] = keys[i - 1];
        
        dataSaver ds = new dataSaver(folder + "\\Stat" + indxSaving + ".gen " + actGeneration + ".fit #" + fitness + "#", attributes);
        ds.addLine(stats);
        ds.saveOnFile();
        indxSaving++;
    }

    public string PathFileToRead;
    public bool readFile = false;
    public bool autoSave = false;
    public string folder;
    string[] attributes;

    /// <summary>
    /// Function called before of the first update, used to initialize
    /// </summary>
    void Start()
    {
        storeInitialPos = nextPosition;
        step = -2;
        Random.seed = (int)System.DateTime.Now.Ticks;
        initializeRndPopulation();
        initializeds();
        timeStepLevel = GeneticSettings.timelimit;        
        sons = new geneticBehaviour[transform.childCount];
        fitnesses = new float[transform.childCount];

        int nOfAttr = transform.GetChild(0).GetComponent<geneticBehaviour>().getKeys().Length;
        attributes = new string[nOfAttr + 1];
        attributes[0] = "fitness";
        for (int i = 1; i < nOfAttr; i++)
            attributes[i] = "Attr" + (i + 1).ToString();
    }

    int nOfSonsAlive = 0;

    /// <summary>
    /// Register the result of the experiment to the manager class.
    /// </summary>
    /// <param name="fitness">Fitness of the drone obtained from the experiment</param>
    /// <param name="naturalDeath">Boolean that indicates if the drone died because the time has expired or because it get to far from the path.</param>
    public override void setExperimentResult(float fitness, bool naturalDeath) { /* Not used in this optimization */ }

    float timeStepLevel = 0;
    int actualLevel = 1;
    int generationsEachLevel = 5;
    int actGeneration = 0;

    int step = -1;
    geneticBehaviour[] sons;
    float[] fitnesses;
    int fillIndx = 0;
    geneticBehaviour[] survivedSons;
    geneticBehaviour[] bestIndividual;

    /// <summary>
    /// Function used to store the sons once they die.
    /// </summary>
    /// <param name="ge">GeneticBehaviour containing the information we want to store</param>
    public void storeDataAboutIndividualLife(geneticBehaviour ge) { if (ge == null) return; sons[fillIndx] = ge; fitnesses[fillIndx] = ge.fitness(); fillIndx++; }

    /// <summary>
    /// Called once per frame. In this section is stored the algorithm that manage the population
    /// </summary>
    void Update()
    {
        if (save) { save = false; ds.saveOnFile();}
       

        switch (step)
        {
            case -1:
                // the population has been created. Now we have to wait until all of them die
                if (transform.childCount == 0)
                {
                    step++;
                    Debug.Log("-1 passed - All sons have lived until their limit and sent the data -");
                }
                else if (transform.childCount < 4)
                {
                    foreach (geneticBehaviour g in GetComponentsInChildren<geneticBehaviour>())
                        g.fatherSayIHaveToSendMyData = true;
                    step++;
                    Debug.Log("-1 passed - most of the sons died, saved the most adapted to the experiment -");
                }
                break;
            case 0:
                // in this step we sort the individuals, accordingly to their fitness
                // in this way it'll be easy to extract the best ones
                survivedSons = new geneticBehaviour[fillIndx];
                float[] fitnessesOfSurvived = new float[fillIndx];

                for (int k = 0; k < fillIndx; k++)
                {
                    survivedSons[k] = sons[k];
                    fitnessesOfSurvived[k] = fitnesses[k];
                }
                System.Array.Sort(fitnessesOfSurvived, survivedSons);
                step++;
                Debug.Log("0 passed - Sons are stored and sorted -");
                break;
            case 1:
                // best individuals are selected and saved to another vector
                int size = Mathf.Min(GeneticSettings.numberOfBest, fillIndx);
                bestIndividual = new geneticBehaviour[size];
                int diff = fillIndx - size;
                for (int k = 0; k < bestIndividual.Length; k++)
                    bestIndividual[k] = survivedSons[k + diff];
                step++;
                Debug.Log("1 passed - best sons have been selected -");
                break;
            case 2:
                // best individuals are saved to file for analysis and put in the list so they'll be respawned in the next generation
                int size2 = Mathf.Min(GeneticSettings.reinjectedIndividual, fillIndx);
                for (int i = bestIndividual.Length - size2; i < bestIndividual.Length; i++)
                {
                    storeSons(bestIndividual[i].getKeys());                   
                    SaveStats(bestIndividual[i].getKeys(), bestIndividual[i].fitness());
                    Debug.Log("generatedReinjectedSon");
                }
                step++;
                Debug.Log("2 passed - elité sons have been reinjected in the birth-list and saved -");
                break;
            case 3:
                // now we generate new sons using randomly selected individuals
                while (!hasReachedMaximum)
                {
                    Random.seed = (int)System.DateTime.Now.Ticks;
                    geneticBehaviour A = survivedSons[Random.Range(0, survivedSons.Length)];
                    geneticBehaviour B = survivedSons[Random.Range(0, survivedSons.Length)];

                    if ((A.getID() != B.getID() || bestIndividual.Length == 1) && A.numberOfSonsCreated < GeneticSettings.maxNumberOfSons)
                    {
                        storeSons(A.cross(B.getKeys()));
                        Debug.Log("generatedNewSon");
                    }
                }                    
                step++;
                Debug.Log("3 passed - the rest of the population has been queued in the birth-list -");
                break;
            case 4:
                // reinitializing variables for the next generation
                nextPosition = storeInitialPos;
                initializePopulationVector(false);
                if (actGeneration % generationsEachLevel == 0)
                {
                    GeneticSettings.timelimit = timeStepLevel * ++actualLevel;
                    timeStepLevel += 0.25f;
                    GeneticSettings.timelimit = (GeneticSettings.timelimit <= GeneticSettings.maxTime ? GeneticSettings.timelimit : GeneticSettings.maxTime);
                }
                    
                step++;
                Debug.Log("4 passed - inizialized variables and setted time -");
                break;
            case 5:
                // regenerating the population, eventually changing the experiment duration and setting the step to -1 so the process restart from the beginning
                if (generatePopulationFromStoredGenes(20))
                {
                    fillIndx = 0;
                    sons = new geneticBehaviour[transform.childCount];
                    fitnesses = new float[transform.childCount];
                    actGeneration++;
                    step = -1;
                    Debug.Log("5 passed - new population has been generated. Next experiment duration:" + GeneticSettings.timelimit + " -");
                }                    

                break;
        }

    }




}
