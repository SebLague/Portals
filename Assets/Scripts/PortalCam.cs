using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PortalCam : MonoBehaviour {

    public Transform homePortal;
    public Transform linkedPortal;
    Camera mainCam;
    Camera portalCam;

    void Start () {
        mainCam = Camera.main;
    }

    void LateUpdate () {
        Vector3 linkedPortalOffset = (linkedPortal.position - mainCam.transform.position);

        transform.position = homePortal.position - linkedPortalOffset;
        transform.rotation = mainCam.transform.rotation;

    }
}