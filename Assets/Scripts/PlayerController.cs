using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera mainCamera;
    private Transform cameraT;
    private CameraController camController;
    public EggPersonController eggPersonController;

    WorldManager worldManager;

    //combat swing business
    public float yaw;
    public float pitch;
    public Vector2 pitchMinMax = new Vector2(-40, 85);
    public Vector2 yawMinMax = new Vector2(-40, 85);
    public float rotationSmoothTime = .12f;
    Vector3 rotationSmoothVelocity;
    public Vector3 currentRotation;
    // Start is called before the first frame update
    void Start()
    {
        eggPersonController = GetComponent<EggPersonController>();
        cameraT = Camera.main.transform;
        mainCamera = Camera.main;
        camController = mainCamera.GetComponent<CameraController>();
        worldManager = GameObject.FindObjectOfType<WorldManager>();

    }

    // Update is called once per frame
    void Update()
    {
        if (eggPersonController.health > 0)
        {

            if (eggPersonController.isRagdolled)
            {
                eggPersonController.ragdollCounter += Time.deltaTime;
                if (eggPersonController.ragdollCounter > eggPersonController.ragdollTimer)
                {
                    eggPersonController.StopRagdoll();
                }
            }
            else
            {

                // input
                Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                Vector2 inputDir = input.normalized;
                eggPersonController.running = Input.GetKey(KeyCode.LeftShift);

                if (eggPersonController.rolling)
                {
                    //RollAround(inputDir, running);
                    //transform.position = epc.transform.position;
                    //transform.position = coreOffset + epc.transform.position;
                }
                else
                {


                    if (eggPersonController.swinging)
                    {
                        //SwingMove(inputDir);
                    }
                    else
                    {
                        Move(inputDir, eggPersonController.running);
                    }


                    //armed / unarmed
                    if (Input.GetKeyUp(KeyCode.F))
                    {
                        eggPersonController.armed = !eggPersonController.armed;
                        eggPersonController.eggAnimator.SetArmed(eggPersonController.armed);
                        eggPersonController.myMace.SetVisible(eggPersonController.armed);
                        eggPersonController.mountPoint.armed = eggPersonController.armed;
                    }
                    if (eggPersonController.armed && Input.GetMouseButtonDown(0))
                    {
                        if (!eggPersonController.swinging)
                        {
                            SetDefaultYawAndPitch();
                            eggPersonController.swinging = true;
                            camController.SetCombatMode(eggPersonController.swinging);
                            eggPersonController.eggAnimator.SetSwinging(true);
                        }
                    }
                    if (eggPersonController.armed && Input.GetMouseButtonUp(0))
                    {
                        if (eggPersonController.swinging)
                        {
                            
                            eggPersonController.swinging = false;
                            camController.SetCombatMode(eggPersonController.swinging);
                            eggPersonController.eggAnimator.SetSwinging(false);
                            
                        }
                    }

                    //jump
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        eggPersonController.Jump();
                        eggPersonController.eggAnimator.OnJumping();
                    }
                }

                if (Input.GetKeyUp(KeyCode.R))
                {
                    eggPersonController.ToggleRoll();
                }
            }
        }
        
    }

    private void LateUpdate()
    {

        if (eggPersonController.health > 0)
        {
            if (eggPersonController.swinging)
            {
                Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                Vector2 inputDir = input.normalized;
                SwingMove(inputDir);
            }
        }

    }
    void SetDefaultYawAndPitch()
    {
        yaw = transform.rotation.eulerAngles.y;
        pitch = -2.5f;
    }
    private void SwingMove(Vector2 inputDir)
    {
        /*
        Vector3 targetForward = cameraT.forward;
        targetForward.y = transform.forward.y;
        transform.forward = targetForward;
        */
        float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
        transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref eggPersonController.turnSmoothVelocity, eggPersonController.GetModifiedSmoothTime(eggPersonController.turnSmoothTime));
        //yaw += targetRotation;

        yaw += Input.GetAxis("Mouse X") * worldManager.mouseSensitivity;
        //yaw = Mathf.Clamp(pitch, yawMinMax.x, yawMinMax.y);
        pitch -= Input.GetAxis("Mouse Y") * worldManager.mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        //TODO: impose min max clamp on yaw.

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);

        eggPersonController.epc.transform.eulerAngles = currentRotation;
        
        //eggPersonController.epc.transform.localEulerAngles = currentRotation;
        //this will move the whole cam... could work though
        //transform.eulerAngles = currentRotation;

        float targetSpeed = (eggPersonController.walkSpeed) * inputDir.magnitude;
        eggPersonController.currentSpeed = Mathf.SmoothStep(eggPersonController.currentSpeed, targetSpeed, 1f);
        eggPersonController.velocityY += Time.deltaTime * eggPersonController.wm.gravity;
        Vector3 velocity = transform.forward * eggPersonController.currentSpeed + Vector3.up * eggPersonController.velocityY;

        eggPersonController.controller.Move(velocity * Time.deltaTime);

        
        /*
        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref eggPersonController.turnSmoothVelocity, eggPersonController.GetModifiedSmoothTime(eggPersonController.turnSmoothTime));
        }

        float targetSpeed = ((running) ? eggPersonController.runSpeed : eggPersonController.walkSpeed) * inputDir.magnitude;
        //currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));
        eggPersonController.currentSpeed = Mathf.SmoothStep(eggPersonController.currentSpeed, targetSpeed, 1f);

        eggPersonController.velocityY += Time.deltaTime * eggPersonController.wm.gravity;
        Vector3 velocity = transform.forward * eggPersonController.currentSpeed + Vector3.up * eggPersonController.velocityY;

        eggPersonController.controller.Move(velocity * Time.deltaTime);
        eggPersonController.currentSpeed = new Vector2(eggPersonController.controller.velocity.x, eggPersonController.controller.velocity.z).magnitude;

        if (eggPersonController.controller.isGrounded)
        {
            eggPersonController.velocityY = 0;
        }
        */
    }

    private void FixedUpdate()
    {
        if (eggPersonController.rolling)
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector2 inputDir = input.normalized;
            eggPersonController.running = Input.GetKey(KeyCode.LeftShift);

            Roll(inputDir, eggPersonController.running);
        }
    }

    void Move(Vector2 inputDir, bool running)
    {
        currentRotation = Vector3.SmoothDamp(currentRotation, Vector3.zero, ref rotationSmoothVelocity, rotationSmoothTime);
        eggPersonController.epc.transform.eulerAngles = currentRotation;

        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref eggPersonController.turnSmoothVelocity, eggPersonController.GetModifiedSmoothTime(eggPersonController.turnSmoothTime));
        }

        float targetSpeed = ((running) ? eggPersonController.runSpeed : eggPersonController.walkSpeed) * inputDir.magnitude;
        //currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));
        eggPersonController.currentSpeed = Mathf.SmoothStep(eggPersonController.currentSpeed, targetSpeed, 1f);

        eggPersonController.velocityY += Time.deltaTime * eggPersonController.wm.gravity;
        Vector3 velocity = transform.forward * eggPersonController.currentSpeed + Vector3.up * eggPersonController.velocityY;

        eggPersonController.controller.Move(velocity * Time.deltaTime);
        eggPersonController.currentSpeed = new Vector2(eggPersonController.controller.velocity.x, eggPersonController.controller.velocity.z).magnitude;

        if (eggPersonController.controller.isGrounded)
        {
            eggPersonController.velocityY = 0;
        }

    }

    void Roll(Vector2 inputDir, bool running)
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        var camForward = mainCamera.transform.forward;
        var camRight = mainCamera.transform.right;
        camForward.Normalize();
        camRight.Normalize();

        movement = camForward * inputDir.y + camRight * inputDir.x;

        float speed = running ? eggPersonController.runSpeed : eggPersonController.walkSpeed;
        speed *= 50;
        eggPersonController.epc.rbody.AddForce(movement * speed);
        //rb.AddForce(movement * speed);

        if (Input.GetKeyDown("space"))
        {
            // Vector3 jump = new Vector3(0.0f, jumpHeight, 0.0f);
            //rb.AddForce(jump);
        }
    }


}
