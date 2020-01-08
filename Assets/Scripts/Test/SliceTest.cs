using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SliceTest : MonoBehaviour {

    public MeshRenderer sliceRenderer;

    void Update () {
        sliceRenderer.sharedMaterial.SetVector ("sliceNormal", transform.forward);
        sliceRenderer.sharedMaterial.SetVector ("sliceCentre", transform.position);
    }
}