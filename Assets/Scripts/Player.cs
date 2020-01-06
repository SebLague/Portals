using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    Vector3 posOld;

    public BoxCollider portalCollider;

    void Start () {
        posOld = transform.position;
    }

    void Update () {
        Vector3 posNew = transform.position;
        Plane plane = new Plane (portalCollider.transform.forward, portalCollider.transform.position);
        float colliderDepth = portalCollider.size.z;

        if (!plane.SameSide (posOld, posNew)) {
            float dstTravelled = (posNew - posOld).magnitude;
            Vector3 dir = (posNew - posOld) / dstTravelled;

            if (portalCollider.Raycast (new Ray (posOld - dir * colliderDepth, dir), out _, dstTravelled + colliderDepth)) {
                Debug.Log ("Went through portal!");
            } else {
                Debug.Log ("Went passed portal");
            }
        }
        posOld = posNew;
    }
}