using Google.XR.ARCoreExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class VPSManager : MonoBehaviour
{
    [SerializeField]
    private AREarthManager earthManager;

    [Serializable]
    public struct GeospatialObject
    {
        public GameObject ObjectPrefab;
        public EarthPosition EarthPosition;
    }

    [SerializeField]
    private ARAnchorManager anchorManager;

    [SerializeField]
    private List<GeospatialObject> geospatialObjects = new List<GeospatialObject>();

    [Serializable]
    public struct EarthPosition
    {
        public double Latitude;
        public double Longitude;
        public double Altitude;
    }

    // Start is called before the first frame update
    void Start()
    {
        VerifyGeospatialSupport();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void VerifyGeospatialSupport()
    {
        FeatureSupported result = earthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);
        switch (result)
        {
            case FeatureSupported.Supported:
                PlaceObjects();
                break;
            case FeatureSupported.Unknown:
                Invoke("VerifyGeospatialSupport", 5.0f);
                break;
            case FeatureSupported.Unsupported:
                break;
        }
    }

    private void PlaceObjects()
    {
        if(earthManager.EarthTrackingState == TrackingState.Tracking)
        {
            GeospatialPose geospatialPose = earthManager.CameraGeospatialPose;
            foreach(GeospatialObject item in geospatialObjects)
            {
                EarthPosition earthPosition = item.EarthPosition;
                ARGeospatialAnchor objAnchor = ARAnchorManagerExtensions.AddAnchor(anchorManager, earthPosition.Latitude, earthPosition.Longitude, earthPosition.Altitude, Quaternion.identity);
                Instantiate(item.ObjectPrefab, objAnchor.transform);
            }
        } else if(earthManager.EarthTrackingState == TrackingState.None)
        {
            Invoke("PlaceObjects", 5.0f);
        }
    }
}
