using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    List<GameObject> travelMarkers;
    public EggPersonController eggPersonController;

    public Transform target;

    //TODO: remove.
    public Vector3 targetDir;
    public Vector3 lookingAt;

    float targetReachedPos = 5;
    bool running = false;
    // Start is called before the first frame update
    void Start()
    {
        travelMarkers = GameObject.FindGameObjectsWithTag("pathMarkers").ToList();
        SetTargetToRandomTravelMarker();
        eggPersonController = GetComponent<EggPersonController>();
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
                float distance = (transform.position - target.position).magnitude;
                if (distance < targetReachedPos)
                {
                    SetTargetToRandomTravelMarker();
                }
                MoveToTarget();
            }
        }
        else
        {
            //eggPersonController.ToggleRagdoll(true);
            this.enabled = false;
        }


    }


    //Im dumb. Shoulda just just the pathfinding agent that unity provides.
    void MoveToTarget()
    {
        targetDir = target.position - transform.position;

        /*
        lookingAt = Quaternion.LookRotation(targetDir).eulerAngles;
        //rotation
        float targetRotation = lookingAt.y;
        //float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
        //float targetRotation = Mathf.Rad2Deg * targetDir.y;
        transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref eggPersonController.turnSmoothVelocity, eggPersonController.GetModifiedSmoothTime(eggPersonController.turnSmoothTime));
        */
        transform.LookAt(targetDir);
        //movement
        float targetSpeed = ((running) ? eggPersonController.runSpeed : eggPersonController.walkSpeed) * 1;
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

    void SetTargetToRandomTravelMarker()
    {
        int index = Random.Range(0, travelMarkers.Count);
        if(travelMarkers[index].transform != target)
        {
            target = travelMarkers[index].transform;
        }
        else
        {
            SetTargetToRandomTravelMarker();
        }
    }
}
