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
        targetYaw = yaw - 180;
        targetPitch = pitch;
        swingReleased = true;
        yAtRelease = eggPersonController.epc.transform.eulerAngles.y;
    }

    public void StopSwinging()
    {

        eggPersonController.swinging = false;
        eggPersonController.mountPoint.swinging = false;
        yaw = 0;
        pitch = 0;
        targetYaw = 0;
        targetPitch = 0;
        yAtRelease = 0;
        //camController.SetCombatMode(eggPersonController.swinging);
        eggPersonController.eggAnimator.SetSwinging(false);
        currentRotation = new Vector3(0, 0, 0);
    }

    public void DecreaseYaw()
    {

        yaw = Mathf.Lerp(yaw, targetYaw - 20, Time.deltaTime * 5);
    }

    
}
