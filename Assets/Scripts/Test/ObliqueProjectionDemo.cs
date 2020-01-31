using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObliqueProjectionDemo : MonoBehaviour {

    public MeshFilter meshFilterA;
    public MeshFilter meshFilterB;
    public bool divideByW;
    public bool dropZ;
    [Range (0, 1)]
    public float projectPercent = 1;
    Vector3[] vertsA;

    public Color nearPlaneCol;
    public Color projLinesCol;
    public Color projLinesCol2;

    public bool useOblique;
    public Transform clipPlane;

    void Awake () {
        vertsA = meshFilterA.mesh.vertices;
    }

    void Update () {
        if (useOblique) {
            SetNearClipPlane ();
        }
        Run ();

    }

    void SetNearClipPlane () {
        Camera cam = Camera.main;
        // Resources:
        // http://tomhulton.blogspot.com/2015/08/portal-rendering-with-offscreen-render.html
        // http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
        Transform plane = clipPlane;
        int dot = (Vector3.Dot (transform.position - cam.transform.position, plane.forward) < 0) ? -1 : 1;

        Vector3 camSpacePos = cam.worldToCameraMatrix.MultiplyPoint (plane.position);
        Vector3 camSpaceNormal = cam.worldToCameraMatrix.MultiplyVector (plane.forward).normalized * dot;
        float camSpaceDstToPlane = -Vector3.Dot (camSpacePos, camSpaceNormal) + 0;

        Vector4 clipPlaneCameraSpace = new Vector4 (camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDstToPlane);

        // Update projection based on new clip plane
        // Calculate matrix with player cam so that player camera settings (fov, etc) are used
        cam.projectionMatrix = cam.CalculateObliqueMatrix (clipPlaneCameraSpace);
    }

    void Run () {
        Camera cam = Camera.main;

        Vector3[] newVerts = new Vector3[vertsA.Length];
        var worldToCam = cam.worldToCameraMatrix;
        var projectionMatrix = cam.projectionMatrix;
        var mvp = cam.projectionMatrix * worldToCam * meshFilterA.transform.localToWorldMatrix;

        // to projection space
        for (int i = 0; i < vertsA.Length; i++) {
            //var worldVert = meshFilterA.transform.TransformPoint (vertsA[i]);
            Vector4 worldVert = meshFilterA.transform.localToWorldMatrix * new Vector4 (vertsA[i].x, vertsA[i].y, vertsA[i].z, 1);

            Vector4 projectionSpaceVert = mvp * new Vector4 (vertsA[i].x, vertsA[i].y, vertsA[i].z, 1);
            if (divideByW) {
                newVerts[i] = projectionSpaceVert / projectionSpaceVert.w;
            }
            if (dropZ) {
                newVerts[i] = new Vector3 (newVerts[i].x, newVerts[i].y, 1);
            }
            newVerts[i] = Vector3.Lerp (worldVert, newVerts[i], Mathf.Clamp (projectPercent, 0, 0.999f));

        }
        meshFilterB.mesh.vertices = newVerts;
        meshFilterB.mesh.RecalculateBounds ();
    }

    void OnDrawGizmos () {
        var cam = Camera.main;
        float halfHeight = cam.nearClipPlane * Mathf.Tan (cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * cam.aspect;
        float dstToNearClipPlaneCorner = new Vector3 (halfWidth, halfHeight, cam.nearClipPlane).magnitude;
        Vector3 topLeft = new Vector3 (-halfWidth, halfHeight, cam.nearClipPlane);
        Vector3 topRight = new Vector3 (halfWidth, halfHeight, cam.nearClipPlane);
        Vector3 bottomLeft = new Vector3 (-halfWidth, -halfHeight, cam.nearClipPlane);
        Vector3 bottomRight = new Vector3 (halfWidth, -halfHeight, cam.nearClipPlane);

        Vector3 topLeftN = Vector3.zero;
        Vector3 topRightN = Vector3.zero;
        Vector3 bottomLeftN = Vector3.zero;
        Vector3 bottomRightN = Vector3.zero;

        Plane p = new Plane (clipPlane.forward, clipPlane.position);
        float dst;
        if (p.Raycast (new Ray (topLeft, topLeft.normalized), out dst)) {
            topLeftN = topLeft + topLeft.normalized * dst;
        }
        if (p.Raycast (new Ray (topRight, topRight.normalized), out dst)) {
            topRightN = topRight + topRight.normalized * dst;
        }
        if (p.Raycast (new Ray (bottomLeft, bottomLeft.normalized), out dst)) {
            bottomLeftN = bottomLeft + bottomLeft.normalized * dst;
        }
        if (p.Raycast (new Ray (bottomRight, bottomRight.normalized), out dst)) {
            bottomRightN = bottomRight + bottomRight.normalized * dst;
        }

        Gizmos.color = nearPlaneCol;
        Gizmos.DrawLine (topLeftN, topRightN);
        Gizmos.DrawLine (topRightN, bottomRightN);
        Gizmos.DrawLine (bottomRightN, bottomLeftN);
        Gizmos.DrawLine (bottomLeftN, topLeftN);

        Gizmos.color = projLinesCol;
        Gizmos.DrawLine (topLeftN, cam.transform.position);
        Gizmos.DrawLine (topRightN, cam.transform.position);
        Gizmos.DrawLine (bottomRightN, cam.transform.position);
        Gizmos.DrawLine (bottomLeftN, cam.transform.position);

        Gizmos.color = projLinesCol2;
        const float d = 1000;
        Gizmos.DrawRay (topLeftN, topLeftN.normalized * d);
        Gizmos.DrawRay (topRightN, topRightN.normalized * d);
        Gizmos.DrawRay (bottomRightN, bottomRightN.normalized * d);
        Gizmos.DrawRay (bottomLeftN, bottomLeftN.normalized * d);

        Gizmos.color = new Color (1, 0, 0, 0.5f);
        //Gizmos.DrawWireCube (Vector3.forward, new Vector3 (2, 2, 0));
        Gizmos.color = new Color (1, 1, 0, 0.5f);
        Gizmos.DrawWireCube (Vector3.zero, Vector3.one * 2);
    }
}