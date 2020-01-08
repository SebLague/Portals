using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicePortal : Portal {

    PortalEntity entity;

    protected override void LateUpdate () {
        base.LateUpdate ();

        if (entity != null) {
            var relativePosition = entity.interdimensionalObject.graphicObject.transform.position - transform.position;
            entity.interdimensionalObject.mirrorGraphicObject.transform.position = linkedPortal.transform.position + relativePosition;

            bool enteringPositiveSide = Vector3.Dot (transform.forward, relativePosition) > 0;
            int a = (enteringPositiveSide) ? -1 : 1;
            entity.interdimensionalObject.SetSliceParams (transform.forward * a, transform.position, linkedPortal.transform.forward * -a, linkedPortal.transform.position);

            if (!collisionPlane.SameSide (entity.Position, entity.positionOld)) {
                entity.transportedToLinkedPortal = true;
                GameObject.Destroy (entity.interdimensionalObject.mirrorGraphicObject);
                entity.interdimensionalObject.transform.position = linkedPortal.transform.position + relativePosition;
                entity = null;
            } else {
                entity.positionOld = entity.interdimensionalObject.transform.position;
            }
        }
    }

    void OnTriggerEnter (Collider c) {
        if (c.gameObject.GetComponent<Interdimensional> ()) {
            entity = new PortalEntity (c.gameObject.GetComponent<Interdimensional> ());
            entity.interdimensionalObject.mirrorGraphicObject = Instantiate (entity.interdimensionalObject.graphicObject);
        }
    }

    void OnTriggerExit (Collider c) {
        if (entity != null) {
            if (!entity.transportedToLinkedPortal) {
                entity.interdimensionalObject.SetSliceParams (Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero);
                GameObject.Destroy (entity.interdimensionalObject.mirrorGraphicObject);
            }
            entity = null;
        }
    }

    class PortalEntity {
        public Interdimensional interdimensionalObject;
        public Vector3 positionOld;
        public bool transportedToLinkedPortal;

        public PortalEntity (Interdimensional interdimensionalObject) {
            this.interdimensionalObject = interdimensionalObject;
            positionOld = interdimensionalObject.transform.position;
        }

        public Vector3 Position {
            get {
                return interdimensionalObject.transform.position;
            }
        }

        public void Update () {

        }
    }
}