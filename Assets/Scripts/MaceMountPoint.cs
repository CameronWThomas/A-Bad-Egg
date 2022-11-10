using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaceMountPoint : MonoBehaviour
{
    EggPersonController eggPersonController;
    HandMountPoint handMountPoint;
    Quaternion armedRotation = new Quaternion(-0.456308216f, 0.169321358f, -0.135407642f, 0.863005161f);
    public bool isPlayer = false;
    public bool armed = false; 
    // Start is called before the first frame update
    void Start()
    {
        //handMountPoint = eggPersonController.handMountPoint;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (eggPersonController.armed && !armed)
        {
            //SetArmed();
        }else if(!eggPersonController.armed && armed)
        {
            //SetUnarmed();
        }
        */
    }

    public void SetArmed()
    {
        /*
        transform.localPosition = Vector3.zero;
        //transform.localRotation = handMountPoint.transform.rotation * -1;
        transform.localRotation = armedRotation;
        */
        armed = true;
    }

    public void SetUnarmed()
    {
        armed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (armed)
        {
            EggPersonController controller = other.GetComponent<EggPersonController>();

            if (controller != null && other.transform.root.name != transform.root.name)
            {
                controller.OnHit(Vector3.left * 10000);
            }
        }
    }
}
