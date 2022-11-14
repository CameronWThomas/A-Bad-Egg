using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

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
    NavMeshAgent nav;

    private bool hasFixedCharController = false;
    bool isRunning;

    public bool isMoving = true;
    public bool targetingPlayer = false;

    PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        travelMarkers = GameObject.FindGameObjectsWithTag("pathMarkers").ToList();
        SetTargetToRandomTravelMarker();
        eggPersonController = GetComponent<EggPersonController>();
        nav = GetComponent<NavMeshAgent>();
        nav.speed = eggPersonController.walkSpeed;
        playerController = GameObject.FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: remove this "if" after my test
        if(target.name != playerController.name && targetingPlayer)
        {
            TargetPlayer();
        }

        if (!hasFixedCharController)
        {

            eggPersonController.controller.enabled = false;
            hasFixedCharController = true;
        }
        

        if (eggPersonController.health > 0)
        {

            //speed management
            UpdateSpeed();
            if (isRunning && nav.speed != eggPersonController.runSpeed)
            {
                nav.speed = eggPersonController.runSpeed;
            }
            else if (!isRunning && nav.speed != eggPersonController.walkSpeed)
            {
                nav.speed = eggPersonController.walkSpeed;
            }

            //ragdoll controls
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
                //movement management
                if (isMoving)
                {
                    float distance = (transform.position - target.position).magnitude;
                    if (distance < targetReachedPos)
                    {
                        if (targetingPlayer)
                        {
                            //attack!
                        }
                        else
                        {
                            SetTargetToRandomTravelMarker();
                        }
                    }
                    PathfindToTarget();
                }
            }
        }
        else
        {
            //death handling 

            //eggPersonController.ToggleRagdoll(true);
            this.enabled = false;
        }


    }

    
    void TargetPlayer()
    {
        target = playerController.transform;
        isRunning = true;
    }
    void UpdateSpeed()
    {
        eggPersonController.currentSpeed = nav.speed;
    }

    //Im dumb. Shoulda just just the pathfinding agent that unity provides.
    void MoveToTarget()
    {
        targetDir = target.position - transform.position;

        
        lookingAt = Quaternion.LookRotation(targetDir).eulerAngles;
        //rotation
        float targetRotation = lookingAt.y;
        //float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
        //float targetRotation = Mathf.Rad2Deg * targetDir.y;
        transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref eggPersonController.turnSmoothVelocity, eggPersonController.GetModifiedSmoothTime(eggPersonController.turnSmoothTime));
        
        //transform.LookAt(targetDir);
        //movement
        
        float targetSpeed = ((running) ? eggPersonController.runSpeed : eggPersonController.walkSpeed) * 1;
        eggPersonController.currentSpeed = Mathf.SmoothStep(eggPersonController.currentSpeed, targetSpeed, 1f);
        
        eggPersonController.velocityY += Time.deltaTime * eggPersonController.wm.gravity;
        Vector3 velocity = transform.forward * eggPersonController.walkSpeed + Vector3.up * eggPersonController.velocityY;

        eggPersonController.controller.Move(velocity * Time.deltaTime);
        eggPersonController.currentSpeed = new Vector2(eggPersonController.controller.velocity.x, eggPersonController.controller.velocity.z).magnitude;
        
        if (eggPersonController.controller.isGrounded)
        {
            eggPersonController.velocityY = 0;
        }

    }

    void PathfindToTarget()
    {
        nav.SetDestination(target.position);
    }

    void SetTargetToRandomTravelMarker()
    {
        isRunning = false;
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
