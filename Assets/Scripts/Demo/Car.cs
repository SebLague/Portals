using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : PortalTraveller {
    public float maxSpeed = 1;
    float speed;
    float targetSpeed;
    float smoothV;

    void Start () {
        Debug.Log ("Press C to stop/start car");
        targetSpeed = maxSpeed;
    }

    void Update () {
        float moveDst = Time.deltaTime * speed;
        transform.position += transform.forward * Time.deltaTime * speed;

        if (Input.GetKeyDown (KeyCode.C)) {
            targetSpeed = (targetSpeed == 0) ? maxSpeed : 0;
        }
        speed = Mathf.SmoothDamp (speed, targetSpeed, ref smoothV, .5f);
    }
}