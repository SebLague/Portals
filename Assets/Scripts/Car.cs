using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : Interdimensional {

    public GameObject graphic;

    // Start is called before the first frame update
    void Start () {

    }

    // Update is called once per frame
    void Update () {

    }

    public override GameObject GetGraphicObject () {
        return graphic;
    }
}