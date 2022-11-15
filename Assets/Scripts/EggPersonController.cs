using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class EggPersonController : MonoBehaviour
{
    public CharacterController controller;
    public float walkSpeed = 5;
    public float runSpeed = 10;
    public bool running = false;


    public float currentSpeed;
    public float velocityY;


    public float turnSmoothTime = 0.2f;
    public float turnSmoothVelocity;

    public float speedSmoothTime = 0.1f;
    public float speedSmoothVelocity;

    public int health = 2;
    public int armor = 0;

    [Range(0, 1)]
    public float airControlPercent;

    public bool rolling = false;
    public WorldManager wm;


    public bool armed = false;
    public EggAnimator eggAnimator;


    public WeildedMace myMace;
    public MaceMountPoint mountPoint;

    public bool ragdolled = false;
    public List<Rigidbody> ragdollBones;
    public float jumpHeight = 5;


    public EggPersonCore epc;


    public GameObject[] immediateChildren;

    public HandMountPoint handMountPoint;

    public float ragdollTimer = 5f;
    public float ragdollCounter = 0f;


    public bool isRagdolled;
    public bool swinging = false;

    public bool invuln = false;
    public float invulnTimer = 3f;
    public float invulnCounter = 0f;

    public float force;
    public Vector3 away;
    public Vector3 impactPoint;
    ShatteredEggPersonController sepc;
    void Start()
    {
        invulnTimer = 3f;
        ragdollTimer = 5f;

        controller = GetComponent<CharacterController>();
        eggAnimator = GetComponent<EggAnimator>();

        wm = GameObject.FindObjectOfType<WorldManager>();
        //rb = GetComponent<Rigidbody>();

        myMace = GetComponentInChildren<WeildedMace>();
        mountPoint = GetComponentInChildren<MaceMountPoint>();

        epc = GetComponentInChildren<EggPersonCore>();
        handMountPoint = GetComponentInChildren<HandMountPoint>();
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
        if(force > 0.5)
        {
            force = Mathf.Lerp(force, 0, Time.deltaTime * 4);

            away = (epc.transform.position - impactPoint).normalized;
            epc.rbody.AddForce(away);
        }
    }

    public bool IsViolent()
    {
        return mountPoint.VIOLENT;
    }
    public void OnHit(float forceSpeed, Vector3 splodePoint)
    {
        if (health == 2)
        {
            if (!isRagdolled)
            {
                isRagdolled = true;
                health--;
                ToggleRagdoll(true);
                if (forceSpeed > 0)
                {
                    impactPoint = splodePoint;
                    away = (epc.transform.position - splodePoint).normalized;
                    force = forceSpeed;
                    epc.rbody.AddForce(away);
                }
            }
            
        }
        else
        {
            if (sepc == null && !invuln)
            {
                SwapToShattered(forceSpeed, splodePoint);
            }
        }
    }
    public void SwapToShattered(float speed, Vector3 splodePoint)
    {
        
        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform child = transform.GetChild(i);
            GameObject.Destroy(child.gameObject);
            // ...
        }

        wm.killedEnemies++;
        NpcController npc = GetComponent<NpcController>();
        controller.enabled = false;
        npc.enabled = false;

        ShatteredEggPersonController[] sepcs = GameObject.FindObjectsOfType<ShatteredEggPersonController>();

        foreach(ShatteredEggPersonController se in sepcs)
        {
            if (!se.enabled)
            {
                sepc = se;
                break;
            }
        }
        if (sepc != null)
        {
            sepc.transform.position = this.transform.position;
            sepc.transform.rotation = this.transform.rotation;

            sepc.ImpactPoint = splodePoint;
            sepc.ImpactSpeed = speed;
            sepc.EnableGravity();
            this.enabled = false;
        }
    }

    public void StopRagdoll()
    {
        isRagdolled = false;
        ToggleRagdoll(false);
    }

    public void Jump()
    {
        float effectiveHeight = (jumpHeight / (wm.gravity / -2));
        if (controller.isGrounded)
        {
            float jumpVelocity = Mathf.Sqrt(-2 * wm.gravity * effectiveHeight);
            velocityY = jumpVelocity;
        }
    }
    public void ToggleRagdoll(bool start)
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

    public void ToggleRoll()
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
    public void StartRolling()
    {
        rolling = true;
        ToggleRagdoll(true);
        SeperateChildren();
        //rb.useGravity = true;
        //rb.isKinematic = false;
        //controller.enabled = !rolling;
        //eggAnimator.animator.enabled = !rolling;
    }
    public void StopRolling()
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

    public float GetModifiedSmoothTime(float smoothTime)
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
        foreach (GameObject child in immediateChildren)
        {
            child.transform.SetParent(null);
            DistanceKeeper distanceKeeper = child.GetComponent<DistanceKeeper>();
            if (distanceKeeper != null) distanceKeeper.active = true;
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
