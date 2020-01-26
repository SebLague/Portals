using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SliceTest : MonoBehaviour {

    public MeshRenderer sliceRenderer;

    void Update () {
        if (sliceRenderer && sliceRenderer.sharedMaterials != null) {
            for (int i = 0; i < sliceRenderer.sharedMaterials.Length; i++) {
                sliceRenderer.sharedMaterials[i].SetVector ("sliceNormal", transform.forward);
                sliceRenderer.sharedMaterials[i].SetVector ("sliceCentre", transform.position);
            }
        }
    }
}