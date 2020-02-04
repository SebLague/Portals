using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class PortalPhysicsObject : PortalTraveller {

    public float force = 10;
    new Rigidbody rigidbody;

    void Awake () {
        rigidbody = GetComponent<Rigidbody> ();
        graphicsObject.GetComponent<MeshRenderer> ().material.color = new Color (Random.value, Random.value, Random.value);
    }

    public override void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
        base.Teleport (fromPortal, toPortal, pos, rot);
        rigidbody.velocity = toPortal.TransformVector (fromPortal.InverseTransformVector (rigidbody.velocity));
        rigidbody.angularVelocity = toPortal.TransformVector (fromPortal.InverseTransformVector (rigidbody.angularVelocity));
    }
}