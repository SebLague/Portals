using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Master : MonoBehaviour {

    public Shader shader;
    public Transform box;
    public Portal portal;
    Material mat;

    void OnRenderImage (RenderTexture src, RenderTexture dest) {
        if (mat == null || mat.shader != shader) {
            mat = new Material (shader);
        }
        mat.SetTexture ("portalTexture", portal.displayTexture);
        //mat.SetColor ("tint", tint);
        mat.SetMatrix ("boxMatrix", box.worldToLocalMatrix);
        //mat.SetMatrix ("boxMatrix", Matrix4x4.Rotate(Quaternion.Inverse(box.rotation)));

        Graphics.Blit (src, dest, mat);
    }

}