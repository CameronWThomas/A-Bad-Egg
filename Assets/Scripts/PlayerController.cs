using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera mainCamera;
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

    public bool armed = false;
    EggAnimator eggAnimator;


    WeildedMace myMace;

    public bool ragdolled = false;
    public List<Rigidbody> ragdollBones;
    public float jumpHeight = 1;

    EggPersonCore epc;
    //Vector3 coreOffset;

    Rigidbody rootRbody;
    SphereCollider rootCollider;

    public GameObject[] immediateChildren;
    
    // Start is called before the first frame update
    void Start()
    {

        controller = GetComponent<CharacterController>();
        cameraT = Camera.main.transform;
        mainCamera = Camera.main;
        eggAnimator = GetComponent<EggAnimator>();

        wm = GameObject.FindObjectOfType<WorldManager>();
        //rb = GetComponent<Rigidbody>();

        myMace = GetComponentInChildren<WeildedMace>();
        myMace.SetVisible(false);


        epc = GetComponentInChildren<EggPersonCore>();
        ragdollBones = GetComponentsInChildren<Rigidbody>().ToList();
        foreach (Rigidbody ragdollBone in ragdollBones)
        {
            ragdollBone.isKinematic = true;
            ragdollBone.useGravity = false;
            //ragdollBone.mass = 0.05f;
            //ragdollBone.AddForce(new Vector3(0, -1.0f, 0) * ragdollBone.mass * (1 * wm.gravity));
        }

        immediateChildren = GetGameObjectsInDirectChildren(gameObject);

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
            //transform.position = epc.transform.position;
            //transform.position = coreOffset + epc.transform.position;
        }
        else
        {

            Move(inputDir, running);


            //armed / unarmed
            if (Input.GetKeyUp(KeyCode.F))
            {
                armed = !armed;
                eggAnimator.SetArmed(armed);
                myMace.SetVisible(armed);
            }
            if(armed && Input.GetMouseButtonDown(0))
            {
                eggAnimator.Swing();
            }

            //jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
                eggAnimator.OnJumping();
            }
        }

        if (Input.GetKeyUp(KeyCode.R))
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

    protected void Jump()
    {
        float effectiveHeight = (jumpHeight / (wm.gravity / -2));
        if (controller.isGrounded)
        {
            float jumpVelocity = Mathf.Sqrt(-2 * wm.gravity * effectiveHeight);
            velocityY = jumpVelocity;
        }
    }
    public void ToggleRagdoll( bool start)
    {
        if (!start)
        {
            ragdolled = false;
            eggAnimator.animator.enabled = true;
            controller.enabled = true;
            //rootRbody.isKinematic = true;
            //rootRbody.useGravity = false;
            foreach (Rigidbody ragdollBone in ragdollBones)
            {
                ragdollBone.isKinematic = true;
                ragdollBone.useGravity = false;
            }


        }
        else
        {
            ragdolled = true;
            eggAnimator.animator.enabled = false;
            controller.enabled = false;
            //rootRbody.isKinematic = false;
            //rootRbody.useGravity = true;
            foreach (Rigidbody ragdollBone in ragdollBones)
            {
                

                ragdollBone.isKinematic = false;
                ragdollBone.useGravity = true;
                //ragdollBone.AddForce(new Vector3(0, 1.0f, 0) * ragdollBone.mass * (1 * wm.gravity));

                
            }

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
        ToggleRagdoll(true);
        SeperateChildren();
        //rb.useGravity = true;
        //rb.isKinematic = false;
        //controller.enabled = !rolling;
        //eggAnimator.animator.enabled = !rolling;
    }
    void StopRolling()
    {
        SetPosToCore();
        ConsumeChildren();
        rolling = false;
        ToggleRagdoll(false);
        //rb.useGravity = false;
        //rb.isKinematic = true;
        //controller.enabled = !rolling;
        //eggAnimator.animator.enabled = !rolling;

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

        float speed = running ? runSpeed : walkSpeed;
        speed *= 50;
        epc.rbody.AddForce(movement * speed);
        //rb.AddForce(movement * speed);

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
    static T[] GetComponentsInDirectChildren<T>(GameObject gameObject)
    {
        int indexer = 0;

        foreach (Transform transform in gameObject.transform)
        {
            if (transform.GetComponent<T>() != null)
            {
                indexer++;
            }
        }

        T[] returnArray = new T[indexer];

        indexer = 0;

        foreach (Transform transform in gameObject.transform)
        {
            if (transform.GetComponent<T>() != null)
            {
                returnArray[indexer++] = transform.GetComponent<T>();
            }
        }

        return returnArray;
    }
    static GameObject[] GetGameObjectsInDirectChildren(GameObject gameObject)
    {
        int indexer = 0;

        foreach (Transform transform in gameObject.transform)
        {
            if (transform.gameObject != null)
            {
                indexer++;
            }
        }

        GameObject[] returnArray = new GameObject[indexer];

        indexer = 0;

        foreach (Transform transform in gameObject.transform)
        {
            if (transform.gameObject != null)
            {
                returnArray[indexer++] = transform.gameObject;
            }
        }

        return returnArray;
    }

    void SetPosToCore()
    {

        Vector3 newPos = epc.transform.position;
        transform.position = new Vector3(newPos.x, newPos.y + 5, newPos.z);
    }
    void SeperateChildren()
    {
        foreach(GameObject child in immediateChildren)
        {
            child.transform.SetParent(null);
            DistanceKeeper distanceKeeper = child.GetComponent<DistanceKeeper>();
            if(distanceKeeper != null) distanceKeeper.active = true;
        }
    }

    void ConsumeChildren()
    {
        foreach (GameObject child in immediateChildren)
        {
            child.transform.SetParent(gameObject.transform);
            DistanceKeeper distanceKeeper = child.GetComponent<DistanceKeeper>();
            if (distanceKeeper != null) distanceKeeper.active = true;

        }
    }
}
