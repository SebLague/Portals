using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationTest : MonoBehaviour {

    public Transform portalA;
    public Transform portalB;
    public Transform viewPoint;

    void Update () {

    }

    Vector3 CalculateMirrorViewPos () {
        Vector3 worldOffsetToView = viewPoint.position - portalA.position;
        Vector3 localOffsetToView = Quaternion.Inverse (portalA.rotation) * worldOffsetToView;

        return portalB.position + portalB.rotation * localOffsetToView;
    }

    void OnDrawGizmos () {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay (portalA.position, portalA.forward);
        Gizmos.DrawRay (portalB.position, portalB.forward);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere (viewPoint.position, .2f);
        Gizmos.DrawLine (viewPoint.position, portalA.position);

        Gizmos.color = Color.black;
        Vector3 mirrorViewPos = CalculateMirrorViewPos ();
        Gizmos.DrawWireSphere (mirrorViewPos, .2f);
        Gizmos.DrawLine (mirrorViewPos, portalB.position);

        Gizmos.color = Color.red;
        Vector3 worldOffsetToView = viewPoint.position - portalA.position;
        Vector3 localOffsetToView = Quaternion.Inverse (portalA.rotation) * worldOffsetToView;
        Gizmos.DrawRay (portalA.position, localOffsetToView);
    }
}