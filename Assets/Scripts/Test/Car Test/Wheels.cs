using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheels : MonoBehaviour {
    public float wheelRot;
    public Transform[] wheels;
    public float multiplier = 1;

    // Update is called once per frame
    void Update () {
        foreach (Transform t in wheels) {
            //  t.Rotate (t.right * Time.deltaTime * wheelRot, Space.World);
            t.Rotate (Vector3.right * Time.deltaTime * wheelRot * multiplier, Space.Self);

        }
    }
}