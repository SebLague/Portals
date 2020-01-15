using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionTest : MonoBehaviour {

    public MeshFilter meshFilterA;
    public MeshFilter meshFilterB;
    public bool divideByW;
    [Range (0, 1)]
    public float projectPercent = 1;
    [Range (0, 1)]
    public float aspectCorrectPercent = 0;

    void Start () {

    }

    // Update is called once per frame
    void Update () {
        Camera cam = Camera.main;

        var meshA = meshFilterA.mesh;
        var meshB = meshFilterB.mesh;
        Vector3[] newVerts = meshB.vertices;

        // to cam/view space

        // to projection space
        for (int i = 0; i < meshB.vertices.Length; i++) {
            //var worldVert = (Vector3)(meshFilterA.transform.localToWorldMatrix * meshA.vertices[i]);
            var worldVert = meshFilterA.transform.TransformPoint (meshA.vertices[i]);
            var viewSpaceVert = cam.worldToCameraMatrix * worldVert;

            // aspect correct test:
            var centreView = cam.worldToCameraMatrix * meshFilterA.transform.position;
            float s = Mathf.Lerp (1, Screen.width / (float)Screen.height, aspectCorrectPercent);
            var corrected = new Vector3 ((viewSpaceVert.x - centreView.x) * s + centreView.x, viewSpaceVert.y, viewSpaceVert.z);

            //var projectionSpaceVert = cam.projectionMatrix * viewSpaceVert;
            var projectionSpaceVert = cam.projectionMatrix * corrected;

            newVerts[i] = projectionSpaceVert / ((divideByW) ? projectionSpaceVert.w : 1);
            newVerts[i] = Vector3.Lerp (worldVert, newVerts[i], projectPercent);

        }
        meshB.vertices = newVerts;

    }

    void OnDrawGizmos () {
        Gizmos.DrawWireCube (Vector3.forward, new Vector3 (Mathf.Lerp (1, Screen.width / (float)Screen.height, aspectCorrectPercent), 1, 0));
    }
}