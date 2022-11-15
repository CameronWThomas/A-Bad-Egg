using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NpcController : MonoBehaviour
{
    List<PathMarker> travelMarkers;
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

    public int pathFamily = -1;
    public int currentPathOrder = 0;
    public List<PathMarker> pathMarkers;

    public float viewDistance = 30f;

    int frames = 0;
    // Start is called before the first frame update
    void Start()
    {
        viewDistance = 100f;
        travelMarkers = GameObject.FindObjectsOfType<PathMarker>().ToList();
        SetTargetToRandomTravelMarker();
        eggPersonController = GetComponent<EggPersonController>();
        nav = GetComponent<NavMeshAgent>();
        nav.speed = eggPersonController.walkSpeed;
        playerController = GameObject.FindObjectOfType<PlayerController>();

        if(pathFamily >= 0)
        {

            pathMarkers = GameObject.FindObjectsOfType<PathMarker>().ToList().Where(el => el.family == pathFamily).ToList();
        }
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

            //invuln controls
            if (eggPersonController.invuln)
            {
                eggPersonController.invulnCounter += Time.deltaTime;
                if (eggPersonController.invulnCounter > eggPersonController.invulnTimer)
                {
                    eggPersonController.invuln = false;
                    eggPersonController.invulnCounter = 0f;

                }
            }

            //look for player manager
            frames++;
            if(frames % 20 == 0 && !targetingPlayer && playerController.IsViolent())
            {
                frames = 0;
                viewDistance = 20f * eggPersonController.wm.killedEnemies;
                TryToObservePlayer();

            }

            if(!eggPersonController.isRagdolled)
            {
                //movement management
                if (isMoving)
                {
                    float distance = (transform.position - target.position).magnitude;
                    if (distance < targetReachedPos)
                    {
                        if (targetingPlayer)
                        {
                            AttackPlayer();
                        }
                        else
                        {
                            SetTargetToNextPathMarker();
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

    void TryToObservePlayer()
    {
        //raycast to player based on "viewDistance"
        Vector3 targetPos = playerController.transform.position;
        targetPos.y += 2;
        Vector3 startPoint = transform.position;
        startPoint.y += 2;

        Vector3 dir = targetPos - startPoint;
        Ray ray = new Ray(startPoint, dir);

        RaycastHit rayHit;
        Debug.DrawRay(startPoint, dir, Color.red, 1f);

        if (Physics.Raycast(ray, out rayHit, viewDistance))
        {
            if (rayHit.collider.gameObject.tag == "Player")
            {
                targetingPlayer = true;
            }
        }
    }

    void AttackPlayer()
    {

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

    void SetTargetToNextPathMarker()
    {
        if(pathFamily < 0)
        {
            SetTargetToRandomTravelMarker();
        }
        else
        {
            currentPathOrder++;
            PathMarker pm = pathMarkers.Where(el => el.order ==currentPathOrder).FirstOrDefault();
            if(pm != null)
            {
                target = pm.transform;
            }
            else
            {
                currentPathOrder = -1;
                SetTargetToNextPathMarker();
            }
        }
    }
}
