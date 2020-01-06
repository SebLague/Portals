using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PortalCam : MonoBehaviour {

    public Transform myPortal;
    public Transform otherPortal;
    Camera mainCam;
    Camera portalCam;

    void Start () {
        portalCam = GetComponentInChildren<Camera> ();
        mainCam = Camera.main;
        //portalCam.transform.localPosition = mainCam.transform.localPosition;
    }

    void LateUpdate () {
        Vector3 linkedPortalOffset = (otherPortal.position - mainCam.transform.position);

        transform.position = myPortal.position - linkedPortalOffset;
        transform.rotation = mainCam.transform.rotation;
        //portalCam.transform.localRotation = mainCam.transform.localRotation;

        //transform.position = mainCam.transform.position;
        // transform.rotation = mainCam.transform.rotation;

        /*
        Plane portalPlane = new Plane (otherPortal.forward, otherPortal.position);

        Ray mainCamRay = new Ray (mainCam.transform.position, mainCam.transform.forward);
        float dstToPortalPlane;
        portalPlane.Raycast (mainCamRay, out dstToPortalPlane);

        Vector3 p = mainCamRay.GetPoint (dstToPortalPlane);
        Debug.DrawRay (p, mainCamRay.direction, Color.red);

        transform.position = otherPortal.position;
        transform.rotation = mainCam.transform.rotation;
        */

    }
}