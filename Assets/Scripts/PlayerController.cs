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

    CombatController combatController;

    // Start is called before the first frame update
    void Start()
    {
        eggPersonController = GetComponent<EggPersonController>();
        combatController = GetComponent<CombatController>();
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


                    if(eggPersonController.swinging || combatController.swingReleased)
                    {

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
                    }
                    
                    if (eggPersonController.armed && Input.GetMouseButtonDown(0) && !combatController.swingReleased)
                    {
                        if (!eggPersonController.swinging)
                        {
                            combatController.PrimeSwing();
                            eggPersonController.swinging = true;
                            eggPersonController.mountPoint.swinging = true;
                            //camController.SetCombatMode(eggPersonController.swinging);
                            eggPersonController.eggAnimator.SetSwinging(true);
                        }
                    }
                    if (eggPersonController.armed && Input.GetMouseButtonUp(0))
                    {
                        if (eggPersonController.swinging)
                        {
                            combatController.ReleaseSwing();
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

    public bool IsViolent()
    {
        return eggPersonController.IsViolent();
    }
    private void LateUpdate()
    {

        if (eggPersonController.health > 0)
        {
            
            if ( eggPersonController.swinging || combatController.swingReleased)
            {
                Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                Vector2 inputDir = input.normalized;
                SwingMove(inputDir, eggPersonController.running);

                //move yaw to targe yaw
                //RotateToTarget();
            }
        }

    }



    private void RotateToTarget(float inputRotation)
    {
        if (combatController.swingReleased)
        {
            combatController.DecreaseYaw();
        }
        else
        {
            //yaw = initialYaw + eggPersonController.epc.transform.eulerAngles.y;
            //targetYaw += initialYaw - 180 + eggPersonController.epc.transform.eulerAngles.y;
        }

        var yEuler  = (transform.eulerAngles.y > 180) ? transform.eulerAngles.y - 360 : transform.eulerAngles.y;

        combatController.currentRotation = Vector3.SmoothDamp(combatController.currentRotation, new Vector3(combatController.pitch, yEuler + combatController.yaw), ref combatController.rotationSmoothVelocity, combatController.rotationSmoothTime);
        eggPersonController.epc.transform.eulerAngles = combatController.currentRotation;

        if (combatController.yaw <= combatController.targetYaw)
        {
            combatController.swingReleased = false;
            combatController.StopSwinging();
        }
    }



    private void SwingMove(Vector2 inputDir, bool running)
    {
        float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
        transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref eggPersonController.turnSmoothVelocity, eggPersonController.GetModifiedSmoothTime(eggPersonController.turnSmoothTime));
        

        RotateToTarget(targetRotation);

        float targetSpeed = ((running) ? eggPersonController.runSpeed : eggPersonController.walkSpeed) * inputDir.magnitude;

        eggPersonController.currentSpeed = Mathf.SmoothStep(eggPersonController.currentSpeed, targetSpeed, 1f);
        eggPersonController.velocityY += Time.deltaTime * eggPersonController.wm.gravity;
        Vector3 velocity = transform.forward * eggPersonController.currentSpeed + Vector3.up * eggPersonController.velocityY;

        eggPersonController.controller.Move(velocity * Time.deltaTime);

        
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
