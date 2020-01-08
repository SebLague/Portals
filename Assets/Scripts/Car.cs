using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : Interdimensional {

    public bool autoMove;
    public float speed = 1;

    void Update () {
        if (autoMove) {
            transform.position += transform.forward * Time.deltaTime * speed;
        }
    }

}