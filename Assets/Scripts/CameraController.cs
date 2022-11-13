using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    public GameObject combatTarget;

    private Vector3 offset;

    //cust
    public bool lockCursor;
    public float mouseSensitivity = 10;

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

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - target.transform.position;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.L))
        {
            locked = !locked;
        }
    }
    public void SetCombatMode(bool engaged)
    {
        combatMode = engaged;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (!locked)
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
            transform.eulerAngles = currentRotation;

            //transform.position = player.transform.position + offset;

            if (combatMode)
            {
                transform.position = (combatTarget.transform.position) - transform.forward * dstFromTarget;
            }
            else
            {
                transform.position = (target.transform.position) - transform.forward * dstFromTarget;
            }


            //camera distance
            dstFromTarget -= Input.mouseScrollDelta.y * scrollScale;
            dstFromTarget = Mathf.Clamp(dstFromTarget, dstMinMax.x, dstMinMax.y);
        }

    }
}
