using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {

    public Portal linkedPortal;
    public MeshRenderer screen;
    Camera playerCam;
    Camera portalCam;
    RenderTexture viewTexture;
    List<PortalTraveller> trackedTravellers;
    Collider portalCollider;

    void Awake () {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera> ();
        portalCam.enabled = false;
        trackedTravellers = new List<PortalTraveller> ();
        portalCollider = GetComponent<Collider> ();
    }

    void LateUpdate () {
        for (int i = 0; i < trackedTravellers.Count; i++) {
            PortalTraveller traveller = trackedTravellers[i];
            Transform travellerT = traveller.transform;
            var m = linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * travellerT.localToWorldMatrix;

            Vector3 portalOffset = transform.position - travellerT.position;
            int isFacingPortal = System.Math.Sign (Vector3.Dot (portalOffset, transform.forward));
            int wasFacingPortal = System.Math.Sign (Vector3.Dot (traveller.previousPortalOffset, transform.forward));
            // Check if entity has moved from one side of the portal to the other in the last frame
            if (isFacingPortal != wasFacingPortal) {
                // Teleport
                var positionOld = travellerT.position;
                var rotOld = travellerT.rotation;
                traveller.Teleport (transform, linkedPortal.transform, m.GetColumn (3), m.rotation);
                traveller.SetClonePositionAndRotation (positionOld, rotOld);

                // Can't rely on OnTriggerEnter/Exit to be called on the next frame since it depends on when fixedupdate runs
                linkedPortal.OnEnterPortal (traveller);
                trackedTravellers.RemoveAt (i);
                i--;

            } else {
                traveller.SetClonePositionAndRotation (m.GetColumn (3), m.rotation);
                traveller.UpdateSlice (transform, linkedPortal.transform);
                traveller.previousPortalOffset = transform.position - travellerT.position;
            }

        }

        UpdateScreenDepth ();
    }

    void UpdateScreenDepth () {
        // Extend the depth of the portal display when player is going through it so as not to clip with camera near plane
        float halfHeight = playerCam.nearClipPlane * Mathf.Tan (playerCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * playerCam.aspect;
        float dstToNearClipCorner = new Vector3 (halfWidth, halfHeight, playerCam.nearClipPlane).magnitude;

        bool playerFacingSameDirAsPortal = Vector3.Dot (transform.forward, transform.position - playerCam.transform.position) > 0;
        screen.transform.localScale.Set (screen.transform.localScale.x, playerCam.transform.localScale.y, dstToNearClipCorner);
        screen.transform.localPosition = Vector3.forward * dstToNearClipCorner * ((playerFacingSameDirAsPortal) ? 0.5f : -0.5f);
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
        screen.enabled = false;
        CreateViewTexture ();

        // Make portal cam position and rotation the same relative to this portal as player cam relative to linked portal
        var m = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * playerCam.transform.localToWorldMatrix;
        portalCam.transform.SetPositionAndRotation (m.GetColumn (3), m.rotation);

        // Render the camera
        portalCam.Render ();

        screen.enabled = true;
    }

    void OnEnterPortal (PortalTraveller traveller) {
        if (!trackedTravellers.Contains (traveller)) {
            traveller.EnterPortalThreshold ();
            traveller.previousPortalOffset = transform.position - traveller.transform.position;
            trackedTravellers.Add (traveller);
            traveller.UpdateSlice (transform, linkedPortal.transform);
            UpdateScreenDepth ();
        }
    }

    void OnTriggerEnter (Collider other) {
        var traveller = other.GetComponent<PortalTraveller> ();
        if (traveller) {
            OnEnterPortal (traveller);
        }
    }

    void OnTriggerExit (Collider other) {
        var traveller = other.GetComponent<PortalTraveller> ();
        if (trackedTravellers.Contains (traveller)) {
            traveller.ExitPortalThreshold ();
            trackedTravellers.Remove (traveller);
        }
    }

}