using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    void OnPreCull () {
        var portals = FindObjectsOfType<Portal> ();
        foreach (var p in portals) {
            p.Render ();
        }
    }

}