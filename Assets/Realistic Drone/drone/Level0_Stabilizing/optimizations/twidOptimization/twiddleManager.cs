using UnityEngine;
using System.Collections;

public class twiddleManager : OptimizationManager {

    public GameObject originalIndividual;
    int actualGeneration = 0;

    public float startingTime;
    public float actualTime;
    public float maxExpTime;
    public float step;

    public GameObject[] experiments;

    public float[] myVals;
    public float[] potencialChanges;
     


    /// <summary>
    /// Initializes drones and circuits for a new experiment. Once they'll finish they'll send back the data
    /// </summary>
    void startExperiment()
    {
        Vector3 initialPosition = transform.position;
        foreach(GameObject g in experiments)
        {
            GameObject tmp = (GameObject)Instantiate(originalIndividual, initialPosition, Quaternion.identity);
            twiddleBehaviour twiddleB = tmp.GetComponent<twiddleBehaviour>();
            activeDrone(twiddleB);
            twiddleB.setKeys(myVals, g);
            twiddleB.setFather(this);
            tmp.transform.SetParent(transform);

            initialPosition += Vector3.forward * 75;
        }

        actualGeneration++;
        Debug.Log("Experiment " + actualGeneration + " started");
    }

    bool resObtained = false;
    float tmpFitness = 0;
    int expCounter = 0;
    /// <summary>
    /// Register the result of the experiment to the manager class.
    /// </summary>
    /// <param name="fitness">Fitness of the drone obtained from the experiment</param>
    /// <param name="naturalDeath">Boolean that indicates if the drone died because the time has expired or because it get to far from the path.</param>
    public override void setExperimentResult(float fitness, bool naturalDeath) {
        if (expCounter == 0) tmpFitness = 0;

        tmpFitness += (naturalDeath ? fitness : fitness / 50f);
        expCounter++;

        if (expCounter == experiments.Length) { resObtained = true; expCounter = 0; ds.addLine(new float[] { tmpFitness }); }
    }

    public string PathFileToRead;
    public bool readFile = false;
    public bool autoSave = false;
    public string folder;

    public bool save = false;
    dataSaver ds;

    /// <summary>
    /// Function called before of the first update
    /// </summary>
    void Start()
    {
        twiddleSettings.timelimit = startingTime;
        twiddleSettings.step = step;
        twiddleSettings.maxTime = maxExpTime;

        ds = new dataSaver("TwiddleTrend", new string[] {"overallFitness"});
    }

    int indxSaving = 0;
    /// <summary>
    /// Save on file a parameters+changes configuration
    /// </summary>
    /// <param name="stats">array of parameters</param>
    /// <param name="pc">array of potencial changes</param>
    void SaveStats(float[] stats, float[] pc) {
        string[] attributes = new string[myVals.Length];
        for (int i = 0; i < myVals.Length; i++)
            attributes[i] = "Attr" + (i + 1).ToString();

        dataSaver ds = new dataSaver(folder + "\\Stat" + indxSaving + ".fit #" + bestFitness + "#", attributes);
        ds.addLine(stats);
        ds.addLine(pc);
        ds.saveOnFile();
        indxSaving++;
    }

    /// <summary>
    /// Function called each frame
    /// </summary> 
    void Update()
    {
        if (save) { save = false; ds.saveOnFile(); }

        // if the boolean 'readFile' is true, we substitute the array setted in the editor with the one in the file
        // in this way is possible to continue the training until a certain point and later start from that point
        if (readFile) {
            float[,] t = dataReader.readFile(folder + "\\" + PathFileToRead);
            if (t == null) { Debug.Log("Check the filename"); readFile = false; return; }

            for (int i = 0; i <= t.GetUpperBound(1); i++)
                myVals[i] = t[0,i];
            for (int i = 0; i <= t.GetUpperBound(1); i++)
                potencialChanges[i] = t[1,i];
            
            Debug.Log("file readed");
            readFile = false;
            twiddleStep = -2;
            iPC = 0;
        }
        else if (twiddle()) Debug.Log("Experiment finished");
    }

