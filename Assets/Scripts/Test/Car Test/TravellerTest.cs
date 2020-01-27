using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravellerTest : PortalTraveller {
    public float speed = 5;

    void Update () {
        transform.position += transform.forward * Time.deltaTime * speed;
        if (Input.GetKeyDown (KeyCode.C)) {
            speed = (speed == 0) ? 1 : 0;
        }
    }
}