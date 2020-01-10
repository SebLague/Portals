using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudTest : MonoBehaviour {
    public int numViewDirections = 100;
    public float scale = 1;
    public float spawnRadius = 10;
    [Range (0, 1)]
    public float startHeight;
    public GameObject cloudPrefab;

    void Start () {

        float goldenRatio = (1 + Mathf.Sqrt (5)) / 2;
        float angleIncrement = Mathf.PI * 2 * goldenRatio;

        for (int i = 0; i < numViewDirections; i++) {
            float t = (float) i / numViewDirections;
            float inclination = Mathf.Acos (1 - (1 - startHeight) * t);
            float azimuth = angleIncrement * i;

            float x = Mathf.Sin (inclination) * Mathf.Sin (azimuth);
            float y = Mathf.Cos (inclination);
            float z = Mathf.Sin (inclination) * Mathf.Cos (azimuth);

            var g = Instantiate (cloudPrefab, new Vector3 (x, y, z) * spawnRadius, Quaternion.identity);
            g.transform.localScale = Vector3.one * scale;
        }
    }
}