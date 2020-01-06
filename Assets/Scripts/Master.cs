using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Master : MonoBehaviour {

    public Shader shader;
    public RenderTexture portalTexture;
    public Color tint;
    public BoxCollider portalVolumeA;
    Material mat;

    void OnRenderImage (RenderTexture src, RenderTexture dest) {
        if (mat == null || mat.shader != shader) {
            mat = new Material (shader);
        }
        mat.SetTexture ("portalTexture", portalTexture);
        mat.SetColor ("tint", tint);
        
        Graphics.Blit (src, dest, mat);
    }
}