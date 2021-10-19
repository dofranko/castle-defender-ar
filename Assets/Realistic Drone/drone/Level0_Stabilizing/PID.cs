using UnityEngine;
using System.Collections;

public class PID
{
    // ID. Used just for debugging
    static int id = 0;
    int myId = 0;

    // PID constants. Set them in the constructor
    float kP;
    float kI;
    float kD;
    float kU;

    float lastErr = 0;
    float totErr = 0;
    float absError = 0;
    /// <summary>
    /// Gets the overall error accumulated by the PID
    /// </summary>
    /// <returns>Total error accumulated by the PID</returns>
    public float getTotError() { return absError; }

    /// <summary>
    /// Constructor of the PID class
    /// </summary>
    /// <param name="kP">Proporcional constant</param>
    /// <param name="kI">Integrative constant</param>
    /// <param name="kD">Derivative constant</param>
    /// <param name="kU">Constant multiplied for the output error</param>
    public PID(float kP, float kI, float kD, float kU)
    {
        myId = id++;
        this.kP = kP;
        this.kI = kI;
        this.kD = kD;
        this.kU = kU;

        trashBuffer = new float[4];
    }

    float[] trashBuffer;

    /// <summary>
    /// Gets the PID output
    /// </summary>
    /// <param name="Err">Actual error measured by the sensors</param>
    /// <param name="deltaTime">Time from the last request</param>
    /// <returns>The output of the PID, multiplied for the constant U</returns>
    public float getU(float Err, float deltaTime) { return getU(Err, deltaTime, trashBuffer); }

    float SampleTime = 0.05f;
    float accDTime = 0;
    float lastU = 0;
    /// <summary>
    /// Gets the PID output
    /// </summary>
    /// <param name="Err">Actual error measured by the sensors</param>
    /// <param name="deltaTime">Time from the last request</param>
    /// <param name="values">Array used to store the results of the PID. Used for debugging</param>
    /// <returns>The output of the PID, multiplied for the constant U</returns>
    public float getU(float Err, float deltaTime, float[] values)
    {
        // if not enough time is passed since the last call it returns the old result 
        if (deltaTime + accDTime > SampleTime)
        {
            accDTime = deltaTime + accDTime - SampleTime;

            //calculating P factor
            float pFactor = Err;
            //calculating D factor
            float dFactor = (Err - lastErr);
            lastErr = Err;
            totErr += Err;
            //calculating I factor
            float iFactor = totErr;
            absError += Mathf.Abs(Err);

            // calculating the output of the PID
            float u = (kP * pFactor + kD / SampleTime * dFactor + kI * SampleTime * iFactor) * kU;
            
            //storing the results in the array passed as parameter
            values[0] = kP * pFactor;
            values[1] = kI * iFactor;
            values[2] = kD * dFactor;
            values[3] = kU * u;

            lastU = u;
            return u;
        }
        else
        {
            //just increases the timer and returns the last result calculated
            accDTime += deltaTime;
            return lastU;
        }
    }

}
