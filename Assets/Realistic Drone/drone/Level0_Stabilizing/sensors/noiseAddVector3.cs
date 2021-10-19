using UnityEngine;
using System.Collections;

public class noiseAddVector3 {

    noiseAdder xN;
    noiseAdder yN;
    noiseAdder zN;
     
    /// <summary>
    /// Constructor of the class
    /// <para>Just create a noiseAdder class for each element of the vector</para>
    /// </summary>
    public noiseAddVector3()
    {
        xN = new noiseAdder();
        yN = new noiseAdder();
        zN = new noiseAdder();      
    }

    /// <summary>
    /// Obtain the noise using the value passed as parameter and the past values
    /// </summary>
    /// <param name="val">Actual Vector3 which we need to apply the noise to</param>
    /// <returns></returns>
    public Vector3 getNoise(Vector3 val) { return new Vector3(xN.getNoise(val.x), yN.getNoise(val.y), zN.getNoise(val.z)); }
}
