using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class PortalPhysicsObject : PortalTraveller {

    public float force = 10;
    new Rigidbody rigidbody;

    void Start () {
        rigidbody = GetComponent<Rigidbody> ();
    }

    void Update () {
        if (Input.GetKeyDown (KeyCode.L)) {
            GetComponent<Rigidbody> ().AddForce ((transform.forward + transform.up * .5f).normalized * force, ForceMode.Impulse);
        }
    }

    public override void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
        base.Teleport (fromPortal, toPortal, pos, rot);
        rigidbody.velocity = toPortal.TransformVector (fromPortal.InverseTransformVector (rigidbody.velocity));
    }
}