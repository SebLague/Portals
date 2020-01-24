using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DemoScreenSpace : MonoBehaviour {

    public TMPro.TextMeshProUGUI[] text;
    public Transform[] corners;
    public MeshRenderer screen;
    bool active;

    void Update () {
        if (Input.GetKeyDown (KeyCode.T)) {
            screen.material.SetInt ("_S", 1);
            active = true;
        }

        if (!active) {
            return;
        }

        var cam = Camera.main;
        for (int i = 0; i < corners.Length; i++) {
            var c = cam.WorldToViewportPoint (corners[i].position);
            text[i].text = string.Format ("({0:0.00}# {1:0.00})", c.x, c.y).Replace (',', '.').Replace ('#', ',');
        }

    }
}