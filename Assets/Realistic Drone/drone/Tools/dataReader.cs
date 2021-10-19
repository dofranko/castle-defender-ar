using System;
using System.IO;

/// <summary>
/// Class used to read values from file
/// <para>Works with <seealso cref="dataSaver"/></para>
/// </summary>
public class dataReader
{
    /// <summary>
    /// Reads data from a file and put it in a matrix of float
    /// </summary>
    /// <param name="filename">Name of the file we want to read</param>
    /// <returns></returns>
    public static float[,] readFile(string filename)
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + filename + ".csv";
        string[] buffer;
        

        if (File.Exists(path))
        {            
            buffer = File.ReadAllLines(path);
        }            
        else
            return null;

        //reads line 1
        string[] stringRes = buffer[1].Split(',');
        //reads line 2
        string[] stringPC = buffer[2].Split(',');
        float[,] res = new float[2 ,stringRes.Length - 1];

        for (int i = 0; i <= res.GetUpperBound(1); i++)
            res[0, i] = float.Parse(stringRes[i]);
        for (int i = 0; i <= res.GetUpperBound(1); i++)
            res[1, i] = float.Parse(stringPC[i]);

        return res;
    }

 
}