using System.Collections.Generic;
using UnityEngine;

public class PortalTraveller : MonoBehaviour {

    public GameObject graphicsObject;
    public GameObject graphicsClone { get; set; }
    public Vector3 previousOffsetFromPortal { get; set; }

    Material[] originalMaterials;
    Material[] cloneMaterials;

    public virtual void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
        transform.position = pos;
        transform.rotation = rot;
    }

    // Called when first touches portal
    public virtual void EnterPortalThreshold () {
        if (graphicsClone == null) {
            graphicsClone = Instantiate (graphicsObject);
            graphicsClone.transform.parent = graphicsObject.transform.parent;
            graphicsClone.transform.localScale = graphicsObject.transform.localScale;
            originalMaterials = GetMaterials (graphicsObject);
            cloneMaterials = GetMaterials (graphicsClone);
        } else {
            graphicsClone.SetActive (true);
        }
    }

    // Called once no longer touching portal (excluding when teleporting)
    public virtual void ExitPortalThreshold () {
        graphicsClone.SetActive (false);
    }

    public virtual void UpdateSlice (Transform portal, Transform linkedPortal) {
        var relativePosition = graphicsObject.transform.position - portal.position;
        bool enteringPositiveSide = Vector3.Dot (portal.forward, relativePosition) > 0;
        int side = (enteringPositiveSide) ? -1 : 1;

        for (int i = 0; i < originalMaterials.Length; i++) {
            originalMaterials[i].SetVector ("sliceCentre", portal.position);
            originalMaterials[i].SetVector ("sliceNormal", portal.forward * side);
            cloneMaterials[i].SetVector ("sliceCentre", linkedPortal.position);
            cloneMaterials[i].SetVector ("sliceNormal", linkedPortal.forward * -side);

        }
    }

    Material[] GetMaterials (GameObject g) {
        var renderers = g.GetComponentsInChildren<MeshRenderer> ();
        var matList = new List<Material> ();
        foreach (var renderer in renderers) {
            foreach (var mat in renderer.materials) {
                matList.Add (mat);
            }
        }
        return matList.ToArray ();
    }
}