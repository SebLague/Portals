using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicePortal : Portal {

    Interdimensional entity;

    void Update () {
        if (entity) {
            entity.SetSliceParams (transform.forward, transform.position, entity.graphicObject);
            entity.mirrorGraphicObject.transform.position = linkedPortal.transform.position + (entity.graphicObject.transform.position - transform.position);
            entity.SetSliceParams (-linkedPortal.transform.forward, linkedPortal.transform.position, entity.mirrorGraphicObject);
        }
    }

    void OnTriggerEnter (Collider c) {
        if (c.gameObject.GetComponent<Interdimensional> ()) {
            entity = c.gameObject.GetComponent<Interdimensional> ();
            entity.mirrorGraphicObject = Instantiate (entity.graphicObject);
        }
    }

    void OnTriggerExit (Collider c) {
        entity = null;
    }
}