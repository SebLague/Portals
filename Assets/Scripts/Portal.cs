using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {

    RenderTexture displayTexture;
    public Portal linkedPortal;
    public MeshRenderer portalMesh;
    public BoxCollider portalCollider;
    Camera playerCam;
    Camera portalCam;
    protected Plane collisionPlane;
    Vector3 posOld;
    FPSController player;

    void Awake () {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera> ();
    }

    protected virtual void Start () {
        collisionPlane = new Plane (transform.forward, transform.position);
        player = FindObjectOfType<FPSController> ();
        posOld = player.transform.position;
    }

    void TrackPlayer () {
        Vector3 posNew = player.transform.position;
        Plane plane = new Plane (transform.forward, transform.position);

        if (!plane.SameSide (posOld, posNew)) {
            float dstTravelled = (posNew - posOld).magnitude;
            Vector3 dir = (posNew - posOld) / dstTravelled;

            if (portalCollider.Raycast (new Ray (posOld - dir * 10, dir), out _, dstTravelled + 10)) {
                Vector3 portalOffset = player.transform.position - portalCollider.transform.position;
                Debug.Log ("Went through portal: " + linkedPortal.transform.position + portalOffset);

                TeleportPlayer (linkedPortal.transform.position + portalOffset);

            } else {
                Debug.Log ("Went passed portal");
            }
        }
        posOld = player.transform.position;
    }

    void TeleportPlayer (Vector3 position) {
        player.Teleport (position);

        // Immediately update cam pos + the depth of linked portal graphic in case player is entering the portal backwards
        UpdateCameraPosition ();
        linkedPortal.UpdateGraphicDepth ();
    }

    void SetNearClipPlane () {
        //return;
        // Resources: http://tomhulton.blogspot.com/2015/08/portal-rendering-with-offscreen-render.html
        // https://www.csharpcodi.com/vs2/805/Unity-AudioVisualization-/Assets/SampleAssets/Environment/Water/Water/Scripts/PlanarReflection.cs/
        // http://aras-p.info/texts/obliqueortho.html 
        Transform plane = transform;

        int dot = (Vector3.Dot (transform.position - portalCam.transform.position, plane.forward) < 0) ? -1 : 1;

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
        UpdateCameraPosition ();
        SetNearClipPlane ();
        TrackPlayer ();
        UpdateGraphicDepth ();
    }

    void UpdateCameraPosition () {
        Vector3 playerOffsetToLinkedPortal = playerCam.transform.position - linkedPortal.transform.position;
        Vector3 localOffset = linkedPortal.transform.InverseTransformVector (playerOffsetToLinkedPortal);

        portalCam.transform.position = transform.position + transform.TransformVector (playerOffsetToLinkedPortal);
        portalCam.transform.rotation = playerCam.transform.rotation;
    }

    void UpdateGraphicDepth () {
        float portalMeshDepth = 0.001f;
        float portalExtendDepth = playerCam.nearClipPlane + 0.1f;

        float playerSqrDstToPortal = (portalCollider.ClosestPoint (player.transform.position) - player.transform.position).sqrMagnitude;
        if (playerSqrDstToPortal <= playerCam.nearClipPlane) {
            int dir = (Vector3.Dot (transform.forward, transform.position - player.transform.position) < 0) ? -1 : 1;
            portalMesh.transform.localScale = new Vector3 (portalMesh.transform.localScale.x, portalMesh.transform.localScale.y, portalExtendDepth + portalMeshDepth);
            portalMesh.transform.localPosition = Vector3.forward * portalExtendDepth / 2 * dir;
        } else {
            portalMesh.transform.localScale = new Vector3 (portalMesh.transform.localScale.x, portalMesh.transform.localScale.y, portalMeshDepth);
            portalMesh.transform.localPosition = Vector3.zero;
        }
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