using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlacementScript : MonoBehaviour
{

    public GameObject castleToPlace;
    public GameObject placementIndicatorPrefab;
    public GameObject playerUI;
    public GameObject weapon;

    public event System.EventHandler OnCastleSpawn;

    private GameObject placementIndicator;
    private ARRaycastManager arRaycastManager;
    private Pose placementPose;
    private EnemySpawner enemySpawner;
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
                Instantiate(castleToPlace, placementPose.position, placementPose.rotation);
                OnCastleSpawn?.Invoke(this, System.EventArgs.Empty);

                Destroy(placementIndicator);
                placementIndicator = null;
                Instantiate(weapon);

                ARPlaneManager arpm = GetComponent<ARPlaneManager>();
                arpm.planePrefab = null;
                arpm.SetTrackablesActive(false);

                playerUI.SetActive(true);

                enabled = false;
            }
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }
}
