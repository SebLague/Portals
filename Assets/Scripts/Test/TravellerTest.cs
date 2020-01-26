using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravellerTest : PortalTraveller {
    public float speed = 5;

    void Update () {
        transform.position += transform.forward * Time.deltaTime * speed;
       
    }
}