using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlacementScript : MonoBehaviour
{

    public GameObject objectToPlace;
    public GameObject placementIndicatorPrefab;
    public GameObject Castle { get; private set; }
    public Vector3 firstLocation;
    public GameObject gameEngine;

    private GameObject placementIndicator;
    private ARRaycastManager arRaycastManager;
    private Pose placementPose;
    private EnemySpawner enemySpawner;
    void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        enemySpawner = gameEngine.GetComponent<EnemySpawner>();
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
                firstLocation = placementPose.position;
                Castle = Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
                enemySpawner.SpawnEnemies(placementPose.position);
            }
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }
}
