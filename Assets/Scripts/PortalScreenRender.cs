using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScreenRender : MonoBehaviour {

    public RenderTexture texture { get; private set; }
    public RenderTexture depthTexture { get; private set; }

    void Start () {

    }

    void Update () {
        UpdateTexture ();
    }

    public void ManualUpdate () {
        UpdateTexture ();

        GetComponent<Camera> ().targetTexture = texture;
        GetComponent<Camera> ().Render ();
        GetComponent<Camera> ().targetTexture = depthTexture;
        GetComponent<Camera> ().Render ();
    }

    void UpdateTexture () {
        if (texture == null || texture.width != Screen.width || texture.height != Screen.height) {
            if (texture != null) {
                texture.Release ();
                depthTexture.Release ();
            }
            texture = new RenderTexture (Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
            texture.Create ();
            depthTexture = new RenderTexture (Screen.width, Screen.height, 24, RenderTextureFormat.Depth);
            depthTexture.Create ();
            //GetComponent<Camera> ().targetTexture = texture;
            // GetComponent<Camera> ().SetTargetBuffers (texture.colorBuffer, texture.depthBuffer);
        }
    }
}