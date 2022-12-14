using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    public GameObject combatTarget;

    private Vector3 offset;

    //cust
    public bool lockCursor;
    public bool softLocked;

    public Vector2 pitchMinMax = new Vector2(-40, 85);

    public float rotationSmoothTime = .12f;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    float yaw;
    float pitch;

    public bool locked;

    bool combatMode = false;

    public float dstFromTarget = 2;
    public Vector2 dstMinMax = new Vector2(1, 10);
    public float scrollScale = 0.3f;

    PlayerController playerController;

    WorldManager worldManager;
    // Start is called before the first frame update
    void Start()
    {
        //offset = transform.position - target.transform.position;
        playerController = GameObject.FindObjectOfType<PlayerController>();
        worldManager = GameObject.FindObjectOfType<WorldManager>();

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        /*
        if (Input.GetKeyUp(KeyCode.L))
        {
            locked = !locked;
        }
        */
    }
    public void SetCombatMode(bool engaged)
    {
        combatMode = engaged;
    }

    public void LockCursor() 
    {
        lockCursor = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        lockCursor = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    // Update is called once per frame
    void LateUpdate()
    {

        if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            /*
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                lockCursor = !lockCursor;

                Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = lockCursor;
            }
            */
            if (!locked && !softLocked)
            {


                //transform.position = player.transform.position + offset;
                /*
                if (combatMode)
                {
                    //transform.position = (combatTarget.transform.position) - transform.forward * dstFromTarget;
                    currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
                    transform.position = (target.transform.position) - transform.forward * dstFromTarget;
                    //transform.forward = playerController.transform.forward;
                }
                else
                {
                */
                yaw += Input.GetAxis("Mouse X") * worldManager.mouseSensitivity;
                pitch -= Input.GetAxis("Mouse Y") * worldManager.mouseSensitivity;
                pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

                currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
                transform.eulerAngles = currentRotation;

                transform.position = (target.transform.position) - transform.forward * dstFromTarget;
                //transform.position = target.transform.position;
                //}


                //camera distance
                dstFromTarget -= Input.mouseScrollDelta.y * scrollScale;
                dstFromTarget = Mathf.Clamp(dstFromTarget, dstMinMax.x, dstMinMax.y);
            }
            else if (softLocked)
            {
                yaw += Input.GetAxis("Mouse X") * worldManager.mouseSensitivity;
                pitch -= Input.GetAxis("Mouse Y") * worldManager.mouseSensitivity;
                pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
                currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
                transform.eulerAngles = currentRotation;
            }
        }

    }
}
