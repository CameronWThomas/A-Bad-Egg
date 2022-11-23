using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public float yaw;
    public float targetYaw;
    public float pitch;
    public float targetPitch;
    public bool swingReleased;
    public Vector2 pitchMinMax = new Vector2(-40, 85);
    public Vector2 yawMinMax = new Vector2(-40, 85);
    public float rotationSmoothTime = .12f;
    public Vector3 rotationSmoothVelocity;
    public Vector3 currentRotation;
    public float yAtRelease;

    EggPersonController eggPersonController;
    // Start is called before the first frame update
    void Start()
    {
        eggPersonController = GetComponent<EggPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PrimeSwing()
    {
        yaw = transform.rotation.y + 90;
        pitch = -2.5f;
    }

    public void ReleaseSwing()
    {
        eggPersonController.swingCooldown = true;
        targetYaw = yaw - 180;
        targetPitch = pitch;
        swingReleased = true;
        eggPersonController.mountPoint.swingReleased = true;
        yAtRelease = eggPersonController.epc.transform.eulerAngles.y;
    }

    public void StopSwinging()
    {
        eggPersonController.eggAnimator.SetSwinging(false); 
        eggPersonController.swinging = false;
        eggPersonController.mountPoint.swinging = false;
        yaw = 0;
        pitch = 0;
        targetYaw = 0;
        targetPitch = 0;
        yAtRelease = 0;
        //camController.SetCombatMode(eggPersonController.swinging);
        currentRotation = new Vector3(0, 0, 0);
    }
    public void RotateToTarget()
    {
        if (eggPersonController.swinging || swingReleased)
        {
            if (swingReleased)
            {
                DecreaseYaw();
            }
            else
            {
                //yaw = initialYaw + eggPersonController.epc.transform.eulerAngles.y;
                //targetYaw += initialYaw - 180 + eggPersonController.epc.transform.eulerAngles.y;
            }

            var yEuler = (transform.eulerAngles.y > 180) ? transform.eulerAngles.y - 360 : transform.eulerAngles.y;
            //var yEuler = transform.eulerAngles.y;

            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yEuler + yaw), ref rotationSmoothVelocity, rotationSmoothTime);
            eggPersonController.epc.transform.eulerAngles = currentRotation;
            //eggPersonController.epc.transform.rotation = new Quaternion(currentRotation.x, currentRotation.y, currentRotation.z, eggPersonController.epc.transform.rotation.w);

            if (yaw <= targetYaw)
            {
                swingReleased = false;
                eggPersonController.mountPoint.swingReleased = false;
                StopSwinging();
            }
        }
    }
    public void DecreaseYaw()
    {

        yaw = Mathf.Lerp(yaw, targetYaw - 20, Time.deltaTime * 5);
    }

    
}
