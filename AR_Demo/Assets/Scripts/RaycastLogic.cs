using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARPlacementController : MonoBehaviour
{
    [SerializeField] private GameObject objectToPlace;
    private ARRaycastManager _raycastManager;
    private GameObject _spawnedObject;
    private static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    void Awake() => _raycastManager = GetComponent<ARRaycastManager>();

    void Update()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;

        // PRIORITY 1: Precise Plane Detection (Polygons)
        // Best for non-ToF devices to ensure object 'sticks' to the floor.
        if (_raycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            PlaceOrMoveObject(s_Hits[0].pose);
        }
        // PRIORITY 2: Feature Point Fallback
        // Allows placement BEFORE planes are fully formed by using the raw point cloud.
        else if (_raycastManager.Raycast(touch.position, s_Hits, TrackableType.FeaturePoint))
        {
            PlaceOrMoveObject(s_Hits[0].pose);
        }
    }

    private void PlaceOrMoveObject(Pose pose)
    {
        if (_spawnedObject == null)
            _spawnedObject = Instantiate(objectToPlace, pose.position, pose.rotation);
        else
            _spawnedObject.transform.SetPositionAndRotation(pose.position, pose.rotation);
    }
}