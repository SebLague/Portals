using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudBehaviorTest : MonoBehaviour {
    public Vector2 rotSpeedMinMax = new Vector2 (10, 20);

    float rotSpeed;

    CloudCoreTest[] cloudCentres;
    Transform myTransform;

    void Start () {
        rotSpeed = Random.Range (rotSpeedMinMax.x, rotSpeedMinMax.y);
        cloudCentres = FindObjectsOfType<CloudCoreTest> ();
        myTransform = transform;
    }

    void Update () {
        myTransform.RotateAround (transform.parent.position, Vector3.up, Time.deltaTime * rotSpeed);
        float maxScale = 0;
        for (int i = 0; i < cloudCentres.Length; i++) {
            CloudCoreTest cloudCentre = cloudCentres[i];
            Vector3 offset = (myTransform.position - cloudCentre.transform.position);
            float sqrDstHorizontal = offset.x * offset.x + offset.z * offset.z;
            float sqrDstVertical = offset.y * offset.y;
            float tH = 1 - Mathf.Min (1, sqrDstHorizontal / (cloudCentre.falloffDstHorizontal * cloudCentre.falloffDstHorizontal));
            float tV = 1 - Mathf.Min (1, sqrDstVertical / (cloudCentre.falloffVertical * cloudCentre.falloffVertical));
            //float t = 1 - Mathf.Min (1, sqrDst / (falloffDst * falloffDst));
            maxScale = Mathf.Max (maxScale, tV * tH * cloudCentre.maxScale);
        }
        myTransform.localScale = Vector3.one * maxScale;
    }
}