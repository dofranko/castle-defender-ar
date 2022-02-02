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

    [SerializeField] private GameObject planeVisualizer;
    [SerializeField] private GameObject planeWithShadows;
    [SerializeField] private ARPlaneManager aRPlaneManager;
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
        if (!PlayerPrefs.HasKey("visualize_planes"))
        {
            aRPlaneManager.planePrefab = planeVisualizer;
        }
        else if (PlayerPrefs.GetInt("visualize_planes") == 1)
        {
            aRPlaneManager.planePrefab = planeVisualizer;
        }

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
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began /*&& !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)*/)
            {
                var meshFilter = castleToPlace.GetComponentInChildren<MeshFilter>();
                var mesh = meshFilter.sharedMesh;
                Vector3 bottom = new Vector3();
                if (mesh)
                    bottom = new Vector3(0, Mathf.Abs(mesh.vertices.Min(v => v.y)) / meshFilter.transform.localScale.y, 0);

                Instantiate(castleToPlace, placementPose.position + bottom, placementPose.rotation);
                OnCastleSpawn?.Invoke(this, System.EventArgs.Empty);
                if (CastleFloorPositionY == Mathf.Infinity)
                    CastleFloorPositionY = placementPose.position.y;
                placementIndicator.SetActive(false);
                if (castleToPlace.GetComponentInChildren<Castle>() != null)
                {
                    var newPlane = GetGamePlanePrefab();
                    aRPlaneManager.planePrefab = newPlane;
                    if (newPlane != null)
                    {
                        var material = newPlane.GetComponent<MeshRenderer>().material;
                        foreach (MeshRenderer mes in transform.GetComponentsInChildren<MeshRenderer>())
                        {
                            mes.material = material;
                        }
                    }
                    else
                    {
                        foreach (MeshRenderer mes in transform.GetComponentsInChildren<MeshRenderer>())
                        {
                            mes.material = null;
                        }
                    }
                }

                OnPlace?.Invoke(this, System.EventArgs.Empty);
                enabled = false;
            }
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private GameObject GetGamePlanePrefab()
    {
        if (!PlayerPrefs.HasKey("shadows"))
        {
            return planeWithShadows;
        }
        if (PlayerPrefs.GetInt("shadows") == 1)
        {
            return planeWithShadows;
        }
        return null;
    }
}
