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
    Material firstRecursionMat;
    public bool log;

    void Awake () {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera> ();
        portalCam.enabled = false;
    }

    protected virtual void Start () {
        collisionPlane = new Plane (transform.forward, transform.position);
        player = FindObjectOfType<FPSController> ();
        posOld = player.transform.position;
        firstRecursionMat = new Material (Shader.Find ("Unlit/Color"));
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

    public void SetRenderTarget (RenderTexture targetTexture) {
        portalCam.targetTexture = targetTexture;
    }

    protected virtual void LateUpdate () {
        TrackPlayer ();

        UpdateRenderTexture ();

        UpdateGraphicDepth ();
    }

    void UpdateGraphicDepth () {
        // Extend the depth of the portal display when player is going through it so as not to clip with camera near plane
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
            //displayTexture = new RenderTexture (Screen.width, Screen.height, 0, (useHDR) ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default);
            displayTexture = new RenderTexture (Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
            portalMesh.material.SetTexture ("_MainTex", displayTexture);
            linkedPortal.SetRenderTarget (displayTexture);
        }

    }

    // http://wiki.unity3d.com/index.php/IsVisibleFrom
    static bool CheckVisible (MeshRenderer renderer, Camera camera) {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes (camera);
        return GeometryUtility.TestPlanesAABB (planes, renderer.bounds);
    }

    public void Render () {
        // Skip rendering the view from this portal if player is not looking at the linked portal
        if (!CheckVisible (linkedPortal.portalMesh, playerCam)) {
            if (log) {

                Debug.Log ("Skip");
            }
            return;
        }

        portalMesh.enabled = false;
        bool useRecursion = true;

        var localToWorldMatrix = playerCam.transform.localToWorldMatrix;
        Matrix4x4[] matrices = new Matrix4x4[recCount];
        for (int i = 0; i < recCount; i++) {
            localToWorldMatrix = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * localToWorldMatrix;
            matrices[recCount - i - 1] = localToWorldMatrix;

            if (i == 0) {
                portalCam.transform.SetPositionAndRotation (localToWorldMatrix.GetColumn (3), localToWorldMatrix.rotation);
                portalCam.projectionMatrix = playerCam.projectionMatrix;
                useRecursion = CheckVisible (linkedPortal.portalMesh, portalCam);
            }
        }

        var originalMat = linkedPortal.portalMesh.material;
        //linkedPortal.portalMesh.material = firstRecursionMat;
        int startIndex = (useRecursion) ? 0 : recCount - 1;
        for (int i = startIndex; i < recCount; i++) {
            portalCam.transform.SetPositionAndRotation (matrices[i].GetColumn (3), matrices[i].rotation);
            SetNearClipPlane ();
            portalCam.Render ();
            linkedPortal.portalMesh.material = originalMat;
        }

        if (log) {
            if (!useRecursion) {
                Debug.Log ("Skip");
            }
        }

        portalMesh.enabled = true;
    }

}