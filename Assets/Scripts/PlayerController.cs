using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera camera;
    private Transform cameraT;
    protected CharacterController controller;
    public float walkSpeed = 2;
    public float runSpeed = 6;
    public bool running = false;


    public float currentSpeed;
    public float velocityY;

    // Start is called before the first frame update
    void Start()
    {

        controller = GetComponent<CharacterController>();
        cameraT = Camera.main.transform;
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // input
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;
        running = Input.GetKey(KeyCode.LeftShift);
        Move(inputDir, running);
    }

    void Move(Vector2 inputDir, bool running)
    {
        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
        }

        float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
        //currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));
        currentSpeed = Mathf.SmoothStep(currentSpeed, targetSpeed, 1f);

        velocityY += Time.deltaTime * wm.gravity;
        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);
        currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

        if (controller.isGrounded)
        {
            velocityY = 0;
        }

    }
}
