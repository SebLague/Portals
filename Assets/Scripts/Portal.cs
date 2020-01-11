using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {

    public bool log;
    RenderTexture displayTexture;
    public Portal linkedPortal;
    public MeshRenderer portalMesh;
    Camera playerCam;
    Camera portalCam;
    protected Plane collisionPlane;

    void Awake () {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera> ();
    }

    protected virtual void Start () {
        collisionPlane = new Plane (transform.forward, transform.position);
    }

    void SetNearClipPlane () {
        // Resources: http://tomhulton.blogspot.com/2015/08/portal-rendering-with-offscreen-render.html
        // https://www.csharpcodi.com/vs2/805/Unity-AudioVisualization-/Assets/SampleAssets/Environment/Water/Water/Scripts/PlanarReflection.cs/
        // http://aras-p.info/texts/obliqueortho.html 
        Transform plane = transform;
        int dot = (Vector3.Dot (portalCam.transform.forward, plane.forward) < 0) ? -1 : 1;
        if (log) {
            Debug.Log(Vector3.Dot (portalCam.transform.forward, plane.forward));
        }

        Vector3 camSpacePos = portalCam.worldToCameraMatrix.MultiplyPoint (plane.position);
        Vector3 camSpaceNormal = portalCam.worldToCameraMatrix.MultiplyVector (plane.forward).normalized * dot;
        Vector4 clipPlaneCameraSpace = new Vector4 (camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, -Vector3.Dot (camSpacePos, camSpaceNormal));

        // Update projection based on new clip plane
        // Calculate matrix with player cam because seem to get weird results calculating it with the already-modifed cam
        portalCam.projectionMatrix = playerCam.CalculateObliqueMatrix (clipPlaneCameraSpace);
    }

    public void SetRenderTarget (RenderTexture targetTexture) {
        portalCam.targetTexture = targetTexture;
    }

    protected virtual void LateUpdate () {
        UpdateRenderTexture ();
        Vector3 playerOffsetToLinkedPortal = playerCam.transform.position - linkedPortal.transform.position;
        Vector3 localOffset = linkedPortal.transform.InverseTransformVector (playerOffsetToLinkedPortal);

        portalCam.transform.position = transform.position + transform.TransformVector (playerOffsetToLinkedPortal);
        portalCam.transform.rotation = playerCam.transform.rotation;
        SetNearClipPlane ();

    }

    void UpdateRenderTexture () {
        if (displayTexture == null || displayTexture.width != Screen.width || displayTexture.height != Screen.height) {
            if (displayTexture != null) {
                displayTexture.Release ();
            }
            displayTexture = new RenderTexture (Screen.width, Screen.height, 0);
            portalMesh.material.SetTexture ("_MainTex", displayTexture);
            linkedPortal.SetRenderTarget (displayTexture);
        }
    }

    void OnValidate () {
        if (linkedPortal != null && linkedPortal.linkedPortal != this) {
            linkedPortal.linkedPortal = this;
        }
    }
}