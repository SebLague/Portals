using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class PortalRenderer : MonoBehaviour {

    public Shader shader;
    public Transform box;
    public Portal portal;
    Material mat;

    RenderTexture screenTexture;

    void OnPreCull () {
        var portals = FindObjectsOfType<Portal> ();
        //var portals = new Portal[] { portal };
        int numPortals = portals.Length;

        if (screenTexture == null || screenTexture.width != Screen.width || screenTexture.height != Screen.height || screenTexture.volumeDepth != numPortals) {
            if (screenTexture != null) {
                screenTexture.Release ();
            }
            screenTexture = new RenderTexture (Screen.width, Screen.height, 0);

            screenTexture.dimension = UnityEngine.Rendering.TextureDimension.Tex2DArray;
            screenTexture.volumeDepth = numPortals;
            screenTexture.Create ();
            screenTexture.name = "myTex";
        }

        for (int i = 0; i < numPortals; i++) {
            portals[i].Render (screenTexture, i);
        }
    }

    void OnRenderImage (RenderTexture src, RenderTexture dest) {
        if (mat == null || mat.shader != shader) {
            mat = new Material (shader);
        }

        var portals = FindObjectsOfType<Portal> ();
        //var portals = new Portal[] { portal };
        var screenMatrices = new Matrix4x4[portals.Length];
        var screenTextures = new RenderTexture[portals.Length];
        for (int i = 0; i < portals.Length; i++) {
            screenMatrices[i] = portals[i].screenMesh.worldToLocalMatrix;
            screenTextures[i] = portals[i].screenTexture;
        }

        mat.SetMatrixArray ("screenMatrices", screenMatrices);

        mat.SetTexture ("screenTextures", screenTexture);
        mat.SetInt ("numScreens", portals.Length);

        //mat.SetTexture ("portalTexture", screenTexture);
        //mat.SetColor ("tint", tint);
        //mat.SetMatrix ("boxMatrix", box.worldToLocalMatrix);
        //mat.SetMatrix ("boxMatrix", Matrix4x4.Rotate(Quaternion.Inverse(box.rotation)));

        Graphics.Blit (src, dest, mat);
    }

}