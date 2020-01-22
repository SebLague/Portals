using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DemoScreenSpace : MonoBehaviour {

    public TMPro.TextMeshProUGUI[] text;
    public Transform[] corners;

    void Update () {
        var cam = Camera.main;
        for (int i = 0; i < corners.Length; i++) {
            var c = cam.WorldToViewportPoint (corners[i].position);
            text[i].text = string.Format ("({0:0.00}# {1:0.00})", c.x, c.y).Replace(',','.').Replace('#',',');
        }

    }
}