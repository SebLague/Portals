using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicePortal : Portal {

    void OnTriggerEnter (Collider c) {
        Debug.Log (c.gameObject.name);
    }
}