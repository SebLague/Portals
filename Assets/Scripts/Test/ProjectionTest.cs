using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionTest : MonoBehaviour {

    public MeshFilter meshFilterA;
    public MeshFilter meshFilterB;
    [Range (0, 1)]
    public float wDivPercent = 1;
    public bool dropZ;
    [Range (0, 1)]
    public float projectPercent = 1;
    [Range (0, 1)]
    public float aspectCorrectPercent = 0;
    Vector3[] verts;
    Vector3[] normalsA;
    public bool drawClipBox;
    public bool drawFrustrum;

    void Awake () {
        verts = meshFilterA.mesh.vertices;
        normalsA = meshFilterA.mesh.normals;

        // normals
        Vector3[] newNormals = new Vector3[normalsA.Length];
        for (int i = 0; i < normalsA.Length; i++) {
            //var worldVert = meshFilterA.transform.TransformPoint (vertsA[i]);
            Vector4 worldNormal = meshFilterA.transform.localToWorldMatrix * meshFilterB.transform.worldToLocalMatrix * new Vector4 (normalsA[i].x, normalsA[i].y, normalsA[i].z, 0);
            newNormals[i] = worldNormal;
        }
        //meshFilterB.mesh.normals = newNormals;
        //meshFilterB.mesh.RecalculateNormals ();
    }

    void Update () {
        RunOptim ();
    }

    void RunOptim () {
        Camera cam = Camera.main;

        Vector3[] newVerts = new Vector3[verts.Length];
        Vector3[] newNormals = new Vector3[normalsA.Length];
        var worldToCameraMatrix = cam.worldToCameraMatrix;
        var projectionMatrix = cam.projectionMatrix;
        var b = cam.projectionMatrix * worldToCameraMatrix * meshFilterA.transform.localToWorldMatrix;
        var localToWorldMatrix = meshFilterA.transform.localToWorldMatrix;

        // to projection space
        for (int i = 0; i < verts.Length; i++) {
            var modelSpaceVertex = new Vector4 (verts[i].x, verts[i].y, verts[i].z, 1);
            //var worldVert = meshFilterA.transform.TransformPoint (vertsA[i]);
            Vector4 worldSpaceVert = localToWorldMatrix * modelSpaceVertex;
            Vector4 viewSpaceVert = worldToCameraMatrix * worldSpaceVert;

            Vector4 projectionSpaceVert = cam.projectionMatrix * viewSpaceVert;
            projectionSpaceVert = b * new Vector4 (verts[i].x, verts[i].y, verts[i].z, 1);
            newNormals[i] = b * new Vector4 (normalsA[i].x, normalsA[i].y, normalsA[i].z, 0);
            //var projectionSpaceVert = projectionMatrix * corrected;
            //newVerts[i] = projectionSpaceVert / projectionSpaceVert.w;

            newVerts[i] = projectionSpaceVert;

            float v = Mathf.Lerp (1, projectionSpaceVert.w, wDivPercent);
            newVerts[i] = projectionSpaceVert / v;

            if (dropZ) {
                newVerts[i] = new Vector3 (newVerts[i].x, newVerts[i].y, 1);
            }
            newVerts[i] = Vector3.Lerp (worldSpaceVert, newVerts[i], Mathf.Clamp (projectPercent, 0, 0.999f));

            //newVerts[i] = viewSpaceVert;

        }
        meshFilterB.mesh.vertices = newVerts;
        meshFilterB.mesh.RecalculateBounds ();
        //meshFilterB.mesh.normals = newNormals;
    }

    void OnDrawGizmos () {
        if (drawFrustrum) {
            var m = Gizmos.matrix;
            var cam = Camera.main;
            Gizmos.matrix = cam.transform.localToWorldMatrix;
            Gizmos.DrawFrustum (cam.transform.position, cam.fieldOfView, cam.farClipPlane, cam.nearClipPlane, cam.aspect);
            Gizmos.matrix = m;
        }
        if (drawClipBox) {

            float s = Mathf.Lerp (1, Camera.main.pixelWidth / (float) Camera.main.pixelHeight, aspectCorrectPercent);
            Gizmos.color = new Color (1, 0, 0, 0.5f);
            //Gizmos.DrawWireCube (Vector3.forward, new Vector3 (2, 2, 0));
            Gizmos.color = new Color (1, 0, 0, 1);
            Gizmos.DrawWireCube (Vector3.zero, Vector3.one * 2);
        }
    }
}