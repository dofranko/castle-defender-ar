using UnityEngine;
using System.Collections.Generic;

public class dataDrawer : MonoBehaviour {

    public GameObject wallObject;
    public GameObject dronePin;
    GameObject[] dronesPins;

    public float maxMapSize = 200;
    dataCollector.Data data;
    LinkedList<dataCollector.indx> dirtyList;
    /// <summary>
    /// Sets the matrix containing the information that the dataDrawer will use to paint a map
    /// </summary>
    /// <param name="data">Matrix where the information is contained</param>
    public void setData(dataCollector.Data data) { this.data = data; dirtyList = data.getDirtyValuesList(); }
    
    float interval = 0.01f;
    float actualTime = 1f;

    dataCollector dC;
    LineRenderer lr;

    //model used for painting the walls
    Transform wallspinscontainer;
    //model used to show the drone position
    Transform dronepinscontainer;

    /// <summary>
    /// Function called before of the first update. Used to initialize the components.
    /// </summary>
    void Start()
    {
        dC = GetComponent<dataCollector>();

        wallspinscontainer = transform.Find("wallspinscontainer");
        dronepinscontainer = transform.Find("dronepinscontainer");

        lr = gameObject.AddComponent<LineRenderer>();
        lr.SetVertexCount(5);
        Vector3 pt = transform.position;

        // draw the borders
        lr.SetPosition(0, pt);
        lr.SetPosition(1, (pt += Vector3.right * maxMapSize));
        lr.SetPosition(2, (pt += Vector3.forward * maxMapSize));
        lr.SetPosition(3, (pt += Vector3.right * -maxMapSize));
        lr.SetPosition(4, (pt += Vector3.forward * -maxMapSize));

        // initialize the drone-Pins, used to show the drones positions
        dronesPins = new GameObject[10];
        for (int i = 0; i < dronesPins.Length; i++)
        {
            dronesPins[i] = (GameObject)Instantiate(dronePin, new Vector3(), Quaternion.identity * Quaternion.AngleAxis(90, Vector3.right));
            dronesPins[i].transform.parent = dronepinscontainer;
        }
            
    }

    public int nrOfBricks = 0;
    /// <summary>
    /// Function called once per frame
    /// </summary>
    void Update()
    {
        if ((actualTime -= Time.deltaTime) < 0)
        {
            // for each element in the list of recently added values, it spawns a new pin (to represent a wall)
            // and place it in the correct position
            actualTime += interval;
            // to speed up the process is possible to process more than 1 value per execution
            int counter = 10;
            while (dirtyList.Count > 0 && counter > 0)
            {
                GameObject g = (GameObject)Instantiate(wallObject, 
                    transform.position + dirtyList.First.Value.divideFloat(data.getArraySize()) * maxMapSize, 
                    Quaternion.identity * Quaternion.AngleAxis(90, Vector3.right));
                g.transform.parent = wallspinscontainer;
                dirtyList.RemoveFirst();
                counter--;
                nrOfBricks++;
            }

            // update the drones positions in the map
            for (int i = 0; i < dC.ticket; i++)
                dronesPins[i].transform.position = transform.position + dC.dronesPositions[i].divideFloat(data.getArraySize()) * maxMapSize;
        }

    }

}
