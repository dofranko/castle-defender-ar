using UnityEngine;
using System.Collections;

public class rotor : MonoBehaviour {

    Rigidbody rBody;
    float power;
    
    /// <summary>
    /// Specify the verse of the rotation
    /// <para> Set this in the editor
    /// </summary>
    public bool counterclockwise = false;
    
    /// <summary>
    /// Specify is the rotors are animated or static. Just a visual effect
    /// <para> Set this in the editor
    /// </summary>
    public bool animationActivated = false;

    /// <summary>
    /// Function called before of the first update
    /// </summary>
    void Start()
    {        
        Transform t = this.transform;
        while (t.parent != null && t.tag != "Player") t = t.parent;
        rBody = t.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Sets the rotating power of the rotor
    /// </summary>
    /// <param name="intensity"> The rotating power of the rotor </param>
    public void setPower(float intensity) { power = intensity; }

    /// <summary>
    /// Gets the rotating power of the rotor
    /// </summary>
    /// <returns>the actual rotating power of the rotor</returns>
    public float getPower() { return power; }
    
    /// <summary>
    /// Function called once per frame
    /// </summary>
    void Update() { if (animationActivated) transform.Rotate(0, 0, power * 700 * Time.deltaTime * (counterclockwise ? -1 : 1)); }

    /// <summary>
    /// Function at regular time interval
    /// </summary>
    void FixedUpdate()
    {        
        rBody.AddForceAtPosition(transform.forward * power, transform.position);
        //lr.SetPosition(1, new Vector3(0, 0, power / 3f));
    }
}
