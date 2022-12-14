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

    float targetReachedPos = 8f;
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

    CombatController combatController;

    int frames = 0;


    Vector2 swingDistanceRange = new Vector2(7f, 12f);
    float swingDistance = 10f;
    float primeDistance = 50f;

    int startingHealth = -1;
    Vector2 SwingPrimeTimeRange = new Vector2(1f, 5f);
    public float SwingPrimeTime = 3f;
    public float SwingPrimeCounter = 0f;

    public float MustReleaseTime = 15f;
    public float MustReleaseCounter = 0f;
    // Start is called before the first frame update
    void Start()
    {
        viewDistance = 100f;
        travelMarkers = GameObject.FindObjectsOfType<PathMarker>().ToList();
        SetTargetToRandomTravelMarker();
        eggPersonController = GetComponent<EggPersonController>();
        //move a bit slower.
        eggPersonController.runSpeed = 8f;
        eggPersonController.walkSpeed = 4f;
        eggPersonController.swingCooldownTimer *= 3;

        nav = GetComponent<NavMeshAgent>();
        nav.speed = eggPersonController.walkSpeed;
        playerController = GameObject.FindObjectOfType<PlayerController>();

        combatController = GetComponent<CombatController>();
        if (pathFamily >= 0)
        {

            pathMarkers = GameObject.FindObjectsOfType<PathMarker>().ToList().Where(el => el.family == pathFamily).ToList();
        }

        startingHealth = eggPersonController.health;

        SetNewSwingDistance();
        SetNewPrimeTime();
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (!eggPersonController.fallingToDeath)
        {
            if (eggPersonController.health < startingHealth)
            {
                targetingPlayer = true;
                target = GameObject.FindObjectOfType<PlayerController>().transform;
            }
            //TODO: remove this "if" after my test
            if (target.name != playerController.name && targetingPlayer)
            {
                TargetPlayer();
            }

            if (!hasFixedCharController)
            {

                eggPersonController.controller.enabled = false;
                hasFixedCharController = true;
            }

            //ragdoll controls
            if (eggPersonController.ragdolled)
            {
                //just float on
                if (!nav.isStopped)
                {
                    nav.isStopped = true;
                }

                eggPersonController.ragdollCounter += Time.deltaTime;
                if (eggPersonController.ragdollCounter > eggPersonController.ragdollTimer)
                {
                    eggPersonController.StopRollingNPC();
                }
            } //healthy controls
            else if (eggPersonController.health > 0)
            {
                if (eggPersonController.health == 1)
                {

                }
                if (nav.isStopped)
                {
                    nav.isStopped = false;
                }
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


                //invuln controls
                /*
                if (eggPersonController.invuln)
                {
                    eggPersonController.invulnCounter += Time.deltaTime;
                    if (eggPersonController.invulnCounter > eggPersonController.invulnTimer)
                    {
                        eggPersonController.invuln = false;
                        eggPersonController.invulnCounter = 0f;

                    }
                }
                */
                if (targetingPlayer)
                {
                    SeekAndDestroy();
                }
                else
                {
                    StandardNavigation();
                }

            }
            else
            {
                //death handling 
                //eggPersonController.ToggleRagdoll(true);
                this.enabled = false;
            }
        }


    }
    void SetNewSwingDistance()
    {
        swingDistance = Random.Range(swingDistanceRange.x, swingDistanceRange.y);
    }
    void SetNewPrimeTime()
    {
        SwingPrimeTime = Random.Range(SwingPrimeTimeRange.x, SwingPrimeTimeRange.y);
    }
    void SeekAndDestroy()
    {
        //determine if we sould stop attacking
        //look for player manager
        frames++;
        if (frames % 5 == 0)
        {
            frames = 0;
            viewDistance = 20f * eggPersonController.wm.killedEnemies;
            IsPlayerDownedOrDead();

        }

        if (isMoving)
        {

            float distance = (transform.position - target.position).magnitude;
            if(distance < primeDistance)
            {
                SwingPrimeCounter += Time.deltaTime;
                if (!eggPersonController.swinging)
                {
                    combatController.PrimeSwing();
                    eggPersonController.swinging = true;
                    eggPersonController.mountPoint.swinging = true;
                    SwingPrimeCounter = 0;
                    //camController.SetCombatMode(eggPersonController.swinging);
                    eggPersonController.eggAnimator.SetSwinging(true);
                }
                else
                {
                    MustReleaseCounter += Time.deltaTime;
                }

                if (distance < swingDistance)
                {
                    if (eggPersonController.swinging && SwingPrimeCounter > SwingPrimeTime && !eggPersonController.swingCooldown)
                    {
                        SwingPrimeCounter = 0;
                        combatController.ReleaseSwing();
                        eggPersonController.swinging = false;

                        SetNewSwingDistance();
                        SetNewPrimeTime();
                    }
                }else if(MustReleaseCounter >= MustReleaseTime)
                {
                    if (eggPersonController.swinging && !eggPersonController.swingCooldown)
                    {
                        SwingPrimeCounter = 0;
                        combatController.ReleaseSwing();
                        eggPersonController.swinging = false;

                        SetNewSwingDistance();
                        SetNewPrimeTime();
                        MustReleaseCounter = 0;
                    }
                }

            }
        }
        PathfindToTarget();
    }

    

    void StandardNavigation()
    {
        //look for player manager
        frames++;
        if (frames % 20 == 0 && !targetingPlayer && playerController.IsViolent())
        {
            frames = 0;
            viewDistance = 20f * eggPersonController.wm.killedEnemies;
            TryToObservePlayer();

        }

        //movement management
        if (isMoving)
        {
            float distance = (transform.position - target.position).magnitude;
            if (distance < targetReachedPos)
            {
                SetTargetToNextPathMarker();

            }
            PathfindToTarget();
        }
    }

    void AttackPlayer()
    {
            

        if (!combatController.swingReleased)
        {
            if (eggPersonController.swinging)
            {
                SwingPrimeCounter++;

                if(SwingPrimeCounter > SwingPrimeTime)
                {
                    combatController.ReleaseSwing();

                }
            }
            else
            {

                combatController.PrimeSwing();
                eggPersonController.swinging = true;
                eggPersonController.mountPoint.swinging = true;
                //camController.SetCombatMode(eggPersonController.swinging);
                eggPersonController.eggAnimator.SetSwinging(true);
            }
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

    void IsPlayerDownedOrDead()
    {
        EggPersonController player = target.GetComponent<EggPersonController>();
        if(player == null)
        {
            targetingPlayer = false;
            SetTargetToNextPathMarker();
        }
        else if(player.health == 0 || player.IsRagdolling())
        {
            targetingPlayer = false;
            SetTargetToRandomTravelMarker();
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


    void PathfindToTarget()
    {
        nav.SetDestination(target.position);
        combatController.RotateToTarget();
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
