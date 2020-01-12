using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTest : MonoBehaviour {
    // Start is called before the first frame update
    void Start () {

    }

    // Update is called once per frame
    void Update () {

    }

    void OnPreRender () {
        Debug.Log ("Render: " + gameObject.name + "  " + GetComponent<Camera> ().targetTexture.name + "  " + RenderTexture.active.name);
        Graphics.SetRenderTarget (RenderTexture.active, 0, CubemapFace.Unknown, 1);
    }
}