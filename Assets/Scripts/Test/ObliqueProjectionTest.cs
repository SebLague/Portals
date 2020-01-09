using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObliqueProjectionTest : MonoBehaviour {

    public Transform plane;

    void LateUpdate () {

        // From http://tomhulton.blogspot.com/2015/08/portal-rendering-with-offscreen-render.html
        if (plane) {
            Camera cam = Camera.main;

            int dot = (Vector3.Dot (cam.transform.forward, plane.forward) < 0) ? -1 : 1;
            print ("dot: " + dot);
            // Calculate clip plane for portal (for culling of objects in-between destination camera and portal)
            Vector4 clipPlaneWorldSpace =
                new Vector4 (
                    plane.forward.x * dot,
                    plane.forward.y * dot,
                    plane.forward.z * dot,
                    Vector3.Dot (plane.position, -plane.forward * dot));

            Vector4 clipPlaneCameraSpace =
                Matrix4x4.Transpose (Matrix4x4.Inverse (cam.worldToCameraMatrix)) * clipPlaneWorldSpace;

            // Update projection based on new clip plane
            // Note: http://aras-p.info/texts/obliqueortho.html and http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
            cam.projectionMatrix = cam.CalculateObliqueMatrix (clipPlaneCameraSpace);
        }
    }
}