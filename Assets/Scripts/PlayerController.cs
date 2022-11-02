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

    public float turnSmoothTime = 0.2f;
    protected float turnSmoothVelocity;

    public float speedSmoothTime = 0.1f;
    protected float speedSmoothVelocity;

    [Range(0, 1)]
    public float airControlPercent;

    public bool rolling = true;
    protected WorldManager wm;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {

        controller = GetComponent<CharacterController>();
        cameraT = Camera.main.transform;
        camera = Camera.main;

        wm = GameObject.FindObjectOfType<WorldManager>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // input
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;
        running = Input.GetKey(KeyCode.LeftShift);

        if (rolling)
        {
            //RollAround(inputDir, running);
        }
        else
        {

            Move(inputDir, running);
        }

        if (Input.GetKey(KeyCode.R))
        {
            ToggleRoll();
        }

    }

    private void FixedUpdate()
    {
        if (rolling)
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector2 inputDir = input.normalized;
            running = Input.GetKey(KeyCode.LeftShift);

            Roll(inputDir, running);
        }
    }
    

    void ToggleRoll()
    {
        if (rolling)
        {
            StopRolling();
        }
        else
        {
            StartRolling();
        }
    }
    void StartRolling()
    {
        rolling = true;
        rb.useGravity = true;
        rb.isKinematic = false;
    }
    void StopRolling()
    {
        rolling = false;
        rb.useGravity = false;
        rb.isKinematic = true;

    }

    void Roll(Vector2 inputDir, bool running)
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        var camForward = camera.transform.forward;
        var camRight = camera.transform.right;
        camForward.Normalize();
        camRight.Normalize();

        movement = camForward * inputDir.y + camRight * inputDir.x;

        float speed = running ? runSpeed : walkSpeed;
        speed *= 2;
        rb.AddForce(movement * speed);

        if (Input.GetKeyDown("space"))
        {
            // Vector3 jump = new Vector3(0.0f, jumpHeight, 0.0f);
            //rb.AddForce(jump);
        }
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

    protected float GetModifiedSmoothTime(float smoothTime)
    {
        if (controller.isGrounded)
        {
            return smoothTime;
        }

        if (airControlPercent == 0)
        {
            return float.MaxValue;
        }
        return smoothTime / airControlPercent;
    }
}
