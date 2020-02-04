using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public bool spawnAtStart;
    public GameObject prefab;

    void Start () {
        if (spawnAtStart) {
            Spawn ();
        }
    }

    void Update () {
        if (Input.GetKeyDown (KeyCode.Space)) {
            Spawn ();
        }
    }

    void Spawn () {
        Instantiate (prefab, transform.position, transform.rotation);
    }
}