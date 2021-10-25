using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class Placement : MonoBehaviour
{

    public event System.EventHandler OnPlace;
    public GameObject castleToPlace;
    public GameObject placementIndicatorPrefab;
    //public GameObject crosshairImage;
    public float CastleFloorPositionY { get; private set; } = Mathf.Infinity;
    public event System.EventHandler OnCastleSpawn;

    private GameObject placementIndicator;
    private ARRaycastManager arRaycastManager;
    private Pose placementPose;
    void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();

    }

    void Start()
    {
        placementIndicator = Instantiate(placementIndicatorPrefab, new Vector3(), new Quaternion());
        placementIndicator.SetActive(false);
    }


    void Update()
    {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);
        if (hits.Count > 0)
        {
            placementPose = hits[0].pose;
            var cameraForward = Camera.main.transform.forward;
            placementPose.rotation = Quaternion.LookRotation(new Vector3(cameraForward.x, 0, cameraForward.z).normalized);
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                var meshFilter = castleToPlace.GetComponentInChildren<MeshFilter>();
                var mesh = meshFilter.sharedMesh;
                Vector3 bottom = new Vector3();
                if (mesh)
                    bottom = new Vector3(0, Mathf.Abs(mesh.vertices.Min(v => v.y)) / meshFilter.transform.localScale.y, 0);

                Instantiate(castleToPlace, placementPose.position + bottom, placementPose.rotation);
                OnCastleSpawn?.Invoke(this, System.EventArgs.Empty); //TODO zamienic na onobjectspawn
                if (CastleFloorPositionY == Mathf.Infinity)
                    CastleFloorPositionY = placementPose.position.y;
                placementIndicator.SetActive(false);

                ///TODO add to game engine (handler or sth)s
                ARPlaneManager arpm = GetComponent<ARPlaneManager>();
                //arpm.planePrefab = null;
                //arpm.SetTrackablesActive(false);
                ///crosshairImage.SetActive(true);

                OnPlace?.Invoke(this, System.EventArgs.Empty);
                enabled = false;
            }
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }
}
