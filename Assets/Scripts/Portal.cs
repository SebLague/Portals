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
        //Plane separationPlane = new Plane (transform.forward, transform.position);

        for (int i = 0; i < trackedTravellers.Count; i++) {
            PortalTraveller traveller = trackedTravellers[i];
            // Check if entity has moved from one side of the portal to the other in the last frame
            Vector3 portalOffset = transform.position - traveller.transform.position;

            int isFacingPortal = System.Math.Sign (Vector3.Dot (portalOffset, transform.forward));
            int wasFacingPortal = System.Math.Sign (Vector3.Dot (traveller.previousPortalOffset, transform.forward));
            if (isFacingPortal != wasFacingPortal) {
                Debug.Log ("teleport: " + gameObject.name);
                // Teleport
                var m = linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * traveller.transform.localToWorldMatrix;
                traveller.Teleport (transform, linkedPortal.transform, m.GetColumn (3), m.rotation);

                // Can't rely on OnTriggerExit to remove tracked traveller next frame since it's
                // dependent on when fixed update is next called
                trackedTravellers.RemoveAt (i);
                i--;
                //linkedPortal.UpdateScreenDepth ();

            } else {
                traveller.previousPortalOffset = transform.position - traveller.transform.position;
            }

        }

        //UpdateScreenDepth ();
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

    void OnTriggerEnter (Collider other) {
        var traveller = other.GetComponent<PortalTraveller> ();
        if (traveller) {
            Debug.Log ("enter: " + gameObject.name);
            traveller.previousPortalOffset = transform.position - traveller.transform.position;
            trackedTravellers.Add (traveller);
        }
    }

    void OnTriggerExit (Collider other) {
        var traveller = other.GetComponent<PortalTraveller> ();
        if (trackedTravellers.Contains (traveller)) {
            Debug.Log ("exit: " + gameObject.name);
            trackedTravellers.Remove (traveller);
        }
    }

}