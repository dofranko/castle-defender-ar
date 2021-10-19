using UnityEngine;
using System.Collections;
using System;
using System.IO;

/// <summary>
/// Class used to save values to file
/// <para>Works with <seealso cref="dataReader"/></para>
/// </summary>
public class dataSaver {

    string[] attributes;
    float[,] values;
    string filename;

    public int counter = 0;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="filename">Name of the file</param>
    /// <param name="attributes">Array of strings containing the name of the attributes</param>
    public dataSaver(string filename, string[] attributes)
    {
        this.filename = filename;
        this.attributes = attributes;
        values = new float[50000, attributes.Length];
    }

    /// <summary>
    /// Add a line to the matrix that will be saved in the file
    /// </summary>
    /// <param name="valLine">array of float conaining the data</param>
    public void addLine(float[] valLine) { for (int i = 0; i < valLine.Length; i++) values[counter , i] = valLine[i]; counter++; }
    
    /// <summary>
    /// Save the data in a file in the desktop.
    /// </summary>
    public void saveOnFile()
    {
        string[] writeThis = new string[counter + 1];
        string tmp = "";

        //Debug.Log(counter);

        foreach (string s in attributes)
            tmp += (s + ", ");
        //tmp += Environment.NewLine;
        writeThis[0] = (string) tmp.Clone();


        for (int r = 1; r <= counter; r++)
        {
            tmp = "";
            for (int c = 0; c <= values.GetUpperBound(1); c++)
                tmp += values[r - 1, c] + ", ";
            
            writeThis[r] = (string)tmp.Clone();
        }

        string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + filename + ".csv";        
        // This text is added only once to the file.
        if (!File.Exists(path))
        {            
            File.WriteAllLines(path, writeThis);
        }
            
        else
        {
            File.Delete(path);
            File.WriteAllLines(path, writeThis);
        }
            

    }
}
