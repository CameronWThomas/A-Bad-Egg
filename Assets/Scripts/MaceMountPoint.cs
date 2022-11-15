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
    }

    private void OnTriggerEnter(Collider other)
    {

        if (swinging)
        {
            EggPersonController controller = other.GetComponent<EggPersonController>();

            if (controller != null && other.transform.root.name != transform.root.name)
            {
                VIOLENT = true;
                controller.OnHit(20, transform.position);
            }
        }
    }
}