    float bestFitness = -50f;
    float threshold = 0.0000001f;
    float minimumValueToCorrect = 0.000001f;
    int twiddleStep = -2;
    int iPC = 0;

    /// <summary>
    /// twiddle function. It is called every frame.
    /// <para>It is composed of differents steps, each one manage one part of the algorithm</para>
    /// </summary>
    /// <returns>Returns true if the process is completed, otherwise fale</returns>
    bool twiddle()
    {
        float sumPC = 0;
        foreach (float f in potencialChanges) sumPC += Mathf.Abs(f);
        if (sumPC < threshold) return true;       

        // periodically save the configuration
        if (autoSave && iPC % myVals.Length == 0 && twiddleStep == 0)
        {
            SaveStats(myVals, potencialChanges);
            Debug.Log("Saved Individual on file " + iPC);
        }
        
        iPC %= myVals.Length;
        int startIndx = iPC;
        // moves to the next value that has to be modified. If there is no-one the process finish
        while (Mathf.Abs(potencialChanges[iPC]) < minimumValueToCorrect)
        {
            iPC = (iPC + 1) % myVals.Length;
            if (iPC == startIndx)
                return true;
        }

        switch (twiddleStep)
        {
            case -2:
                // FIRST experiment. It is needed to calculate the initial fitness of the individual
                Debug.Log("Launched experiment -1 ");
                startExperiment();
                twiddleStep++;
                break;
            case -1:
                if (resObtained)    // wait until the experiment ends
                {
                    // associates the bestFitness with the result obtained and go on
                    Debug.Log("obteined results of exp -1, initial fitness [" + tmpFitness + "]");
                    resObtained = false;
                    bestFitness = tmpFitness;
                    twiddleStep = 0;                   
                }
                break;
            case 0:
                // sum the value of potencial changes to the parameter taken in consideration
                actualTime = twiddleSettings.increaseTimeLimit();
                Debug.Log("twiddle started on indx " + iPC + "; Experiments duration ->" + actualTime);                
                myVals[iPC] += potencialChanges[iPC];
                startExperiment();
                twiddleStep++;
                break;
            case 1:
                if (resObtained)    // wait until the experiment ends
                {                    
                    resObtained = false;
                    if (tmpFitness > bestFitness)   // There was some improvement. Increase this PC and move on next 
                    {
                        Debug.Log("IMPROVED  [" + tmpFitness + "<->" + bestFitness + "]. increase & next iPC");
                        bestFitness = tmpFitness;
                        potencialChanges[iPC] *= 1.1f;
                        iPC++;
                        twiddleStep = 0;
                    }
                    else    // no improvement. Try on the other direction
                    {
                        Debug.Log("no improvements  [" + tmpFitness + "<->" + bestFitness + "]. revert & repeat experiment");
                        myVals[iPC] -= 2 * potencialChanges[iPC];
                        startExperiment();
                        twiddleStep++;                        
                    }
                }
                break;
            case 2:
                if (resObtained)    // wait until the experiment ends
                {                    
                    resObtained = false;
                    if (tmpFitness > bestFitness)   // There was some improvement. Increase this PC and move on next 
                    {
                        Debug.Log("IMPROVED  [" + tmpFitness + "<->" + bestFitness + "]. increase & next iPC");
                        bestFitness = tmpFitness;
                        potencialChanges[iPC] *= 1.1f;
                    }
                    else    // no improvement. Adjust val and reduce this PC
                    {
                        Debug.Log("no improvements  [" + tmpFitness + "<->" + bestFitness + "]. reset changes & next iPC");
                        myVals[iPC] += potencialChanges[iPC];
                        potencialChanges[iPC] *= 0.9f;
                    }
                    // we move to the next index
                    iPC++;
                    //every time we finish a complete round we recalculate the actual values to update the bestFitness at the current experiment length
                    twiddleStep = (iPC == myVals.Length ? -2 : 0);
                }
                break;
        }
        return false;
    }

}
