using UnityEngine;
using System.Collections;

public class lineDrawer : MonoBehaviour {

    public int initialPosition;
    public int lengthOfLineRenderer = 20;
    private int ticket;
    private LineRenderer lr;

    /// <summary>
    /// Function called before of the first update
    /// </summary>
    void Start() {        
        lr = gameObject.GetComponent<LineRenderer>();
        lr.SetVertexCount(lengthOfLineRenderer);

        ticket = initialPosition;
         
        int t = initialPosition;
        while (t < lengthOfLineRenderer)
        {
            lr.SetPosition(t, new Vector3());
            t++;
        }
    }

    /// <summary>
    /// Assign a ticket on the lineRender component. It can be used to print a line
    /// </summary>
    public int getTicket() { int t = ticket; ticket += 2; return t; }

    /// <summary>
    /// Function used to draw a line on the lineRenderer
    /// </summary>
    /// <param name="ticket">The ticket is received at the registration moment and it is associated to a particular line</param>
    /// <param name="point">The final point of the line we want to print</param>
    public void drawPosition(int ticket, Vector3 point) { if (lr == null) return; lr.SetPosition(ticket, point); }

}

