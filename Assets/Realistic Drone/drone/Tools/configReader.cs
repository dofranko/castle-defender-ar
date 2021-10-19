using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Class used to read a PID configuration and inject into the drone automatically.
/// <para>Works with <seealso cref="dataReader"/></para>
/// </summary>
public class configReader : MonoBehaviour {

    public bool readFile;
    public string folder;
    public string PathFileToRead;

    /// <summary>
    /// Function called before of the first update
    /// </summary>
    void Start () {

        // if the boolean readFile is true the script reads the configuration and inject it into the drone
        if (readFile)
        {
            string path = folder;
            Debug.Log(path + "\\" + PathFileToRead);
            float[,] t = dataReader.readFile(path + "\\" + PathFileToRead);
            if (t == null) { Debug.Log("Check the filename, " + path + "\\" + PathFileToRead + "- not found"); readFile = false; return; }

            
            float[] myVals = new float[t.GetUpperBound(1) + 1];
            for (int i = 0; i <= t.GetUpperBound(1); i++)
                myVals[i] = t[0, i];

            writeKeysOnDMC(myVals);

            Debug.Log("file readed");
            readFile = false;
        }
    }

    /// <summary>
    /// Sets the keys to the drone
    /// </summary>
    /// <param name="myVals">Array of float containing the parameters</param>
    protected void writeKeysOnDMC(float[] myVals)
    {
        droneMovementController dmc = GetComponent<droneMovementController>();

        PID yPID = new PID(myVals[0], myVals[1], myVals[2], myVals[3]);
        PID zPID = new PID(myVals[4], myVals[5], myVals[6], myVals[7]);
        PID xPID = new PID(myVals[4], myVals[5], myVals[6], myVals[7]);
        PID yawPID = new PID(myVals[8], myVals[9], myVals[10], myVals[11]);
        PID rollPID = new PID(myVals[12], myVals[13], myVals[14], myVals[15]);
        PID pitchPID = new PID(myVals[12], myVals[13], myVals[14], myVals[15]);

        dmc.setKs(yPID, zPID, xPID, pitchPID, rollPID, yawPID);
        dmc.setConsts(myVals[16], myVals[17], myVals[18], myVals[19], myVals[20], myVals[22], myVals[23]);
    }
}
