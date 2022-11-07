using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera mainCamera;
    private Transform cameraT;
    public EggPersonController eggPersonController;
        
    // Start is called before the first frame update
    void Start()
    {
        eggPersonController = GetComponent<EggPersonController>();
        cameraT = Camera.main.transform;
        mainCamera = Camera.main;

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

                    Move(inputDir, eggPersonController.running);


                    //armed / unarmed
                    if (Input.GetKeyUp(KeyCode.F))
                    {
                        eggPersonController.armed = !eggPersonController.armed;
                        eggPersonController.eggAnimator.SetArmed(eggPersonController.armed);
                        eggPersonController.myMace.SetVisible(eggPersonController.armed);
                    }
                    if (eggPersonController.armed && Input.GetMouseButtonDown(0))
                    {
                        eggPersonController.eggAnimator.Swing();
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
