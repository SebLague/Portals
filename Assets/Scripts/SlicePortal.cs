using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicePortal : Portal {

    List<Interdimensional> trackedEntities;

    /*
    protected override void Start () {
        base.Start ();
        trackedEntities = new List<Interdimensional> ();
    }

    protected override void LateUpdate () {
        base.LateUpdate ();

        foreach (var entity in trackedEntities) {
            if (entity != null) {
                UpdateEntity (entity);
            }
        }

    }

    void UpdateEntity (Interdimensional entity) {

        var relativePosition = entity.graphicObject.transform.position - transform.position;
        entity.mirrorGraphicObject.transform.position = linkedPortal.transform.position + relativePosition;

        bool enteringPositiveSide = Vector3.Dot (transform.forward, relativePosition) > 0;
        int side = (enteringPositiveSide) ? -1 : 1;
        entity.SetSliceParams (transform.forward * side, transform.position, linkedPortal.transform.forward * -side, linkedPortal.transform.position);

        // Centre of entity has moved across portal
        if (!collisionPlane.SameSide (entity.transform.position, entity.positionOld)) {
            entity.teleportedToLinkedPortalLastFrame = true;
            entity.mirrorGraphicObject.transform.position = entity.transform.position;
            entity.transform.position = linkedPortal.transform.position + relativePosition;
        } else {
            entity.positionOld = entity.transform.position;
        }

    }

    void OnTriggerEnter (Collider c) {
        if (c.gameObject.GetComponent<Interdimensional> ()) {
            var entity = c.gameObject.GetComponent<Interdimensional> ();
            trackedEntities.Add (entity);
            entity.EnterPortal ();

        }
    }

    void OnTriggerExit (Collider c) {
        if (c.gameObject.GetComponent<Interdimensional> ()) {
            var entity = c.gameObject.GetComponent<Interdimensional> ();
            entity.ExitPortal ();
            trackedEntities.Remove (entity);
        }

    }
    */
}