using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {

    RenderTexture displayTexture;
    public Portal linkedPortal;
    public MeshRenderer portalMesh;
    Camera playerCam;
    Camera portalCam;
    protected Plane collisionPlane;

    void Awake () {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera> ();
    }

    protected virtual void Start () {
        collisionPlane = new Plane (transform.forward, transform.position);
    }

    public void SetRenderTarget (RenderTexture targetTexture) {
        portalCam.targetTexture = targetTexture;
    }

    protected virtual void LateUpdate () {
        UpdateRenderTexture ();
        Vector3 playerOffsetToLinkedPortal = playerCam.transform.position - linkedPortal.transform.position;
        Vector3 localOffset = linkedPortal.transform.InverseTransformVector (playerOffsetToLinkedPortal);
        
        portalCam.transform.position = transform.position + transform.TransformVector (playerOffsetToLinkedPortal);
        portalCam.transform.rotation = playerCam.transform.rotation;

    }

    void UpdateRenderTexture () {
        if (displayTexture == null || displayTexture.width != Screen.width || displayTexture.height != Screen.height) {
            if (displayTexture != null) {
                displayTexture.Release ();
            }
            displayTexture = new RenderTexture (Screen.width, Screen.height, 0);
            portalMesh.material.SetTexture ("_MainTex", displayTexture);
            linkedPortal.SetRenderTarget (displayTexture);
        }
    }

    void OnValidate () {
        if (linkedPortal != null && linkedPortal.linkedPortal != this) {
            linkedPortal.linkedPortal = this;
        }
    }
}