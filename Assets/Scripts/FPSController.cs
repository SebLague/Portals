using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour {

    public float speed = 2;
    public float smoothMoveTime;
    public Vector3 velocity { get; private set; }
    Vector3 smoothV;

    public bool lockCursor;
    public float mouseSensitivity = 10;
    public Vector2 pitchMinMax = new Vector2 (-40, 85);

    public float rotationSmoothTime = .12f;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    Camera cam;
    float yaw;
    float pitch;

    void Start () {
        cam = Camera.main;
        if (lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.P)){
            Debug.Break();
        }
        Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
        Vector3 inputDir = new Vector3 (input.x, 0, input.y).normalized;
        Vector3 worldInputDir = transform.TransformDirection (inputDir);

        float currentSpeed = (Input.GetKey (KeyCode.LeftShift)) ? speed * 2 : speed;
        Vector3 targetVelocity = worldInputDir * currentSpeed;
        velocity = Vector3.SmoothDamp (velocity, targetVelocity, ref smoothV, smoothMoveTime);

        transform.position += velocity * Time.deltaTime;

    }

    void LateUpdate () {
        yaw += Input.GetAxisRaw ("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxisRaw ("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp (pitch, pitchMinMax.x, pitchMinMax.y);

        currentRotation = Vector3.SmoothDamp (currentRotation, new Vector3 (pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;
    }
}