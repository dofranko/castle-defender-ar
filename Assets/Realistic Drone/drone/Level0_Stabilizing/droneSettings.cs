using UnityEngine;
using System.Collections;

public class droneSettings {
    /// <summary>
    /// Values that represent the limits.
    /// <para>It could be because a drone in the physic world wouldn't exceede them
    /// or because go beyond of them means introduce instability</para> 
    /// </summary>
    public static class saturationValues {
        public const float minRotationSpeed = 0.2f;
        public const float maxRotationSpeed = 3f;

        public const float minTorque = 0.2f;
        public const float maxTorque = 3f;

        public const float minVerticalVel = -8f;
        public const float maxVerticalVel = 15f;
        public const float minVerticalAcc = -6f;
        public const float maxVerticalAcc = 3f;

        public const float maxHorizontalVel = 5f;
    }

    /// <summary>
    /// Constants used by the vertical stabilization algorithm
    /// <para>There are the PID constants (P, I, D, U) 
    /// and the constants used to transform the error given to the PID class</para>
    /// </summary>
    #region Vertical_Stabilization

    public static float constVerticalIdealVelocity = 0.385772f;
    public static float constVerticalIdealAcceler = 0.6716582f;

    public const float verticalPID_P = 0.7635331f;
    public const float verticalPID_I = 0.001476288f;
    public const float verticalPID_D = 0.0001088255f;
    public const float verticalPID_U = 0.1f;

    #endregion

    /// <summary>
    /// Constants used by the horizontal stabilization algorithms (X and Z stabilization)
    /// <para>There are the PID constants (P, I, D, U) 
    /// and the constants used to transform the error given to the PID class</para>
    /// </summary>
    #region Axis_Stabilization

    public static float constAxisIdealVelocity = 0.482393f;
    public static float constAxisIdealAcceler = 0.9510251f;

    public const float axisPID_P = 0.2242663f;
    public const float axisPID_I = 6.129676E-05f;
    public const float axisPID_D = 0.007565225f;
    public const float axisPID_U = 0.5f;

    #endregion

    /// <summary>
    /// Constants used by the orientation stabilization algorithms (Yaw stabilization)
    /// <para>There are the PID constants (P, I, D, U) 
    /// and the constants used to transform the error given to the PID class</para>
    /// </summary>
    #region Yaw_Stabilization

    public static float constYawIdealVelocity = 0.5534329f;

    public const float yawPID_P = 0.07649516f;
    public const float yawPID_I = 2.469936E-05f;
    public const float yawPID_D = 0.002099928f;
    public const float yawPID_U = 0.2f;

    #endregion

    /// <summary>
    /// Constant used by the horizontal rotation stabilization algorithms (Roll and Pitch stabilization)
    /// <para>There are the PID constants (P, I, D, U) 
    /// and the constants used to transform the error given to the PID class</para>
    /// </summary>
    #region Horizontal_Stabilization

    public static float constHorizontalIdealVelocity = 0.9380789f;
    public static float constHorizontalIdealAcceler = 0.9398623f;

    public const float orizPID_P = 0.05998019f;
    public const float orizPID_I = 5.116195E-05f;
    public const float orizPID_D = 0.002372454f;
    public const float orizPID_U = 0.3f;

    #endregion


    #region Common_Functions

    /// <summary>
    /// Normalize a number between an upper and a lower bound
    /// </summary>
    /// <param name="n">Number to normalize</param>
    /// <param name="lbound">Lower bound taken in consideration</param>
    /// <param name="ubound">Upper bound taken in consideration</param>
    /// <returns>A number normalized into the given interval</returns>
    public static float normalizeBetween(float n, float lbound, float ubound)
    {
        return (n - lbound) / (ubound - lbound);
    }

    /// <summary>
    /// If the number is inside the bounds it just returns it. If it is outside it return the nearest bound
    /// </summary>
    /// <param name="num">Number that we want to keep inside the range</param>
    /// <param name="lbound">Lower bound taken in consideration</param>
    /// <param name="ubound">Upper bound taken in consideration</param>
    /// <returns>if (num € [lbound,ubound]) -> num, else if (num is less than lbound) -> lbound, else -> ubound </returns>
    public static float keepOnRange(float num, float lBound, float uBound) { return (num > uBound ? uBound : (num < lBound ? lBound : num)); }
    /// <summary>
    /// If the number is inside the absolute bound it just returns it. If it is outside it return the nearest bound
    /// </summary>
    /// <param name="num">Number that we want to keep inside the range</param>
    /// <param name="bound">Absolute bound taken in consideration</param>
    /// <returns>if (num € [-bound,bound]) -> num, else if (num is less than -bound) -> -bound, else -> bound </returns>
    public static float keepOnAbsRange(float num, float bound) { return keepOnRange(num, -bound, bound); }
    /// <summary>
    /// Says if the number is inside the interval specified by the bounds
    /// </summary>
    /// <param name="num">Number that we want to check</param>
    /// <param name="lbound">Lower bound taken in consideration</param>
    /// <param name="ubound">Upper bound taken in consideration</param>
    /// <returns>if (num € [-bound,bound]) -> TRUE, else -> FALSE</returns>
    public static bool isInsideRange(float num, float lBound, float uBound) { return (num >= lBound && num <= uBound); }

    /// <summary>
    /// Sets a number to 0 if it is less than a bound. If not just returns the original number
    /// </summary>
    /// <param name="num">Number that we want to keep greather than 'bound'</param>
    /// <param name="bound">Lower bound taken in consideration</param>
    /// <returns>if (num is greater than bound) -> num, else -> 0s </returns>
    public static float setZeroIflessThan(float num, float bound) { return (Mathf.Abs(num) > bound ? num : 0); }
    
    /// <summary>
    /// Sets the elements of a Vector3 to 0 if they are less than a specified bound
    /// </summary>
    /// <param name="v">Vector3, which elements we want to keep greather than 'bound'</param>
    /// <param name="bound">Lower bound taken in consideration</param>
    /// <returns>foreach(number n in the Vector3) if (n is greater than bound) -> num, else -> 0s </returns>
    public static Vector3 setZeroIflessThan(Vector3 v, float bound) { return new Vector3(setZeroIflessThan(v.x, bound), setZeroIflessThan(v.y, bound), setZeroIflessThan(v.z, bound)); }

    #endregion
}
