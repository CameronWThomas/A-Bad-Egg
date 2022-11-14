using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggAnimator : MonoBehaviour
{
    public Animator animator;
    EggPersonController eggPersonController;
    // Start is called before the first frame update
    void Start()
    {

        animator = GetComponent<Animator>();
        eggPersonController = GetComponent<EggPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        //speed
        float animationSpeedPercent = ((eggPersonController.running) ? eggPersonController.currentSpeed / eggPersonController.runSpeed : eggPersonController.currentSpeed / eggPersonController.walkSpeed * .5f);
        animator.SetFloat("speedPercent", animationSpeedPercent, eggPersonController.speedSmoothTime, Time.deltaTime);

    }


    public void SetArmed(bool isArmed)
    {
        animator.SetBool("armed", isArmed);
    }
    public void Swing()
    {
        animator.SetTrigger("swing");
    }
    public void SetSwinging(bool swinging)
    {
        animator.SetBool("swinging", swinging);
        /*
        int idx = animator.GetLayerIndex("ArmMask");
        animator.SetLayerWeight(idx, swinging ? 1f : 0f);
        */
    }

    public void OnJumping()
    {
        //trigger jump animation
    }
}
