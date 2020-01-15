using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionTest : MonoBehaviour {

    public MeshFilter meshFilterA;
    public MeshFilter meshFilterB;
    public bool divideByW;
    public bool divideByZ;
    public bool dropZ;
    [Range (0, 1)]
    public float projectPercent = 1;
    [Range (0, 1)]
    public float aspectCorrectPercent = 0;
    Vector3[] vertsA;

    void Awake () {
        vertsA = meshFilterA.mesh.vertices;
        //Debug.Log (Camera.main.projectionMatrix);
    }

    void Update () {
        RunOptim ();
    }

    void RunOptim () {
        Camera cam = Camera.main;

        Vector3[] newVerts = new Vector3[vertsA.Length];
        var worldToCam = cam.worldToCameraMatrix;
        var projectionMatrix = cam.projectionMatrix;

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
            //var projectionSpaceVert = projectionMatrix * corrected;
            //newVerts[i] = projectionSpaceVert / projectionSpaceVert.w;

            if (divideByZ) {
                newVerts[i] = projectionSpaceVert / projectionSpaceVert.z;
            }
            if (divideByW) {
                newVerts[i] = projectionSpaceVert / projectionSpaceVert.w;
            }
            if (divideByZ && divideByW) {
                newVerts[i] = projectionSpaceVert / projectionSpaceVert.w / projectionSpaceVert.z;
            }
            if (dropZ) {
                newVerts[i] = new Vector3 (newVerts[i].x, newVerts[i].y, 1);
            }
            newVerts[i] = Vector3.Lerp (worldVert, newVerts[i], projectPercent);

        }
        meshFilterB.mesh.vertices = newVerts;
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