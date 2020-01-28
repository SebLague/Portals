using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheels : MonoBehaviour {
    public float wheelRot;
    public Transform[] wheels;
    public float multiplier = 1;
    public float wheelRadius = 1;

    public void Turn (float moveDst) {
        float circum = 2 * Mathf.PI * wheelRadius;
        float numTurns = moveDst / circum;

        foreach (Transform t in wheels) {
            t.Rotate (Vector3.right * numTurns * 360, Space.Self);
        }
    }
}