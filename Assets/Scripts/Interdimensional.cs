using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interdimensional : MonoBehaviour {

    public GameObject graphicObject;
    [HideInInspector]
    public GameObject mirrorGraphicObject;

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