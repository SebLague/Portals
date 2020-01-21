using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {

    RenderTexture displayTexture;
    public Portal linkedPortal;
    public MeshRenderer portalMesh;
    public BoxCollider portalCollider;
    public bool useHDR;
    Camera playerCam;
    Camera portalCam;
    protected Plane collisionPlane;
    Vector3 posOld;
    FPSController player;
    public int recCount = 1;

    void Awake () {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera> ();
        portalCam.enabled = false;
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

            if (portalCollider.Raycast (new Ray (posOld - dir * 2, dir), out _, dstTravelled + 2)) {
                Vector3 portalOffset = player.transform.position - portalCollider.transform.position;
                Debug.Log ("Went through portal: " + linkedPortal.transform.position + portalOffset);

                TeleportPlayer (linkedPortal.transform.position + portalOffset);

            } else {
                //Debug.Log ("Went passed portal");
            }
        }
        posOld = player.transform.position;
    }

    void TeleportPlayer (Vector3 position) {
        var mirrorMatrix = linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * player.transform.localToWorldMatrix;
        Vector3 teleportPos = mirrorMatrix.GetColumn (3);
        Quaternion teleportRot = mirrorMatrix.rotation;
        //portalCam.transform.SetPositionAndRotation (mirrorMatrix.GetColumn (3), mirrorMatrix.rotation);
        //player.Teleport (teleportPos, teleportRot);
        player.Teleport (transform, linkedPortal.transform);
        // Immediately update cam pos + the depth of linked portal graphic in case player is entering the portal backwards
        //UpdateCameraPosition ();

        linkedPortal.UpdateGraphicDepth ();
        linkedPortal.posOld = teleportPos;
    }

    void SetNearClipPlane () {
        return;
        // Resources: http://tomhulton.blogspot.com/2015/08/portal-rendering-with-offscreen-render.html
        // https://www.csharpcodi.com/vs2/805/Unity-AudioVisualization-/Assets/SampleAssets/Environment/Water/Water/Scripts/PlanarReflection.cs/
        // http://aras-p.info/texts/obliqueortho.html 
        Transform plane = transform;
        int dot = (Vector3.Dot (transform.position - portalCam.transform.position, plane.forward) < 0) ? -1 : 1;

        Vector3 camSpacePos = portalCam.worldToCameraMatrix.MultiplyPoint (plane.position);
        Vector3 camSpaceNormal = portalCam.worldToCameraMatrix.MultiplyVector (plane.forward).normalized * dot;
        float camSpaceDst = -Vector3.Dot (camSpacePos, camSpaceNormal);
        if (Mathf.Abs (camSpaceDst) > 0.01f) {
            Vector4 clipPlaneCameraSpace = new Vector4 (camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

            // Update projection based on new clip plane
            // Calculate matrix with player cam so that player camera settings (fov etc) are used
            portalCam.projectionMatrix = playerCam.CalculateObliqueMatrix (clipPlaneCameraSpace);
        } else {
            //Debug.Log (camSpaceDst);
            portalCam.projectionMatrix = playerCam.projectionMatrix;
        }
    }

    public void SetRenderTarget (RenderTexture targetTexture) {
        portalCam.targetTexture = targetTexture;
    }

    protected virtual void LateUpdate () {
        TrackPlayer ();

        UpdateRenderTexture ();
        UpdateCameraPosition ();
        SetNearClipPlane ();
        UpdateGraphicDepth ();
    }

    void UpdateCameraPosition () {
        var mirrorMatrix = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * playerCam.transform.localToWorldMatrix;
        portalCam.transform.SetPositionAndRotation (mirrorMatrix.GetColumn (3), mirrorMatrix.rotation);
        return;
        Vector3 playerOffsetToLinkedPortal = playerCam.transform.position - linkedPortal.transform.position;
        Vector3 localOffset = linkedPortal.transform.InverseTransformVector (playerOffsetToLinkedPortal);

        portalCam.transform.position = transform.position + transform.TransformVector (playerOffsetToLinkedPortal);
        portalCam.transform.rotation = playerCam.transform.rotation;
    }

    void UpdateGraphicDepth () {

        float halfHeight = playerCam.nearClipPlane * Mathf.Tan (playerCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * playerCam.aspect;
        float dst = Mathf.Sqrt (halfHeight * halfHeight + halfWidth * halfWidth + playerCam.nearClipPlane * playerCam.nearClipPlane);

        float portalMeshDepth = 0.001f;
        float portalExtendDepth = dst;

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
            displayTexture = new RenderTexture (Screen.width, Screen.height, 0, (useHDR) ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default);
            portalMesh.material.SetTexture ("_MainTex", displayTexture);
            linkedPortal.SetRenderTarget (displayTexture);
        }

    }

    public void Render () {

        portalMesh.enabled = false;
        var m = linkedPortal.portalMesh.material;
        //linkedPortal.portalMesh.material = new Material (Shader.Find ("Unlit/RecTest"));
        linkedPortal.portalMesh.material.mainTexture = portalCam.targetTexture;
        //linkedPortal.portalMesh.material = m;

        //linkedPortal.portalMesh.material.mainTexture = linkedPortal.displayTexture;
        for (int i = 0; i < recCount; i++) {
            linkedPortal.portalMesh.material.SetInt ("mode", (i == 0) ? 1 : 0);

            //var camTex = new RenderTexture (Screen.width, Screen.height, 0, RenderTextureFormat.Default);
            //portalCam.targetTexture = camTex;
            portalCam.Render ();

        }
        linkedPortal.portalMesh.material.SetInt ("mode", 0);
        //linkedPortal.portalMesh.material.mainTexture = linkedPortal.displayTexture;
        //linkedPortal.portalMesh.material = m;
        portalMesh.enabled = true;
    }

    void OnValidate () {
        if (linkedPortal != null && linkedPortal.linkedPortal != this) {
            linkedPortal.linkedPortal = this;
        }
    }
}