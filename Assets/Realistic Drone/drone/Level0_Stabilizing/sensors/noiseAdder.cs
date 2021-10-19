using UnityEngine;
using System.Collections.Generic;

public class noiseAdder {

    float sigma = 1f;

    LinkedList<float> lastValues;
    const int maxNr = 50;
    int filled = 0;
     
    /// <summary>
    /// constructor of noiseAdder class
    /// </summary>
    public noiseAdder()
    {
        lastValues = new LinkedList<float>();
    }

    /// <summary>
    /// Obtain the noise using the value passed as parameter and the past values
    /// </summary>
    /// <param name="val">Actual value which we need to apply the noise to</param>
    /// <returns></returns>
    public float getNoise(float val) {
        if (filled < maxNr)
        {
            lastValues.AddFirst(val);
            filled++;
        }
        else
        {
            lastValues.AddFirst(val);
            lastValues.RemoveLast();
        }

        // we calculate the average and the sigma, to obtain a normalized value
        float average = 0;
        foreach (float f in lastValues)
            average += f;
        average /= lastValues.Count;
        sigma = (val - average) * (val - average);
        sigma = droneSettings.keepOnRange(sigma, 0, 2f);

        return val + sigma * gaussianFloatBetween1_1() * 0.1f;
    }

    /// <summary>
    /// Return a floating point value, using a normal distribution, in the range [-1,1]
    /// </summary>
    private float gaussianFloatBetween1_1() {
        float f = gaussianFloat();
        f = (!droneSettings.isInsideRange(f, -3, 3) ? 0 : f);
        return droneSettings.normalizeBetween(f, -3, 3) - 0.5f;        
    }

    /// <summary>
    /// Obtain a number using a normal distribution
    /// </summary>
    /// <returns></returns>
    private float gaussianFloat()
    {
        float u, v, S;
        do
        {
            u = 2.0f * Random.Range(0f, 1f) - 1.0f;
            v = 2.0f * Random.Range(0f, 1f) - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0);

        float fac = Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);
        return u * fac;
    }

}
