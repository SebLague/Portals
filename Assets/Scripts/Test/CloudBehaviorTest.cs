using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudBehaviorTest : MonoBehaviour {
    public Vector2 rotSpeedMinMax = new Vector2 (10, 20);
    public float falloffDst = 3;
    public float maxScale = 1;
    float rotSpeed;

    Transform cloudCentre;

    void Start () {
        rotSpeed = Random.Range (rotSpeedMinMax.x, rotSpeedMinMax.y);
        cloudCentre = GameObject.FindGameObjectWithTag ("Player").transform;
    }

    void Update () {
        transform.RotateAround (Vector3.zero, Vector3.up, Time.deltaTime * rotSpeed);
        float sqrDst = (transform.position - cloudCentre.position).sqrMagnitude;
        float t = 1 - Mathf.Min (1, sqrDst / (falloffDst * falloffDst));
        transform.localScale = Vector3.one * t;
    }
}