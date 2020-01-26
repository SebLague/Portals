using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {

    public Portal linkedPortal;
    public MeshRenderer screen;
    Camera playerCam;
    Camera portalCam;
    RenderTexture viewTexture;
    List<PortalTraveller> trackedTravellers;
    public float testD;

    void Awake () {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera> ();
        portalCam.enabled = false;
        trackedTravellers = new List<PortalTraveller> ();
    }

    void LateUpdate () {
        for (int i = 0; i < trackedTravellers.Count; i++) {
            PortalTraveller traveller = trackedTravellers[i];
            Transform travellerT = traveller.transform;
            var m = linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * travellerT.localToWorldMatrix;

            Vector3 offsetFromPortal = travellerT.position - transform.position;
            int portalSide = System.Math.Sign (Vector3.Dot (offsetFromPortal, transform.forward));
            int portalSideOld = System.Math.Sign (Vector3.Dot (traveller.previousOffsetFromPortal, transform.forward));
            // Teleport the traveller if it has crossed from one side of the portal to the other
            if (portalSide != portalSideOld) {
                var positionOld = travellerT.position;
                var rotOld = travellerT.rotation;
                traveller.Teleport (transform, linkedPortal.transform, m.GetColumn (3), m.rotation);
                traveller.SetClonePositionAndRotation (positionOld, rotOld);
                // Can't rely on OnTriggerEnter/Exit to be called next frame since it depends on when FixedUpdate runs
                linkedPortal.OnTravellerEnterPortal (traveller);
                trackedTravellers.RemoveAt (i);
                i--;
            } else {
                traveller.SetClonePositionAndRotation (m.GetColumn (3), m.rotation);
                traveller.UpdateSlice (transform, linkedPortal.transform);
                traveller.previousOffsetFromPortal = offsetFromPortal;
            }
        }

        ProtectScreenFromClipping ();
    }

    void ProtectScreenFromClipping () {
        // Set the thickness of the portal screen so as not to clip with camera near plane when player goes through
        float halfHeight = playerCam.nearClipPlane * Mathf.Tan (playerCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * playerCam.aspect;
        float dstToNearClipCorner = new Vector3 (halfWidth, halfHeight, playerCam.nearClipPlane).magnitude;

        Transform screenT = screen.transform;
        bool camFacingSameDirAsPortal = Vector3.Dot (transform.forward, transform.position - playerCam.transform.position) > 0;
        screenT.localScale = new Vector3 (screenT.localScale.x, screenT.localScale.y, dstToNearClipCorner);
        screenT.localPosition = Vector3.forward * dstToNearClipCorner * ((camFacingSameDirAsPortal) ? 0.5f : -0.5f);
    }

    void CreateViewTexture () {
        if (viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height) {
            if (viewTexture != null) {
                viewTexture.Release ();
            }
            viewTexture = new RenderTexture (Screen.width, Screen.height, 0);
            // Render the view from the portal camera to the view texture
            portalCam.targetTexture = viewTexture;
            // Display the view texture on the screen of the linked portal
            linkedPortal.screen.material.SetTexture ("_MainTex", viewTexture);
        }
    }

    static bool VisibleFromCamera (Renderer renderer, Camera camera) {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes (camera);
        return GeometryUtility.TestPlanesAABB (frustumPlanes, renderer.bounds);
    }

    public void Render () {
        if (!VisibleFromCamera (linkedPortal.screen, playerCam)) {
            return;
        }
        screen.enabled = false;
        CreateViewTexture ();
        SetNearClipPlane ();

        // Make portal cam position and rotation the same relative to this portal as player cam relative to linked portal
        var m = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * playerCam.transform.localToWorldMatrix;
        portalCam.transform.SetPositionAndRotation (m.GetColumn (3), m.rotation);

        // Render the camera
        portalCam.Render ();
        screen.enabled = true;
    }

    void SetNearClipPlane () {

        // Resources: http://tomhulton.blogspot.com/2015/08/portal-rendering-with-offscreen-render.html
        // https://www.csharpcodi.com/vs2/805/Unity-AudioVisualization-/Assets/SampleAssets/Environment/Water/Water/Scripts/PlanarReflection.cs/
        // http://aras-p.info/texts/obliqueortho.html 
        Transform plane = transform;
        int dot = (Vector3.Dot (transform.position - portalCam.transform.position, plane.forward) < 0) ? -1 : 1;

        Vector3 camSpacePos = portalCam.worldToCameraMatrix.MultiplyPoint (plane.position);
        Vector3 camSpaceNormal = portalCam.worldToCameraMatrix.MultiplyVector (plane.forward).normalized * dot;
        float camSpaceDst = -Vector3.Dot (camSpacePos, camSpaceNormal);
        // Don't use oblique clip plane if very close to portal as this can cause some visual artifacts
        if (Mathf.Abs (camSpaceDst) > 0.02f) {
            Vector4 clipPlaneCameraSpace = new Vector4 (camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

            // Update projection based on new clip plane
            // Calculate matrix with player cam so that player camera settings (fov etc) are used
            portalCam.projectionMatrix = playerCam.CalculateObliqueMatrix (clipPlaneCameraSpace);
        } else {
            //Debug.Log (camSpaceDst);
            portalCam.projectionMatrix = playerCam.projectionMatrix;
        }
    }

    void OnTravellerEnterPortal (PortalTraveller traveller) {
        if (!trackedTravellers.Contains (traveller)) {
            traveller.EnterPortalThreshold ();
            traveller.UpdateSlice (transform, linkedPortal.transform);
            traveller.previousOffsetFromPortal = traveller.transform.position - transform.position;
            trackedTravellers.Add (traveller);
            ProtectScreenFromClipping ();
        }
    }

    void OnTriggerEnter (Collider other) {
        var traveller = other.GetComponent<PortalTraveller> ();
        if (traveller) {
            OnTravellerEnterPortal (traveller);
        }
    }

    void OnTriggerExit (Collider other) {
        var traveller = other.GetComponent<PortalTraveller> ();
        if (traveller && trackedTravellers.Contains (traveller)) {
            traveller.ExitPortalThreshold ();
            trackedTravellers.Remove (traveller);
        }
    }
}