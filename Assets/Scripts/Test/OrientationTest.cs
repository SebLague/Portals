using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationTest : MonoBehaviour {

    public Transform portalA;
    public Transform portalB;
    public Transform viewPoint;
    public Transform mirrorViewPoint;

    public int perfIterations = 1000;

    void Start () {
        MeasureTom ();
        MeasureMatrix ();

    }

    void MeasureMatrix () {
        var sw = System.Diagnostics.Stopwatch.StartNew ();
        var m1 = portalB.localToWorldMatrix;
        var m2 = portalA.worldToLocalMatrix;
        var m3 = viewPoint.localToWorldMatrix;
        for (int i = 0; i < perfIterations; i++) {
            //mirrorViewPoint.position = m1.MultiplyPoint3x4 (m2.MultiplyPoint3x4 (viewPoint.position));
            // var mirrorMatrix = portalB.localToWorldMatrix * portalA.worldToLocalMatrix * viewPoint.localToWorldMatrix;
            var mirrorMatrix = m1 * m2 * m3;
            //mirrorViewPoint.position = mirrorMatrix.GetColumn (3);
            //mirrorViewPoint.SetPositionAndRotation (mirrorMatrix.GetColumn (3), mirrorMatrix.rotation);
            //mirrorViewPoint.position = mirrorMatrix.GetColumn (3);
        }
        print ("matrix: " + sw.ElapsedMilliseconds);
    }

    void MeasureTom () {
        var sw = System.Diagnostics.Stopwatch.StartNew ();
        for (int i = 0; i < perfIterations; i++) {
            Vector3 cameraPositionInSourceSpace = portalA.InverseTransformPoint (viewPoint.position);
            mirrorViewPoint.position = portalB.TransformPoint (cameraPositionInSourceSpace);
            Quaternion cameraRotationInSourceSpace = Quaternion.Inverse (portalA.rotation) * viewPoint.rotation;
            mirrorViewPoint.rotation = portalA.rotation * cameraRotationInSourceSpace;
        }
        print ("tom: " + sw.ElapsedMilliseconds);

    }

    void Update () {

    }

    Vector3 CalculateMirrorViewPos () {

        // Matrix test
        var mirrorMatrix = portalB.localToWorldMatrix * portalA.worldToLocalMatrix * viewPoint.localToWorldMatrix;
        mirrorViewPoint.SetPositionAndRotation (mirrorMatrix.GetColumn (3), mirrorMatrix.rotation);
        return mirrorMatrix.GetColumn (3);
        return portalB.localToWorldMatrix.MultiplyPoint3x4 (portalA.worldToLocalMatrix.MultiplyPoint3x4 (viewPoint.position));
        // Tom:
        Vector3 cameraPositionInSourceSpace = portalA.InverseTransformPoint (viewPoint.position);
        return portalB.TransformPoint (cameraPositionInSourceSpace);
        // Quaternion cameraRotationInSourceSpace = Quaternion.Inverse (Source.rotation) * MainCamera.transform.rotation;
        //PortalCamera.transform.rotation = Destination.rotation * cameraRotationInSourceSpace;

        // My nonsense:
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
        Gizmos.DrawRay (mirrorViewPoint.position, mirrorViewPoint.forward);
        //Gizmos.DrawRay (mirrorViewPos, portalB.TransformDirection (portalA.InverseTransformDirection (viewPoint.forward)));

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere (viewPoint.position, .2f);
        Gizmos.DrawLine (viewPoint.position, portalA.position);

        Gizmos.DrawWireSphere (mirrorViewPoint.position, .2f);

        /*
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere (mirrorViewPos, .2f);
        Gizmos.DrawLine (mirrorViewPos, portalB.position);

        Gizmos.color = Color.red;
        Vector3 worldOffsetToView = viewPoint.position - portalA.position;
        Vector3 localOffsetToView = Quaternion.Inverse (portalA.rotation) * worldOffsetToView;
        Gizmos.DrawRay (portalA.position, localOffsetToView);
        */
    }
}