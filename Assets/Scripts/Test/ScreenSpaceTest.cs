using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSpaceTest : MonoBehaviour {

    public MeshFilter meshFilter;

    void Start () {

    }

    void Update () {
        var cam = Camera.main;
        var worldToCam = cam.worldToCameraMatrix;
        var projectionMatrix = cam.projectionMatrix;
        var b = cam.projectionMatrix * worldToCam * meshFilter.transform.localToWorldMatrix;

        var verts = meshFilter.mesh.vertices;
        var uvs = new Vector2[verts.Length];

        // to projection space
        for (int i = 0; i < verts.Length; i++) {

            var projectionSpaceVert = b * new Vector4 (verts[i].x, verts[i].y, verts[i].z, 1);

            projectionSpaceVert /= projectionSpaceVert.w;

            uvs[i] = new Vector2 (projectionSpaceVert.x*.5f+.5f, projectionSpaceVert.y*.5f+.5f);
        }
        meshFilter.mesh.uv = uvs;
    }
}