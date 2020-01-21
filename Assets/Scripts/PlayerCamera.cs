using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    void OnPreCull () {
        var portals = FindObjectsOfType<Portal> ();
        foreach (var p in portals) {
            CalculateScreenCoords (p);
            p.Render ();
        }
    }

    void CalculateScreenCoords (Portal p) {
        var meshFilter = p.portalMesh.GetComponent<MeshFilter> ();
        var cam = GetComponent<Camera> ();
        var worldToCam = cam.worldToCameraMatrix;
        var projectionMatrix = cam.projectionMatrix;
        var mvpMatrix = cam.projectionMatrix * worldToCam * meshFilter.transform.localToWorldMatrix;

        var verts = meshFilter.mesh.vertices;
        var uvs = new List<Vector4> (verts.Length);
        // to projection space
        for (int i = 0; i < verts.Length; i++) {

            var projectionSpaceVert = mvpMatrix * new Vector4 (verts[i].x, verts[i].y, verts[i].z, 1);
            uvs.Add (projectionSpaceVert);
        }

        meshFilter.mesh.SetUVs (0, uvs);
    }
}