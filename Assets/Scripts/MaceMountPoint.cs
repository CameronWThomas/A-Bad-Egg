using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaceMountPoint : MonoBehaviour
{
    HandMountPoint handMountPoint;
    Quaternion armedRotation = new Quaternion(-0.456308216f, 0.169321358f, -0.135407642f, 0.863005161f);
    public bool swinging = false;
    public bool VIOLENT = false;
    public float violentTimer = 0f;
    public float violentTimerMax = 10f;

    float hitCooldownCounter = 0f;
    float hitCooldownTimer = 0.2f;
    bool cooldown = false;

    Vector3 force;
    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {
        if (VIOLENT && violentTimer < violentTimerMax) 
        {
            violentTimer += Time.deltaTime;
        }else if(VIOLENT && violentTimer >= violentTimerMax)
        {
            VIOLENT = false;
            violentTimer = 0f;
        }

        if (cooldown)
        {
            hitCooldownCounter += Time.deltaTime;
            if(hitCooldownCounter > hitCooldownTimer)
            {
                cooldown = false;
                hitCooldownCounter = 0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (swinging && !cooldown)
        {
            //npc case
            if (other.transform.parent != null)
            {
                EggPersonController controller = other.GetComponentInParent<EggPersonController>();
                if (controller == null)
                {
                    controller = other.GetComponent<EggPersonController>();
                }

                if (controller != null && other.transform.root.name != transform.root.name)
                {
                    VIOLENT = true;
                    controller.OnHit(20, transform.position);
                    cooldown = true;
                }
            }
            else //player case
            {
                EggPersonController controller = other.GetComponent<EggPersonController>();

                if (controller != null && other.transform.root.name != transform.root.name)
                {
                    VIOLENT = true;
                    controller.OnHit(20, transform.position);
                    cooldown = true;
                }
            }
        }
    }
}
