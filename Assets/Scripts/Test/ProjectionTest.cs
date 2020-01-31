using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionTest : MonoBehaviour {

    public MeshFilter meshFilterA;
    public MeshFilter meshFilterB;
    public bool divideByW;
    public bool dropZ;
    [Range (0, 1)]
    public float projectPercent = 1;
    Vector3[] vertsA;

    void Awake () {
        vertsA = meshFilterA.mesh.vertices;
    }

    void Update () {
        Run ();
    }

    void Run () {
        Camera cam = Camera.main;

        Vector3[] newVerts = new Vector3[vertsA.Length];
        var worldToCam = cam.worldToCameraMatrix;
        var projectionMatrix = cam.projectionMatrix;
        var mvp = cam.projectionMatrix * worldToCam * meshFilterA.transform.localToWorldMatrix;

        // to projection space
        for (int i = 0; i < vertsA.Length; i++) {
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
        var m = Gizmos.matrix;
        var cam = Camera.main;
        Gizmos.matrix = cam.transform.localToWorldMatrix;
        Gizmos.DrawFrustum (cam.transform.position, cam.fieldOfView, cam.farClipPlane, cam.nearClipPlane, cam.aspect);
        Gizmos.matrix = m;

        Gizmos.color = new Color (1, 0, 0, 0.5f);
        Gizmos.DrawWireCube (Vector3.forward, new Vector3 (2, 2, 0));
        Gizmos.color = new Color (1, 1, 0, 0.5f);
        Gizmos.DrawWireCube (Vector3.zero, Vector3.one * 2);
    }
}