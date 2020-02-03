using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudCoreTest : MonoBehaviour {

    public float falloffDstHorizontal = 3;
    public float falloffVertical = 1.5f;
    public float maxScale = 1;

    public Vector2 rotSpeedMinMax = new Vector2 (10, 20);

    float rotSpeed;

    [HideInInspector]
    public Transform myTransform;

    void Start () {
        rotSpeed = Random.Range (rotSpeedMinMax.x, rotSpeedMinMax.y);
        myTransform = transform;
    }

    void Update () {
        myTransform.RotateAround (transform.parent.position, Vector3.up, Time.deltaTime * rotSpeed);
    }
}