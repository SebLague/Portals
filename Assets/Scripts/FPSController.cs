using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour {

    public float walkSpeed = 3;
    public float runSpeed = 6;
    public float smoothMoveTime = 0.1f;
    public float jumpForce = 8;
    public float gravity = 18;

    public bool lockCursor;
    public float mouseSensitivity = 10;
    public Vector2 pitchMinMax = new Vector2 (-40, 85);
    public float rotationSmoothTime = 0.1f;

    CharacterController controller;
    Camera cam;
    float yaw;
    float pitch;
    float smoothYaw;
    float smoothPitch;

    float yawSmoothV;
    float pitchSmoothV;
    float verticalVelocity;
    Vector3 velocity;
    Vector3 smoothV;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    void Start () {
        cam = Camera.main;
        if (lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        controller = GetComponent<CharacterController> ();
    }

    void Update () {
        if (Input.GetKeyDown (KeyCode.P)) {
            Debug.Break ();
        }
        Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
        Vector3 inputDir = new Vector3 (input.x, 0, input.y).normalized;
        Vector3 worldInputDir = transform.TransformDirection (inputDir);

        float currentSpeed = (Input.GetKey (KeyCode.LeftShift)) ? runSpeed : walkSpeed;
        Vector3 targetVelocity = worldInputDir * currentSpeed;
        velocity = Vector3.SmoothDamp (velocity, targetVelocity, ref smoothV, smoothMoveTime);

        verticalVelocity -= gravity * Time.deltaTime;
        velocity = new Vector3 (velocity.x, verticalVelocity, velocity.z);

        var flags = controller.Move (velocity * Time.deltaTime);
        if (flags == CollisionFlags.Below) {
            verticalVelocity = 0;
        }

        if (Input.GetKeyDown (KeyCode.Space)) {
            if (controller.isGrounded) {
                verticalVelocity = jumpForce;
            }
        }

        yaw += Input.GetAxisRaw ("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxisRaw ("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp (pitch, pitchMinMax.x, pitchMinMax.y);
        smoothPitch = Mathf.SmoothDamp (smoothPitch, pitch, ref pitchSmoothV, rotationSmoothTime);
        smoothYaw = Mathf.SmoothDamp (smoothYaw, yaw, ref yawSmoothV, rotationSmoothTime);

        transform.eulerAngles = Vector3.up * smoothYaw;
        cam.transform.localEulerAngles = Vector3.right * smoothPitch;

    }

    public void Teleport (Vector3 pos) {
        controller.enabled = false;
        transform.position = pos;
        controller.enabled = true;
    }

}