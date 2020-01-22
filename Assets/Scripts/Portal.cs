using UnityEngine;
public class Portal : MonoBehaviour {

    public Portal linkedPortal;
    public MeshRenderer screen;
    Camera playerCam;
    Camera portalCam;
    RenderTexture viewTexture;

    void Awake() {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera>();
        portalCam.enabled = false;
    }

    void CreateViewTexture() {
        if (viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height) {
            if (viewTexture != null) {
                viewTexture.Release();
            }
            viewTexture = new RenderTexture(Screen.width, Screen.height, 0);
            // Render the view from the portal camera to the view texture
            portalCam.targetTexture = viewTexture;
            // Display the view texture on the screen of the linked portal
            linkedPortal.screen.material.SetTexture("_MainTex", viewTexture);
        }
    }

    // Called just before player camera is rendered
    public void Render() {
        screen.enabled = false;
        CreateViewTexture();

        // Make portal cam position and rotation the same relative to this portal as player cam relative to linked portal
        var m = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * playerCam.transform.localToWorldMatrix;
        portalCam.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);

        // Render the camera
        portalCam.Render();

        screen.enabled = true;
    }
}