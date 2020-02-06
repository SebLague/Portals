using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Fog : MonoBehaviour {

    public Shader shader;
    public Vector4 testParams;

    // Internal
    [HideInInspector]
    public Material material;

    public Color colA;
    public Color colB;

    public Color colC;
    public Color colD;

    [ImageEffectOpaque]
    private void OnRenderImage (RenderTexture src, RenderTexture dest) {

        // Validate inputs
        if (material == null || material.shader != shader) {
            material = new Material (shader);
        }

        material.SetVector ("params", testParams);
        material.SetColor ("colA", colA);
        material.SetColor ("colB", colB);
        material.SetColor ("colC", colC);
        material.SetColor ("colD", colD);
        // Bit does the following:
        // - sets _MainTex property on material to the source texture
        // - sets the render target to the destination texture
        // - draws a full-screen quad
        // This copies the src texture to the dest texture, with whatever modifications the shader makes
        Graphics.Blit (src, dest, material);
    }
}