using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearclipTest : MonoBehaviour {
    // Start is called before the first frame update
    void Start () {

    }

    // Update is called once per frame
    void Update () {

    }

    float CalculateDst () {
        Camera cam = Camera.main;
        float halfHeight = cam.nearClipPlane * Mathf.Tan (cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * cam.aspect;
        float dst = Mathf.Sqrt (halfHeight * halfHeight + halfWidth * halfWidth + cam.nearClipPlane * cam.nearClipPlane);
        return dst;
    }

    void OnDrawGizmos () {
        Camera cam = Camera.main;

        Gizmos.color = Color.green;
        Gizmos.DrawRay (transform.position, transform.forward * cam.nearClipPlane);

        var frustumHeight = 2.0f * cam.nearClipPlane * Mathf.Tan (cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        var frustumWidth = frustumHeight * cam.aspect;
        Gizmos.color = Color.red;
        Gizmos.DrawRay (transform.position + transform.forward * cam.nearClipPlane - transform.up * frustumHeight / 2, transform.up * frustumHeight);

        Vector3 corner = transform.position - transform.right * frustumWidth / 2 + transform.up * frustumHeight / 2 + transform.forward * cam.nearClipPlane;
        Vector3 dir = (corner - transform.position).normalized;
        float dst = CalculateDst ();
        Gizmos.DrawRay (transform.position, dir * dst);
    }
}