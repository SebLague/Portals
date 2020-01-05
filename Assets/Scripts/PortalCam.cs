using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCam : MonoBehaviour {

    public Transform myPortal;
    public Transform otherPortal;
    Camera mainCam;

    void Start () {
        mainCam = Camera.main;
    }

    void LateUpdate () {
        Vector3 linkedPortalOffset = (otherPortal.position - mainCam.transform.position);

        Plane portalPlane = new Plane (otherPortal.forward, otherPortal.position);

        Ray mainCamRay = new Ray (mainCam.transform.position, mainCam.transform.forward);
        float dstToPortalPlane;
        portalPlane.Raycast (mainCamRay, out dstToPortalPlane);

        Vector3 p = mainCamRay.GetPoint (dstToPortalPlane);
        Debug.DrawRay (p, mainCamRay.direction, Color.red);

        transform.position = otherPortal.position;
        transform.rotation = mainCam.transform.rotation;

    }
}