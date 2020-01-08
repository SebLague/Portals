using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interdimensional : MonoBehaviour {

    public GameObject graphicObject;

    [HideInInspector]
    public GameObject mirrorGraphicObject;

    public Vector3 positionOld { get; set; }
    public bool teleportedToLinkedPortalLastFrame { get; set; }

    public void EnterPortal () {
        if (mirrorGraphicObject == null) {
            mirrorGraphicObject = Instantiate (graphicObject);
        }
        positionOld = transform.position;
    }

    public void ExitPortal () {
        if (teleportedToLinkedPortalLastFrame) {
            teleportedToLinkedPortalLastFrame = false;
        } else {
            SetSliceParams (Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero);
            GameObject.Destroy (mirrorGraphicObject);
        }

    }

    public virtual void SetSliceParams (Vector3 sliceNormal, Vector3 slicePoint, Vector3 mirrorSliceNormal, Vector3 mirrorSlicePoint) {
        Slice (sliceNormal, slicePoint, graphicObject);
        Slice (mirrorSliceNormal, mirrorSlicePoint, mirrorGraphicObject);
    }

    void Slice (Vector3 sliceNormal, Vector3 slicePoint, GameObject sliceObject) {
        var renderers = sliceObject.GetComponentsInChildren<MeshRenderer> ();
        foreach (var r in renderers) {
            r.material.SetVector ("sliceNormal", sliceNormal);
            r.material.SetVector ("sliceCentre", slicePoint);
        }
    }
}