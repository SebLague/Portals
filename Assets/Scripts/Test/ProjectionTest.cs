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
    [Range (0, 1)]
    public float aspectCorrectPercent = 0;
    Vector3[] vertsA;
    Vector3[] normalsA;

    void Awake () {
        vertsA = meshFilterA.mesh.vertices;
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

        Vector3[] newVerts = new Vector3[vertsA.Length];
        Vector3[] newNormals = new Vector3[normalsA.Length];
        var worldToCam = cam.worldToCameraMatrix;
        var projectionMatrix = cam.projectionMatrix;
        var b = cam.projectionMatrix * worldToCam * meshFilterA.transform.localToWorldMatrix;

        // to projection space
        for (int i = 0; i < vertsA.Length; i++) {
            //var worldVert = meshFilterA.transform.TransformPoint (vertsA[i]);
            Vector4 worldVert = meshFilterA.transform.localToWorldMatrix * new Vector4 (vertsA[i].x, vertsA[i].y, vertsA[i].z, 1);
            Vector4 viewSpaceVert = worldToCam * worldVert;

            // aspect correct test:
            //var centreView = worldToCam * meshFilterA.transform.position;
            //float s = Mathf.Lerp (1, Screen.width / (float) Screen.height, aspectCorrectPercent);
            //var corrected = new Vector3 ((viewSpaceVert.x - centreView.x) * s + centreView.x, viewSpaceVert.y, viewSpaceVert.z);

            Vector4 projectionSpaceVert = cam.projectionMatrix * viewSpaceVert;
            projectionSpaceVert = b * new Vector4 (vertsA[i].x, vertsA[i].y, vertsA[i].z, 1);
            newNormals[i] = b * new Vector4 (normalsA[i].x, normalsA[i].y, normalsA[i].z, 0);
            //var projectionSpaceVert = projectionMatrix * corrected;
            //newVerts[i] = projectionSpaceVert / projectionSpaceVert.w;

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
       // meshFilterB.mesh.normals = newNormals;
    }

    void OnDrawGizmos () {
        var m = Gizmos.matrix;
        var cam = Camera.main;
        Gizmos.matrix = cam.transform.localToWorldMatrix;
        Gizmos.DrawFrustum (cam.transform.position, cam.fieldOfView, cam.farClipPlane, cam.nearClipPlane, cam.aspect);
        Gizmos.matrix = m;

        float s = Mathf.Lerp (1, Camera.main.pixelWidth / (float) Camera.main.pixelHeight, aspectCorrectPercent);
        Gizmos.color = new Color (1, 0, 0, 0.5f);
        Gizmos.DrawWireCube (Vector3.forward, new Vector3 (2, 2, 0));
        Gizmos.color = new Color (1, 1, 0, 0.5f);
        Gizmos.DrawWireCube (Vector3.zero, Vector3.one * 2);
    }
}