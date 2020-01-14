using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Master : MonoBehaviour {

    public Shader shader;
    public Transform box;
    Material mat;

    void OnRenderImage (RenderTexture src, RenderTexture dest) {
        if (mat == null || mat.shader != shader) {
            mat = new Material (shader);
        }
        FindObjectOfType<PortalScreenRender> ().ManualUpdate ();
        mat.SetTexture ("portalTexture", FindObjectOfType<PortalScreenRender> ().texture);
        mat.SetTexture ("portalDepthTexture", FindObjectOfType<PortalScreenRender> ().depthTexture);

        //mat.SetMatrix ("boxMatrix", Matrix4x4.Rotate(Quaternion.Inverse(box.rotation)));

        Graphics.Blit (src, dest, mat);
    }

}