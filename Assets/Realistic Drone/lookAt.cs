using UnityEngine;
using System.Collections;

public class lookAt : MonoBehaviour {

    public Transform target;
    public Vector3[] positions;
    int indx = 0;

	// Use this for initialization
	void Start () {
        if (positions.Length == 0) positions = new Vector3[] { transform.position };
        transform.position = positions[indx];
	} 
	
	// Update is called once per frame
	void Update () {
        

        if (Input.GetKeyDown(KeyCode.C))
        {
            indx = (indx + 1) % positions.Length;
            transform.position = positions[indx];
        }

        transform.LookAt(target.position);
    }
}
