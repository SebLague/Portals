using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Portal : MonoBehaviour {

    public RenderTexture displayTexture;
    public Portal linkedPortal;
    Camera playerCam;
    Camera portalCam;

    void Awake () {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera> ();
    }

    void Start () {
        linkedPortal.SetRenderTarget (displayTexture);
    }

    public void SetRenderTarget (RenderTexture targetTexture) {
        portalCam.targetTexture = targetTexture;
    }

    void LateUpdate () {
        Vector3 playerOffsetToLinkedPortal = playerCam.transform.position - linkedPortal.transform.position;

        portalCam.transform.position = transform.position - playerOffsetToLinkedPortal;
        portalCam.transform.rotation = playerCam.transform.rotation;
    }
}