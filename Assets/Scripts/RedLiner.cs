using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedLiner : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    private float castleY;

    void Start()
    {
        var placement = FindObjectOfType<Placement>();
        castleY = placement.CastleFloorPositionY;
        line.enabled = true;
    }
    void Update()
    {
        line.SetPosition(0, new Vector3(transform.parent.position.x, transform.parent.position.y, transform.parent.position.z));
        line.SetPosition(1, new Vector3(transform.parent.position.x, castleY, transform.parent.position.z));
    }
}
