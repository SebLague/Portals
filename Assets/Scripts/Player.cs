using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    Vector3 posOld;

    public BoxCollider portalCollider;
    public Transform linkedPortal;
    FPSController controller;
    Camera viewCam;

    void Start () {
        viewCam = Camera.main;
        posOld = GetCamNearClipCentre ();
        controller = GetComponent<FPSController> ();
    }

    Vector3 GetCamNearClipCentre () {
        return viewCam.transform.position + viewCam.transform.forward * viewCam.nearClipPlane;
    }

    void Update () {
        return;
        if (portalCollider == null || portalCollider == null) {
            return;
        }
        Vector3 posNew = GetCamNearClipCentre ();
        Plane plane = new Plane (portalCollider.transform.forward, portalCollider.transform.position);
        float colliderDepth = portalCollider.size.z;

        if (!plane.SameSide (posOld, posNew)) {
            float dstTravelled = (posNew - posOld).magnitude;
            Vector3 dir = (posNew - posOld) / dstTravelled;

            if (portalCollider.Raycast (new Ray (posOld - dir * colliderDepth, dir), out _, dstTravelled + colliderDepth)) {
                Vector3 portalOffset = transform.position - portalCollider.transform.position;
                Debug.Log ("Went through portal (player): " + (linkedPortal.transform.position + portalOffset));
                //controller.Teleport (linkedPortal.position + portalOffset);

            } else {
                Debug.Log ("Went passed portal");
            }
        }
        posOld = GetCamNearClipCentre ();
    }
}