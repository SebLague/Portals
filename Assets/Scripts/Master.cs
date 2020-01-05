using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Master : MonoBehaviour {

    public Shader shader;
    public RenderTexture portalTexture;
    Material mat;

    void OnRenderImage (RenderTexture src, RenderTexture dest) {
        if (mat == null || mat.shader != shader) {
            mat = new Material (shader);
        }
        mat.SetTexture ("portalTexture", portalTexture);
        Graphics.Blit (src, dest, mat);
    }
}