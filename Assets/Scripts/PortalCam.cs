using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCam : MonoBehaviour {

    public Transform portal;
    Camera mainCam;

    void Start () {
        mainCam = Camera.main;
    }

    void LateUpdate () {
        transform.position= mainCam.transform.position;
        transform.rotation= mainCam.transform.rotation;
        
    }
}