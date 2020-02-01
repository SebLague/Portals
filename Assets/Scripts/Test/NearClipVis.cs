using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearClipVis : MonoBehaviour {
    public Transform clipPlane;
    public bool useClipPlane;
    public bool vis;
    public float offset;

    public Color nearPlaneCol;
    public Color projLinesCol;
    public Color projLinesCol2;
    public MeshFilter sliceMesh;

    void Update () {
        if (useClipPlane) {
            SetNearClipPlane ();
        }

        SetMesh ();
    }

    void SetNearClipPlane () {
        Camera cam = GetComponent<Camera> ();
        // Learning resource:
        // http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
        Transform plane = clipPlane;
        int dot = (Vector3.Dot (transform.position - cam.transform.position, plane.forward) < 0) ? -1 : 1;

        Vector3 camSpacePos = cam.worldToCameraMatrix.MultiplyPoint (plane.position);
        Vector3 camSpaceNormal = cam.worldToCameraMatrix.MultiplyVector (plane.forward) * dot;
        float camSpaceDstToPlane = -Vector3.Dot (camSpacePos, camSpaceNormal) + offset;

        Vector4 clipPlaneCameraSpace = new Vector4 (camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDstToPlane);

        // Update projection based on new clip plane
        // Calculate matrix with player cam so that player camera settings (fov, etc) are used
        cam.projectionMatrix = cam.CalculateObliqueMatrix (clipPlaneCameraSpace);
    }

    void SetMesh () {
        var cam = GetComponent<Camera> ();
        float halfHeight = cam.nearClipPlane * Mathf.Tan (cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * cam.aspect;
        float dstToNearClipPlaneCorner = new Vector3 (halfWidth, halfHeight, cam.nearClipPlane).magnitude;
        Vector3 topLeft = cam.transform.TransformPoint (new Vector3 (-halfWidth, halfHeight, cam.nearClipPlane));
        Vector3 topRight = cam.transform.TransformPoint (new Vector3 (halfWidth, halfHeight, cam.nearClipPlane));
        Vector3 bottomLeft = cam.transform.TransformPoint (new Vector3 (-halfWidth, -halfHeight, cam.nearClipPlane));
        Vector3 bottomRight = cam.transform.TransformPoint (new Vector3 (halfWidth, -halfHeight, cam.nearClipPlane));

        Vector3 topLeftN = Vector3.zero;
        Vector3 topRightN = Vector3.zero;
        Vector3 bottomLeftN = Vector3.zero;
        Vector3 bottomRightN = Vector3.zero;

        Plane p = new Plane (clipPlane.forward, clipPlane.position);
        float dst;
        if (p.Raycast (new Ray (topLeft, (topLeft - cam.transform.position).normalized), out dst)) {
            topLeftN = topLeft + (topLeft - cam.transform.position).normalized * dst;
        }
        if (p.Raycast (new Ray (topRight, (topRight - cam.transform.position).normalized), out dst)) {
            topRightN = topRight + (topRight - cam.transform.position).normalized * dst;
        }
        if (p.Raycast (new Ray (bottomLeft, (bottomLeft - cam.transform.position).normalized), out dst)) {
            bottomLeftN = bottomLeft + (bottomLeft - cam.transform.position).normalized * dst;
        }
        if (p.Raycast (new Ray (bottomRight, (bottomRight - cam.transform.position).normalized), out dst)) {
            bottomRightN = bottomRight + (bottomRight - cam.transform.position).normalized * dst;
        }

        Vector3[] verts = { topLeftN, bottomLeftN, topRightN, bottomRightN };
        sliceMesh.sharedMesh.vertices = verts;
        sliceMesh.sharedMesh.RecalculateBounds ();
    }

    void OnDrawGizmos () {
        if (!vis) {
            return;
        }
        var cam = GetComponent<Camera> ();
        float halfHeight = cam.nearClipPlane * Mathf.Tan (cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * cam.aspect;
        float dstToNearClipPlaneCorner = new Vector3 (halfWidth, halfHeight, cam.nearClipPlane).magnitude;
        Vector3 topLeft = cam.transform.TransformPoint (new Vector3 (-halfWidth, halfHeight, cam.nearClipPlane));
        Vector3 topRight = cam.transform.TransformPoint (new Vector3 (halfWidth, halfHeight, cam.nearClipPlane));
        Vector3 bottomLeft = cam.transform.TransformPoint (new Vector3 (-halfWidth, -halfHeight, cam.nearClipPlane));
        Vector3 bottomRight = cam.transform.TransformPoint (new Vector3 (halfWidth, -halfHeight, cam.nearClipPlane));

        Vector3 topLeftN = Vector3.zero;
        Vector3 topRightN = Vector3.zero;
        Vector3 bottomLeftN = Vector3.zero;
        Vector3 bottomRightN = Vector3.zero;

        Plane p = new Plane (clipPlane.forward, clipPlane.position);
        float dst;
        if (p.Raycast (new Ray (topLeft, (topLeft - cam.transform.position).normalized), out dst)) {
            topLeftN = topLeft + (topLeft - cam.transform.position).normalized * dst;
        }
        if (p.Raycast (new Ray (topRight, (topRight - cam.transform.position).normalized), out dst)) {
            topRightN = topRight + (topRight - cam.transform.position).normalized * dst;
        }
        if (p.Raycast (new Ray (bottomLeft, (bottomLeft - cam.transform.position).normalized), out dst)) {
            bottomLeftN = bottomLeft + (bottomLeft - cam.transform.position).normalized * dst;
        }
        if (p.Raycast (new Ray (bottomRight, (bottomRight - cam.transform.position).normalized), out dst)) {
            bottomRightN = bottomRight + (bottomRight - cam.transform.position).normalized * dst;
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
        Gizmos.DrawRay (topLeftN, (topLeftN - cam.transform.position).normalized * d);
        Gizmos.DrawRay (topRightN, (topRightN - cam.transform.position).normalized * d);
        Gizmos.DrawRay (bottomRightN, (bottomRightN - cam.transform.position).normalized * d);
        Gizmos.DrawRay (bottomLeftN, (bottomLeftN - cam.transform.position).normalized * d);

    }

}