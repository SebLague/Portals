using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    List<Portal> portals;

    void Awake () {
        portals = new List<Portal> (FindObjectsOfType<Portal> ());
    }

    int SortPortals (Portal a, Portal b) {
        float sqrDstToViewA = (a.linkedPortal.transform.position - transform.position).sqrMagnitude;
        float sqrDstToViewB = (b.linkedPortal.transform.position - transform.position).sqrMagnitude;
        return sqrDstToViewB.CompareTo (sqrDstToViewA);
    }

    void OnPreCull () {

        // Order by distance (furthest to nearest)
        //portals.Sort ((a, b) => SortPortals (a, b));

        // Render events
        for (int i = 0; i < portals.Count; i++) {
            portals[i].PrePortalRender ();
        }
        for (int i = 0; i < portals.Count; i++) {
            portals[i].Render ();
        }

        for (int i = 0; i < portals.Count; i++) {
            portals[i].PostPortalRender ();
        }
    }

}