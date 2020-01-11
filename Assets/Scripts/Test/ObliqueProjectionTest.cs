using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObliqueProjectionTest : MonoBehaviour {

    public Transform plane;
    public float buffer = 0.01f;
    public Camera extra;

    void LateUpdate () {
        // http://tomhulton.blogspot.com/2015/08/portal-rendering-with-offscreen-render.html
        if (plane) {
            Camera cam = Camera.main;

            int dot = (Vector3.Dot (cam.transform.forward, plane.forward) < 0) ? -1 : 1;

            // https://www.csharpcodi.com/vs2/805/Unity-AudioVisualization-/Assets/SampleAssets/Environment/Water/Water/Scripts/PlanarReflection.cs/
            Vector3 camSpacePos = cam.worldToCameraMatrix.MultiplyPoint (plane.position - plane.forward * buffer * dot);
            Vector3 camSpaceNormal = cam.worldToCameraMatrix.MultiplyVector (plane.forward).normalized * dot;
            Vector4 clipPlaneCameraSpace = new Vector4 (camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, -Vector3.Dot (camSpacePos, camSpaceNormal));

            // Update projection based on new clip plane
            // Note: http://aras-p.info/texts/obliqueortho.html and http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
            cam.projectionMatrix = extra.CalculateObliqueMatrix (clipPlaneCameraSpace);

            // dst test
            Plane p = new Plane (plane.forward, plane.position);
            Debug.Log (p.distance + "  dst centre: " + (plane.position.magnitude) + " Dot: " + Vector3.Dot (plane.position, -plane.forward));
        }
    }
}