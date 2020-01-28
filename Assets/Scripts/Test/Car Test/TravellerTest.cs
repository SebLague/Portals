using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravellerTest : PortalTraveller {
    public float speed = 5;
    float targetSpeed;
    float v;

    void Update () {
        float moveDst = Time.deltaTime * speed;
        transform.position += transform.forward * Time.deltaTime * speed;
        if (Input.GetKeyDown (KeyCode.C)) {
            targetSpeed = (targetSpeed == 0) ? 1 : 0;
        }
        speed = Mathf.SmoothDamp (speed, targetSpeed, ref v, .5f);
        var w = FindObjectsOfType<Wheels> ();
        foreach (var w0 in w) {
            w0.Turn (moveDst);
        }
    }
}