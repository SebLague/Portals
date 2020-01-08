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
        Vector3 mirrorViewPos = CalculateMirrorViewPos ();

        Gizmos.color = Color.blue;
        Gizmos.DrawRay (portalA.position, portalA.forward);
        Gizmos.DrawRay (portalB.position, portalB.forward);
        Gizmos.DrawRay (viewPoint.position, viewPoint.forward);
        Gizmos.DrawRay (mirrorViewPos, portalB.TransformDirection (portalA.InverseTransformDirection (viewPoint.forward)));

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere (viewPoint.position, .2f);
        Gizmos.DrawLine (viewPoint.position, portalA.position);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere (mirrorViewPos, .2f);
        Gizmos.DrawLine (mirrorViewPos, portalB.position);

        Gizmos.color = Color.red;
        Vector3 worldOffsetToView = viewPoint.position - portalA.position;
        Vector3 localOffsetToView = Quaternion.Inverse (portalA.rotation) * worldOffsetToView;
        Gizmos.DrawRay (portalA.position, localOffsetToView);
    }
}