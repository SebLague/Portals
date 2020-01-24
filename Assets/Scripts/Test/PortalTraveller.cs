using UnityEngine;

public class PortalTraveller : MonoBehaviour {

    public MeshRenderer graphic;
    MeshRenderer graphicClone;
    public Vector3 previousPortalOffset { get; set; }

    public virtual void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
        transform.position = pos;
        transform.rotation = rot;
    }

    // Called when first touches portal
    public virtual void EnterPortalThreshold () {
        if (graphicClone == null) {
            graphicClone = Instantiate (graphic);
            graphicClone.transform.parent = graphic.transform.parent;
        } else {
            graphicClone.enabled = true;
        }
    }

    // Called when no longer touching portal
    public virtual void ExitPortalThreshold () {
        graphicClone.enabled = false;
    }

    public virtual void SetClonePositionAndRotation (Vector3 pos, Quaternion rot) {
        graphicClone.transform.SetPositionAndRotation (pos, rot);

    }

    public virtual void UpdateSlice (Transform portal, Transform linkedPortal) {
        var relativePosition = graphic.transform.position - portal.position;
        bool enteringPositiveSide = Vector3.Dot (portal.forward, relativePosition) > 0;
        int side = (enteringPositiveSide) ? -1 : 1;

        for (int i = 0; i < graphic.materials.Length; i++) {
            graphic.materials[i].SetVector ("sliceCentre", portal.position);
            graphic.materials[i].SetVector ("sliceNormal", portal.forward * side);
            graphicClone.materials[i].SetVector ("sliceCentre", linkedPortal.position);
            graphicClone.materials[i].SetVector ("sliceNormal", linkedPortal.forward * -side);

        }
    }

}